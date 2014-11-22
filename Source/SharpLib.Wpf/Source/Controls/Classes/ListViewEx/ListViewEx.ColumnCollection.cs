using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using SharpLibWpf;

namespace SharpLib.Wpf.Controls
{
    public class ListViewColumnCollection : ObservableCollection<ListViewColumn>
    {
        #region Поля

        private readonly Dictionary<string, ListViewColumn> _columns = new Dictionary<string, ListViewColumn>();

        #endregion

        #region Свойства

        public ListViewColumn this[string fieldName]
        {
            get
            {
                ListViewColumn column;

                _columns.TryGetValue(fieldName, out column);

                return column;
            }
        }

        #endregion

        #region Конструктор

        public ListViewColumnCollection()
        {
            _columns = new Dictionary<string, ListViewColumn>();
        }

        #endregion

        #region Методы

        protected override void ClearItems()
        {
            _columns.Clear();

            base.ClearItems();
        }

        protected override void InsertItem(int index, ListViewColumn item)
        {
            if (item != null)
            {
                _columns.Add(item.FieldName, item);
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var column = this[index];

            if (column != null)
            {
                _columns.Remove(column.FieldName);
            }

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, ListViewColumn item)
        {
            if (item != null && item.FieldName.IsNotValid())
            {
                throw new ArgumentException("Колонка должна иметь имя FieldName");
            }

            var column = this[index];

            if ((column != null) && (column != item))
            {
                _columns.Remove(column.FieldName);
            }

            if ((item != null) && (column != item))
            {
                _columns.Add(item.FieldName, item);
            }

            base.SetItem(index, item);
        }

        #endregion
    }
}