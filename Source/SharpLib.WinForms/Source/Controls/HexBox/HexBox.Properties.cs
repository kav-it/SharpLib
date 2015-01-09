﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Свойства HexBox, отображаемые в дизайнере
    /// </summary>
    public partial class HexBox
    {
        #region Константы

        /// <summary>
        /// Название категории в которого отображаются свойства
        /// </summary>
        private const string CATEGORY_NAME = "SharpLib";

        /// <summary>
        /// Стиль рамки элемента по умолчанию
        /// </summary>
        private const string DEFAULT_BORDER_STYLE = "Fixed3D";

        /// <summary>
        /// Количество байт в строке по умолчанию
        /// </summary>
        private const int DEFAULT_BYTE_PES_LINE = 16;

        /// <summary>
        /// Цвет элемента Disable по умолчанию
        /// </summary>
        private const string DEFAULT_DISABLE_COLOR_NAME = "WhiteSmoke";

        /// <summary>
        /// Количество байт в группе
        /// </summary>
        private const int DEFAULT_GROUP_SIZE = 4;

        /// <summary>
        /// Режим "Только чтение" 
        /// </summary>
        private const bool DEFAULT_READ_ONLY = true;

        /// <summary>
        /// Количество байт в строке фиксировано
        /// </summary>
        private const bool DEFAULT_USE_FIXED_BYTES_PER_LINE = true;

        /// <summary>
        /// Видимость линий разделителей групп
        /// </summary>
        private const bool DEFAULT_GROUP_SEPARATOR_VISIBLE = true;

        /// <summary>
        /// Видимость линии "Смещение"
        /// </summary>
        private const bool DEFAULT_HEADER_OFFSET_VISIBLE = true;

        /// <summary>
        /// Видимость колонки "Адрес"
        /// </summary>
        private const bool DEFAULT_COLUMN_ADDR_VISIBLE = true;

        /// <summary>
        /// Видимость колонки "Ascii"
        /// </summary>
        private const bool DEFAULT_COLUMN_ASCII_VISIBLE = true;

        /// <summary>
        /// Видимость вертикальной полосы прокрутки
        /// </summary>
        private const bool DEFAULT_VSCROLL_BAR_VISIBLE = true;

        #endregion

        #region Свойства

        /// <summary>
        /// Цвет фона, когда элемент Disable
        /// </summary>
        [Category(CATEGORY_NAME), Description("Цвет фона, когда элемент Disable")]
        [DefaultValue(typeof(Color), DEFAULT_DISABLE_COLOR_NAME)]
        public Color BackColorDisabled { get; set; }

        /// <summary>
        /// Режим "Только чтение"
        /// </summary>
        [Category(CATEGORY_NAME), Description("true: Только чтение")]
        [DefaultValue(DEFAULT_READ_ONLY)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                if (_readOnly == value)
                {
                    return;
                }

                _readOnly = value;
                OnReadOnlyChanged(EventArgs.Empty);
                Invalidate();
            }
        }

        /// <summary>
        /// Количество байт в одной строке
        /// </summary>
        [Category(CATEGORY_NAME), Description("Количество байт в одной строке")]
        [DefaultValue(DEFAULT_BYTE_PES_LINE)]
        public int BytesPerLine
        {
            get { return _bytesPerLine; }
            set
            {
                if (_bytesPerLine == value)
                {
                    return;
                }

                _bytesPerLine = value;
                OnBytesPerLineChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// Количество байт в группе (используется если GroupSeparatorVisible = true)
        /// </summary>
        [Category(CATEGORY_NAME), Description("Количество байт в одной строке")]
        [DefaultValue(DEFAULT_GROUP_SIZE)]
        public int GroupSize
        {
            get { return _groupSize; }
            set
            {
                if (_groupSize == value)
                {
                    return;
                }

                _groupSize = value;
                OnGroupSizeChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// true: Количество байт в одной строке фиксировано
        /// </summary>
        [Category(CATEGORY_NAME), Description("true: Количество байт в одной строке фиксировано")]
        [DefaultValue(DEFAULT_USE_FIXED_BYTES_PER_LINE)]
        public bool UseFixedBytesPerLine
        {
            get { return _useFixedBytesPerLine; }
            set
            {
                if (_useFixedBytesPerLine == value)
                {
                    return;
                }

                _useFixedBytesPerLine = value;
                OnUseFixedBytesPerLineChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// true: Отображение вертикального ScrollBar
        /// </summary>
        [Category(CATEGORY_NAME), Description("true: Отображение вертикального ScrollBar")]
        [DefaultValue(DEFAULT_VSCROLL_BAR_VISIBLE)]
        public bool VScrollBarVisible
        {
            get { return _vScrollBarVisible; }
            set
            {
                if (_vScrollBarVisible == value)
                {
                    return;
                }

                _vScrollBarVisible = value;

                if (_vScrollBarVisible)
                {
                    Controls.Add(_vScrollBar);
                }
                else
                {
                    Controls.Remove(_vScrollBar);
                }

                UpdateRectanglePositioning();
                UpdateScrollSize();

                OnVScrollBarVisibleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// true: Отображается разделитель групп
        /// </summary>
        [Category(CATEGORY_NAME), Description("true: Отображается разделитель групп")]
        [DefaultValue(DEFAULT_GROUP_SEPARATOR_VISIBLE)]
        public bool GroupSeparatorVisible
        {
            get { return _groupSeparatorVisible; }
            set
            {
                if (_groupSeparatorVisible == value)
                {
                    return;
                }

                _groupSeparatorVisible = value;
                OnGroupSeparatorVisibleChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// true: Отображается информация о колонке
        /// </summary>
        [Category(CATEGORY_NAME), Description("true: Отображается верняя линия смещения байт")]
        [DefaultValue(DEFAULT_HEADER_OFFSET_VISIBLE)]
        public bool HeaderOffsetVisible
        {
            get { return _headerOffsetVisible; }
            set
            {
                if (_headerOffsetVisible == value)
                {
                    return;
                }

                _headerOffsetVisible = value;
                OnColumnInfoVisibleChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// true: Отображается информация о строке
        /// </summary>
        [Category(CATEGORY_NAME), Description("true: Отображается информация о строке")]
        [DefaultValue(DEFAULT_COLUMN_ADDR_VISIBLE)]
        public bool ColumnAddrVisible
        {
            get { return _columnAddrVisible; }
            set
            {
                if (_columnAddrVisible == value)
                {
                    return;
                }

                _columnAddrVisible = value;
                OnLineInfoVisibleChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// Смещение информационной строки
        /// </summary>
        [Category(CATEGORY_NAME), Description("Смещение информационной строки")]
        [DefaultValue((long)0)]
        public long LineInfoOffset
        {
            get { return _lineInfoOffset; }
            set
            {
                if (_lineInfoOffset == value)
                {
                    return;
                }

                _lineInfoOffset = value;

                Invalidate();
            }
        }

        /// <summary>
        /// Стиль рамки элемента
        /// </summary>
        [Category(CATEGORY_NAME), Description("Стиль рамки элемента")]
        [DefaultValue(typeof(BorderStyle), DEFAULT_BORDER_STYLE)]
        public BorderStyle BorderStyle
        {
            get { return _borderStyle; }
            set
            {
                if (_borderStyle == value)
                {
                    return;
                }

                _borderStyle = value;
                switch (_borderStyle)
                {
                    case BorderStyle.None:
                        _recBorderLeft = _recBorderTop = _recBorderRight = _recBorderBottom = 0;
                        break;
                    case BorderStyle.Fixed3D:
                        _recBorderLeft = _recBorderRight = SystemInformation.Border3DSize.Width;
                        _recBorderTop = _recBorderBottom = SystemInformation.Border3DSize.Height;
                        break;
                    case BorderStyle.FixedSingle:
                        _recBorderLeft = _recBorderTop = _recBorderRight = _recBorderBottom = 1;
                        break;
                }

                UpdateRectanglePositioning();

                OnBorderStyleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// true: Отображается вид строки
        /// </summary>
        [Category(CATEGORY_NAME), Description("true: Отображается колонка 'Ascii'")]
        [DefaultValue(DEFAULT_COLUMN_ASCII_VISIBLE)]
        public bool ColumnAsciiVisible
        {
            get { return _columnAsciiVisible; }
            set
            {
                if (_columnAsciiVisible == value)
                {
                    return;
                }

                _columnAsciiVisible = value;
                OnStringViewVisibleChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// Выбор отображения HEX-байт (upper или lower)
        /// </summary>
        [Category(CATEGORY_NAME), Description("Выбор отображения HEX-байт (upper или lower)")]
        [DefaultValue(typeof(HexCasing), "Upper")]
        public HexCasing HexCasing
        {
            get
            {
                if (_hexStringFormat == "X")
                {
                    return HexCasing.Upper;
                }
                return HexCasing.Lower;
            }
            set
            {
                var format = value == HexCasing.Upper ? "X" : "x";

                if (_hexStringFormat == format)
                {
                    return;
                }

                _hexStringFormat = format;
                OnHexCasingChanged(EventArgs.Empty);

                Invalidate();
            }
        }

        /// <summary>
        /// Цвет поля информации
        /// </summary>
        [Category(CATEGORY_NAME), Description("Цвет поля информации")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color InfoForeColor
        {
            get { return _infoForeColor; }
            set
            {
                _infoForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Цвет фона выбранных байт
        /// </summary>
        [Category(CATEGORY_NAME), Description("Цвет фона выбранных байт")]
        [DefaultValue(typeof(Color), "Blue")]
        public Color SelectionBackColor
        {
            get { return _selectionBackColor; }
            set
            {
                _selectionBackColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Цвет текста выбранных байт
        /// </summary>
        [Category(CATEGORY_NAME), Description("Цвет фона выбранных байт")]
        [DefaultValue(typeof(Color), "White")]
        public Color SelectionForeColor
        {
            get { return _selectionForeColor; }
            set
            {
                _selectionForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// true: Отображается тень выделения
        /// </summary>
        [Category(CATEGORY_NAME), Description("Отображается тень выделения")]
        [DefaultValue(true)]
        public bool ShadowSelectionVisible
        {
            get { return _shadowSelectionVisible; }
            set
            {
                if (_shadowSelectionVisible == value)
                {
                    return;
                }
                _shadowSelectionVisible = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Цвет тени выделения (должен использоваться alpha-канал)
        /// </summary>
        [Category(CATEGORY_NAME), Description("Цвет тени выделения")]
        public Color ShadowSelectionColor
        {
            get { return _shadowSelectionColor; }
            set
            {
                _shadowSelectionColor = value;
                Invalidate();
            }
        }

        #endregion

        #region События

        /// <summary>
        /// Изменилось свойство BorderStyle
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство BorderStyle")]
        public event EventHandler BorderStyleChanged;

        /// <summary>
        /// Изменилось свойство ByteProvider
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство ByteProvider")]
        public event EventHandler ByteProviderChanged;

        /// <summary>
        /// Изменилось свойство BytesPerLine
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство BytesPerLine")]
        public event EventHandler BytesPerLineChanged;

        /// <summary>
        /// Изменилось свойство CharSize
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство CharSize")]
        public event EventHandler CharSizeChanged;

        /// <summary>
        /// Изменилось свойство ColumnInfoVisibleChanged
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство ColumnInfoVisibleChanged")]
        public event EventHandler ColumnInfoVisibleChanged;

        /// <summary>
        /// Метод Copy сгенерировал событие ClipBoardData
        /// </summary>
        [Category(CATEGORY_NAME), Description("Метод Copy сгенерировал событие ClipBoardData")]
        public event EventHandler Copied;

        /// <summary>
        /// Метод Copy в Hex-редакторе
        /// </summary>
        [Category(CATEGORY_NAME), Description("Метод Copy в Hex-редакторе")]
        public event EventHandler CopiedHex;

        /// <summary>
        /// Изменилось свойство CurrentLineChanged
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство CurrentLineChanged")]
        public event EventHandler CurrentLineChanged;

        /// <summary>
        /// Изменилось свойство CurrentPositionInLineChanged
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство CurrentPositionInLineChanged")]
        public event EventHandler CurrentPositionInLineChanged;

        /// <summary>
        /// Изменилось свойство GroupSeparatorVisibleChanged
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство GroupSeparatorVisibleChanged")]
        public event EventHandler GroupSeparatorVisibleChanged;

        /// <summary>
        /// Изменилось свойство ColumnWidth
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство ColumnWidth")]
        public event EventHandler GroupSizeChanged;

        /// <summary>
        /// Изменилось свойство HexCasingChanged
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство HexCasingChanged")]
        public event EventHandler HexCasingChanged;

        /// <summary>
        /// Изменилось свойство HorizontalByteCount
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство HorizontalByteCount")]
        public event EventHandler HorizontalByteCountChanged;

        /// <summary>
        /// Изменилось свойство InsertActive
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство InsertActive")]
        public event EventHandler InsertActiveChanged;

        /// <summary>
        /// Изменилось свойство ColumnAddrVisible
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство ColumnAddrVisible")]
        public event EventHandler LineInfoVisibleChanged;

        /// <summary>
        /// Изменилось свойство ReadOnly
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство ReadOnly")]
        public event EventHandler ReadOnlyChanged;

        /// <summary>
        /// Изменилось свойство RequiredWidth
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство RequiredWidth")]
        public event EventHandler RequiredWidthChanged;

        /// <summary>
        /// Изменилось свойство SelectionLength
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство SelectionLength")]
        public event EventHandler SelectionLengthChanged;

        /// <summary>
        /// Изменилось свойство SelectionStart
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство SelectionStart")]
        public event EventHandler SelectionStartChanged;

        /// <summary>
        /// Изменилось свойство ColumnAsciiVisible
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство ColumnAsciiVisible")]
        public event EventHandler StringViewVisibleChanged;

        /// <summary>
        /// Изменилось свойство UseFixedBytesPerLine
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство UseFixedBytesPerLine")]
        public event EventHandler UseFixedBytesPerLineChanged;

        /// <summary>
        /// Изменилось свойство VScrollBarVisible
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство VScrollBarVisible")]
        public event EventHandler VScrollBarVisibleChanged;

        /// <summary>
        /// Изменилось свойство VerticalByteCount
        /// </summary>
        [Category(CATEGORY_NAME), Description("Изменилось свойство VerticalByteCount")]
        public event EventHandler VerticalByteCountChanged;

        #endregion
    }
}