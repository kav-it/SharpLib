using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.WinForms.Controls
{
    public class MemoLineCollection : IList<string>
    {
        #region Поля

        private readonly MemoControl _control;

        private readonly List<string> _lines;

        /// <summary>
        /// Максимальная длина строки
        /// </summary>
        internal int MaxLengthX { get; private set; }

        #endregion

        #region Свойства

        public int Count
        {
            get { return _lines.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public string this[int index]
        {
            get { return _lines[index]; }
            set
            {
                _lines[index] = value;
                UpdateMaxX();
                Refresh();
            }
        }

        #endregion

        #region Конструктор

        internal MemoLineCollection(MemoControl control)
        {
            _control = control;
            MaxLengthX = 0;
            _lines = new List<string>();
        }

        #endregion

        #region Методы

        private void Refresh()
        {
            // Перерасчет максимальной длины строки
            UpdateMaxX();
            // Уведомление элемента, об изменении данных
            _control.ChangeData();
        }

        /// <summary>
        /// Перерасчет максимальной длины строки
        /// </summary>
        private void UpdateMaxX()
        {
            MaxLengthX = _lines.Any() ? _lines.Max(x => x.Length) : 0;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string item)
        {
            _lines.Add(item);

            Refresh();
        }

        public void Clear()
        {
            _lines.Clear();

            Refresh();
        }

        public bool Contains(string item)
        {
            return _lines.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _lines.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            if (!_lines.Remove(item))
            {
                return false;
            }

            Refresh();

            return true;
        }

        public int IndexOf(string item)
        {
            return _lines.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            _lines.Insert(index, item);

            Refresh();
        }

        public void RemoveAt(int index)
        {
            _lines.RemoveAt(index);
            Refresh();
        }

        public void AddRange(IEnumerable<string> values)
        {
            if (values == null)
            {
                return;
            }

            _lines.AddRange(values);

            Refresh();
        }

        #endregion
    }
}