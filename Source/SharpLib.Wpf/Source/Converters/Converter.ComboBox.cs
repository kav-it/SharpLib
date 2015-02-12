using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Wpf.Converters
{
    public class ComboBoxConverterItem<T>
    {
        #region Свойства

        public int Index { get; set; }

        public T Value { get; set; }

        public String Text { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Text;
        }

        #endregion
    }

    public class ComboBoxConverterTable<T> : IEnumerable<ComboBoxConverterItem<T>>
    {

        #region Поля

        #endregion

        #region Свойства

        public List<ComboBoxConverterItem<T>> Items { get; set; }

        #endregion

        #region Конструктор

        public ComboBoxConverterTable()
        {
            Items = new List<ComboBoxConverterItem<T>>();
        }

        #endregion

        #region Методы

        public void Clear()
        {
            Items.Clear();
        }

        public void Add(T value, String text)
        {
            var item = new ComboBoxConverterItem<T>
            {
                Index = Items.Count,
                Value = value,
                Text = text
            };

            Items.Add(item);
        }

        public void AddRange(IDictionary<T, string> values)
        {
        }

        public T IndexToValue(int index)
        {
            foreach (ComboBoxConverterItem<T> item in Items)
            {
                if (item.Index == index)
                {
                    return item.Value;
                }
            }

            return Items[0].Value;
        }

        public int ValueToIndex(T value)
        {
            foreach (ComboBoxConverterItem<T> item in Items)
            {
                if (item.Value.Equals(value))
                    return item.Index;
            }

            return -1;
        }

        public IEnumerator<ComboBoxConverterItem<T>> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}