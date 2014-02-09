// ****************************************************************************
//
// Имя файла    : 'ConvWpf.cs'
// Заголовок    : Конверторы визуального представления WPF
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 06/06/2012
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SharpLib
{

    #region Класс ByteArrayConverter

    /// <summary>
    /// Конвертор "Массив байт" <-> "Текстовое преставление"
    /// <example>0x01 0x02 0x03 <->01 02 03</example>
    /// </summary>
    [ValueConversion(typeof(Byte[]), typeof(String))]
    public class ByteArrayConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Byte[] buffer = (Byte[])value;

            String text = buffer.ToAsciiEx();

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ByteArrayConverter

    #region Класс AsciiArrayConverter

    /// <summary>
    /// Конвертор "Массив байт" <-> "Ascii преставление"
    /// <example>0x01 0x02 0x03 0x02 0x31 0x32<->'... 12'</example>
    /// </summary>
    [ValueConversion(typeof(Byte[]), typeof(String))]
    public class AsciiArrayConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Byte[] buffer = (Byte[])value;

            String text = Conv.BufferToString(buffer, ".");

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс AsciiArrayConverter

    #region Класс BooleanTypConverter

    /// <summary>
    /// Конвертор "Value" <-> "Boolean"
    /// <example>0<->false</example>
    /// <example>1<->true</example>
    /// <example>"a"<->true</example>
    /// </summary>
    [ValueConversion(typeof(Boolean), typeof(Object))]
    public class BooleanTypConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is String)
                {
                    String text = value as String;

                    return text != String.Empty;
                }

                int data = (int)value;

                return (data != 0);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс BooleanTypConverter

    #region Класс VisibilityConverter

    /// <summary>
    /// Конвертор "Boolean" <-> "Visibly"
    /// <para>0: Преобразовние Bool <-> Visibly</para>
    /// <para>1: Преобразовние null <-> Visibly</para>
    /// <para>2: Преобразовние ""   <-> Visibly</para>
    /// <para>3: Преобразовние 0    <-> Visibly</para>
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Object obj = value;
            String param;

            if (parameter == null)
                param = "0";
            else
                param = (String)parameter;

            if (param == "0")
            {
                Boolean data = (Boolean)obj;
                if (data == false)
                    return Visibility.Collapsed;
            }
            else if (param == "1")
            {
                if (obj == null)
                    return Visibility.Collapsed;
            }
            else if (param == "2")
            {
                String data = (String)obj;
                if (data == null || data == "")
                    return Visibility.Collapsed;
            }
            else if (param == "3")
            {
                int data = (int)obj;
                if (data == 0)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс VisibilityConverter

    #region Класс OpacityTypConverter

    /// <summary>
    /// Конвертор "Value" <-> "Boolean"
    /// <example>0<->false</example>
    /// <example>1<->true</example>
    /// <example>"a"<->true</example>
    /// </summary>
    [ValueConversion(typeof(Boolean), typeof(Object))]
    public class OpacityTypConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is String)
                {
                    String text = value as String;

                    return text != String.Empty ? 1 : 0.3;
                }

                int data = (int)value;

                return data != 0 ? 1 : 0.3;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс OpacityTypConverter

    #region Класс DallasRomIdConverter

    /// <summary>
    /// Конвертор "RomId" <-> "Текстовое преставление"
    /// <example>param = 0: 0x0102030405060708 -> 01 02 03 04 05 06 07 08</example>
    /// </summary>
    [ValueConversion(typeof(UInt64), typeof(String))]
    public class DallasRomIdConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UInt64 romId = (UInt64)value;
            String param;

            if (romId == 0) return "";

            if (parameter == null)
                param = "0";
            else
                param = (String)parameter;

            if (param == "0")
            {
                Byte[] buffer = new Byte[8];
                Mem.PutByte64(buffer, 0, romId, Endianess.Big);

                String result = buffer.ToAsciiEx();

                return result;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс DallasRomIdConverter

    #region Класс ExDateTimeConverter

    /// <summary>
    /// Конвертор "Время" <-> "Текстовое преставление"
    /// <example>param = 0: DateTime -> 20/06/80 08:30:00.123</example>
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class ExDateTimeConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            String param;

            if (parameter == null)
                param = "0";
            else
                param = (String)parameter;

            if (param == "0")
            {
                String result = dt.ToStringEx();

                return result;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ExDateTimeConverter

    #region Класс ComboBoxConverterItem

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

    #endregion Класс ComboBoxConverterItem

    #region Класс ComboBoxConverterTable

    public class ComboBoxConverterTable<T>
    {
        #region Поля

        private List<ComboBoxConverterItem<T>> _items;

        #endregion

        #region Свойства

        public List<ComboBoxConverterItem<T>> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #endregion

        #region Конструктор

        public ComboBoxConverterTable()
        {
            _items = new List<ComboBoxConverterItem<T>>();
        }

        #endregion

        #region Методы

        public void Clear()
        {
            _items.Clear();
        }

        public void Add(T value, String text)
        {
            ComboBoxConverterItem<T> item = new ComboBoxConverterItem<T>();
            item.Index = _items.Count;
            item.Value = value;
            item.Text = text;

            _items.Add(item);
        }

        public T IndexToValue(int index)
        {
            foreach (ComboBoxConverterItem<T> item in Items)
            {
                if (item.Index == index)
                    return item.Value;
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

        #endregion
    }

    #endregion Класс ComboBoxConverterTable
}