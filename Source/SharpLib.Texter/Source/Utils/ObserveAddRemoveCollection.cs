using System;
using System.Collections.ObjectModel;

namespace SharpLib.Texter.Utils
{
    internal sealed class ObserveAddRemoveCollection<T> : Collection<T>
    {
        #region Поля

        private readonly Action<T> onAdd;

        private readonly Action<T> onRemove;

        #endregion

        #region Конструктор

        public ObserveAddRemoveCollection(Action<T> onAdd, Action<T> onRemove)
        {
            if (onAdd == null)
            {
                throw new ArgumentNullException("onAdd");
            }
            if (onRemove == null)
            {
                throw new ArgumentNullException("onRemove");
            }
            this.onAdd = onAdd;
            this.onRemove = onRemove;
        }

        #endregion

        #region Методы

        protected override void ClearItems()
        {
            if (onRemove != null)
            {
                foreach (T val in this)
                {
                    onRemove(val);
                }
            }
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            if (onAdd != null)
            {
                onAdd(item);
            }
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (onRemove != null)
            {
                onRemove(this[index]);
            }
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            if (onRemove != null)
            {
                onRemove(this[index]);
            }
            try
            {
                if (onAdd != null)
                {
                    onAdd(item);
                }
            }
            catch
            {
                base.RemoveAt(index);
                throw;
            }
            base.SetItem(index, item);
        }

        #endregion
    }
}