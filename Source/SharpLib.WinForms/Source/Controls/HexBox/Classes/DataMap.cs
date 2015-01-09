using System;
using System.Collections;

namespace SharpLib.WinForms.Controls
{
    internal class DataMap : ICollection
    {
        #region Поля

        internal int _version;

        #endregion

        #region Свойства

        public DataBlock FirstBlock { get; internal set; }

        public int Count { get; internal set; }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot { get; private set; }

        #endregion

        #region Конструктор

        public DataMap()
        {
            SyncRoot = new object();
        }

        public DataMap(IEnumerable collection)
        {
            SyncRoot = new object();
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (DataBlock item in collection)
            {
                AddLast(item);
            }
        }

        #endregion

        #region Методы

        public void AddAfter(DataBlock block, DataBlock newBlock)
        {
            AddAfterInternal(block, newBlock);
        }

        public void AddBefore(DataBlock block, DataBlock newBlock)
        {
            AddBeforeInternal(block, newBlock);
        }

        public void AddFirst(DataBlock block)
        {
            if (FirstBlock == null)
            {
                AddBlockToEmptyMap(block);
            }
            else
            {
                AddBeforeInternal(FirstBlock, block);
            }
        }

        public void AddLast(DataBlock block)
        {
            if (FirstBlock == null)
            {
                AddBlockToEmptyMap(block);
            }
            else
            {
                AddAfterInternal(GetLastBlock(), block);
            }
        }

        public void Remove(DataBlock block)
        {
            RemoveInternal(block);
        }

        public void RemoveFirst()
        {
            if (FirstBlock == null)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            RemoveInternal(FirstBlock);
        }

        public void RemoveLast()
        {
            if (FirstBlock == null)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            RemoveInternal(GetLastBlock());
        }

        public DataBlock Replace(DataBlock block, DataBlock newBlock)
        {
            AddAfterInternal(block, newBlock);
            RemoveInternal(block);
            return newBlock;
        }

        public void Clear()
        {
            DataBlock block = FirstBlock;
            while (block != null)
            {
                DataBlock nextBlock = block.NextBlock;
                InvalidateBlock(block);
                block = nextBlock;
            }
            FirstBlock = null;
            Count = 0;
            _version++;
        }

        private void AddAfterInternal(DataBlock block, DataBlock newBlock)
        {
            newBlock._previousBlock = block;
            newBlock._nextBlock = block._nextBlock;
            newBlock._map = this;

            if (block._nextBlock != null)
            {
                block._nextBlock._previousBlock = newBlock;
            }
            block._nextBlock = newBlock;

            _version++;
            Count++;
        }

        private void AddBeforeInternal(DataBlock block, DataBlock newBlock)
        {
            newBlock._nextBlock = block;
            newBlock._previousBlock = block._previousBlock;
            newBlock._map = this;

            if (block._previousBlock != null)
            {
                block._previousBlock._nextBlock = newBlock;
            }
            block._previousBlock = newBlock;

            if (FirstBlock == block)
            {
                FirstBlock = newBlock;
            }
            _version++;
            Count++;
        }

        private void RemoveInternal(DataBlock block)
        {
            DataBlock previousBlock = block._previousBlock;
            DataBlock nextBlock = block._nextBlock;

            if (previousBlock != null)
            {
                previousBlock._nextBlock = nextBlock;
            }

            if (nextBlock != null)
            {
                nextBlock._previousBlock = previousBlock;
            }

            if (FirstBlock == block)
            {
                FirstBlock = nextBlock;
            }

            InvalidateBlock(block);

            Count--;
            _version++;
        }

        private DataBlock GetLastBlock()
        {
            DataBlock lastBlock = null;
            for (DataBlock block = FirstBlock; block != null; block = block.NextBlock)
            {
                lastBlock = block;
            }
            return lastBlock;
        }

        private void InvalidateBlock(DataBlock block)
        {
            block._map = null;
            block._nextBlock = null;
            block._previousBlock = null;
        }

        private void AddBlockToEmptyMap(DataBlock block)
        {
            block._map = this;
            block._nextBlock = null;
            block._previousBlock = null;

            FirstBlock = block;
            _version++;
            Count++;
        }

        public void CopyTo(Array array, int index)
        {
            DataBlock[] blockArray = array as DataBlock[];
            for (DataBlock block = FirstBlock; block != null; block = block.NextBlock)
            {
                blockArray[index++] = block;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Вложенный класс: Enumerator

        internal class Enumerator : IEnumerator, IDisposable
        {
            #region Поля

            private readonly DataMap _map;

            private readonly int _version;

            private DataBlock _current;

            private int _index;

            #endregion

            #region Свойства

            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0 || _index > _map.Count)
                    {
                        throw new InvalidOperationException("Enumerator is positioned before the first element or after the last element of the collection.");
                    }
                    return _current;
                }
            }

            #endregion

            #region Конструктор

            internal Enumerator(DataMap map)
            {
                _map = map;
                _version = map._version;
                _current = null;
                _index = -1;
            }

            #endregion

            #region Методы

            public bool MoveNext()
            {
                if (_version != _map._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                if (_index >= _map.Count)
                {
                    return false;
                }

                if (++_index == 0)
                {
                    _current = _map.FirstBlock;
                }
                else
                {
                    _current = _current.NextBlock;
                }

                return (_index < _map.Count);
            }

            void IEnumerator.Reset()
            {
                if (_version != _map._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                _index = -1;
                _current = null;
            }

            public void Dispose()
            {
            }

            #endregion
        }

        #endregion
    }
}