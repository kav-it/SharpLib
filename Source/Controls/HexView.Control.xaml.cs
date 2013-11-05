//*****************************************************************************
//
// Имя файла    : 'HexView.Control.cs'
// Заголовок    : Компонент "Hex/Ascii" представление буфера
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 29/10/2012
//
//*****************************************************************************
			
using System;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib
{
    #region Класс HexViewControl
    public partial class HexViewControl : UserControl
    {
        #region Константы
        private const int ORG_DEFAULT = 16;
        #endregion Константы

        #region Поля
        private int _bytesInLine;
        private int _address;
        #endregion Поля

        #region Свойства
        public Byte[] Buffer
        {
            get { return (Byte[])GetValue(BufferProperty); }
            set { SetValue(BufferProperty, value); }
        }
        public int BytesInLine
        {
            get { return _bytesInLine; }
            set 
            {
                if (_bytesInLine != value)
                {
                    _bytesInLine = value;
                    UpdateView();
                }
            }
        }
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }
        #endregion Свойства

        #region Свойства зависимости
        public static readonly DependencyProperty BufferProperty;
        #endregion Свойства зависимости
        
        #region Конструктор
        static HexViewControl()
        {
            BufferProperty = DependencyProperty.Register("Buffer", typeof(Byte[]), typeof(HexViewControl),
                new PropertyMetadata(null, new PropertyChangedCallback(OnBufferPropertyChanged)));
        }
        public HexViewControl()
        {
            InitializeComponent();

            _bytesInLine = ORG_DEFAULT;
            _address = 0;
        }
        #endregion Конструктор

        #region Методы
        private static void OnBufferPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Byte[] text = (Byte[])e.NewValue;
            ((HexViewControl)sender).SetBuffer(text);
        }
        private void SetBuffer(Byte[] value)
        {
            if (PART_TextBox != null)
                PART_TextBox.Text = GenerateText(value);
        }
        private void UpdateView()
        {
            SetBuffer(Buffer);
        }
        private String GenerateText (Byte[] data)
        {
            if (data != null) 
            {
                int    bytesInLine  = BytesInLine;
                int    processSize  = 0;
                int    totalSize    = data.Length;
                String result       = "";

                // Добавление шапки
                //if (BytesInLine == 16)
                //{
                //    result += "  Address  |  0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 |                 " + "\r\n";
                //    result += "-------------------------------------------------------------|-----------------" + "\r\n";
                //}
                //else
                //{
                //    result += "  Address  |  0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 |                 " + "\r\n";
                //    result += "-------------------------------------------------------------------------------------------------------------|-----------------" + "\r\n";
                //}
                // result += "0x00AA00AA | 00 00 00 00 00 00 00 00 00 00 00 AA BB CC DD EE | 0123456789ABCDEF";

                while (processSize < totalSize)
                {
                    int size = totalSize - processSize;
                    if (size > bytesInLine) size = bytesInLine;

                    Byte[] buffer = Mem.Clone(data, processSize, size);

                    String addr  = Conv.IntToHex((UInt32)(Address + processSize));
                    String hex   = buffer.ToAsciiEx(" ");
                    String ascii = Conv.BufferToString(buffer, ".");

                    if (size < bytesInLine)
                    {
                        // Дополнение пробелами до полной строки
                        int remain = bytesInLine - size;
                        ascii      = ascii.PadRight(bytesInLine, ' ');
                        hex        = hex.PadRight(bytesInLine * 3 - 1, ' ');
                    }

                    String text  = String.Format("{0} | {1} | {2}", addr, hex, ascii) + "\r\n";

                    result      += text;
                    processSize += size;
                }

                return result;
            }

            return null;
        }
        /// <summary>
        /// Смена ширины данных
        /// </summary>
        private void CheckButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckButton button = sender as CheckButton;

            if (button != null)
            {
                String count = (String)button.Content;
                BytesInLine = count.ToIntEx();
            }
        }
        #endregion Методы
    }
    #endregion Класс HexViewControl
}
