using System;
using System.Collections.ObjectModel;

namespace SharpLib.Notepad.Utils
{
    [Serializable]
    public class NullSafeCollection<T> : Collection<T> where T : class
    {
        #region Методы

        protected override void InsertItem(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }

        #endregion
    }
}