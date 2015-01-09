using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using SharpLib.Native.Windows;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// ��������� "HexBox"
    /// </summary>
    public partial class HexBox : Control
    {
        #region ���������

        /// <summary>
        /// Contains the thumptrack delay for scrolling in milliseconds.
        /// </summary>
        private const int THUMB_TRACK_DELAY = 50;

        #endregion

        #region ����

        /// <summary>
        /// ���������� ����������� ����
        /// </summary>
        private readonly HexBoxContextMenu _builtInContextMenu;

        /// <summary>
        /// Contains string format information for text drawing
        /// </summary>
        private readonly StringFormat _stringFormat;

        /// <summary>
        /// Contains a timer for thumbtrack scrolling
        /// </summary>
        private readonly Timer _thumbTrackTimer;

        /// <summary>
        /// Contains a vertical scroll
        /// </summary>
        private readonly VScrollBar _vScrollBar;

        /// <summary>
        /// Contains true, if the find (Find method) should be aborted.
        /// </summary>
        private bool _abortFind;

        private BorderStyle _borderStyle;

        private IByteCharConverter _byteCharConverter;

        /// <summary>
        /// Contains the current char position in one byte
        /// </summary>
        /// <example>
        /// "1A"
        /// "1" = char position of 0
        /// "A" = char position of 1
        /// </example>
        private int _byteCharacterPos;

        /// <summary>
        /// Contains the current byte position
        /// </summary>
        private long _bytePos;

        private IByteProvider _byteProvider;

        private int _bytesPerLine;

        /// <summary>
        /// Contains True if caret is visible
        /// </summary>
        private bool _caretVisible;

        private SizeF _charSize;

        private bool _headerOffsetVisible;

        private long _currentLine;

        private int _currentPositionInLine;

        /// <summary>
        /// Contains an empty key interpreter without functionality
        /// </summary>
        private EmptyKeyInterpreter _eki;

        /// <summary>
        /// Contains the index of the last visible byte
        /// </summary>
        private long _endByte;

        /// <summary>
        /// Contains a value of the current finding position.
        /// </summary>
        private long _findingPos;

        private bool _groupSeparatorVisible;

        private int _groupSize;

        /// <summary>
        /// Contains string format information for hex values
        /// </summary>
        private string _hexStringFormat;

        /// <summary>
        /// Contains the maximum of visible bytes.
        /// </summary>
        private int _iHexMaxBytes;

        /// <summary>
        /// Contains the maximum of visible horizontal bytes
        /// </summary>
        private int _iHexMaxHBytes;

        /// <summary>
        /// Contains the maximum of visible vertical bytes
        /// </summary>
        private int _iHexMaxVBytes;

        private Color _infoForeColor;

        /// <summary>
        /// Contains a state value about Insert or Write mode. When this value is true and the ByteProvider SupportsInsert is true
        /// bytes are inserted instead of overridden.
        /// </summary>
        private bool _insertActive;

        /// <summary>
        /// Contains the current key interpreter
        /// </summary>
        private IKeyInterpreter _keyInterpreter;

        /// <summary>
        /// Contains the default key interpreter
        /// </summary>
        private KeyInterpreter _ki;

        /// <summary>
        /// Contains the Enviroment.TickCount of the last refresh
        /// </summary>
        private int _lastThumbtrack;

        private long _lineInfoOffset;

        private bool _columnAddrVisible;

        private bool _readOnly;

        /// <summary>
        /// Contains the border bottom shift
        /// </summary>
        private int _recBorderBottom;

        /// <summary>
        /// Contains the border�s left shift
        /// </summary>
        private int _recBorderLeft;

        /// <summary>
        /// Contains the border�s right shift
        /// </summary>
        private int _recBorderRight;

        /// <summary>
        /// Contains the border�s top shift
        /// </summary>
        private int _recBorderTop;

        /// <summary>
        /// Contains the column info header rectangle bounds
        /// </summary>
        private Rectangle _recColumnInfo;

        /// <summary>
        /// Contains the hole content bounds of all text
        /// </summary>
        private Rectangle _recContent;

        /// <summary>
        /// Contains the hex data bounds
        /// </summary>
        private Rectangle _recHex;

        /// <summary>
        /// Contains the line info bounds
        /// </summary>
        private Rectangle _recLineInfo;

        /// <summary>
        /// Contains the string view bounds
        /// </summary>
        private Rectangle _recStringView;

        private int _requiredWidth;

        /// <summary>
        /// Contains the scroll bars maximum value
        /// </summary>
        private long _scrollVmax;

        /// <summary>
        /// Contains the scroll bars minimum value
        /// </summary>
        private long _scrollVmin;

        /// <summary>
        /// Contains the scroll bars current position
        /// </summary>
        private long _scrollVpos;

        private Color _selectionBackColor;

        private Color _selectionForeColor;

        private long _selectionLength;

        private Color _shadowSelectionColor;

        private bool _shadowSelectionVisible;

        /// <summary>
        /// Contains the string key interpreter
        /// </summary>
        private StringKeyInterpreter _ski;

        /// <summary>
        /// Contains the index of the first visible byte
        /// </summary>
        private long _startByte;

        private bool _columnAsciiVisible;

        /// <summary>
        /// Contains the thumbtrack scrolling position
        /// </summary>
        private long _thumbTrackPosition;

        private bool _useFixedBytesPerLine;

        private bool _vScrollBarVisible;

        #endregion

        #region ��������

        /// <summary>
        /// Gets a value that indicates the current position during Find method execution.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long CurrentFindingPosition
        {
            get { return _findingPos; }
        }

        ///// <summary>
        ///// Gets or sets the background color for the control.
        ///// </summary>
        //[DefaultValue(typeof(Color), "White")]
        //public override Color BackColor
        //{
        //    get { return base.BackColor; }
        //    set { base.BackColor = value; }
        //}

        /// <summary>
        /// ����� ����������
        /// </summary>
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// ������� �������� Text �� ������������ � ���������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// ������� �������� �� ������������ � ���������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(false)]
        public override RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
        }

        /// <summary>
        /// Gets or sets the ByteProvider.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IByteProvider ByteProvider
        {
            get { return _byteProvider; }
            set
            {
                if (_byteProvider == value)
                {
                    return;
                }

                if (value == null)
                {
                    ActivateEmptyKeyInterpreter();
                }
                else
                {
                    ActivateKeyInterpreter();
                }

                if (_byteProvider != null)
                {
                    _byteProvider.LengthChanged -= _byteProvider_LengthChanged;
                }

                _byteProvider = value;
                if (_byteProvider != null)
                {
                    _byteProvider.LengthChanged += _byteProvider_LengthChanged;
                }

                OnByteProviderChanged(EventArgs.Empty);

                if (value == null) // do not raise events if value is null
                {
                    _bytePos = -1;
                    _byteCharacterPos = 0;
                    _selectionLength = 0;

                    DestroyCaret();
                }
                else
                {
                    SetPosition(0, 0);
                    SetSelectionLength(0);

                    if (_caretVisible && Focused)
                    {
                        UpdateCaret();
                    }
                    else
                    {
                        CreateCaret();
                    }
                }

                CheckCurrentLineChanged();
                CheckCurrentPositionInLineChanged();

                _scrollVpos = 0;

                UpdateVisibilityBytes();
                UpdateRectanglePositioning();

                Invalidate();
            }
        }

        /// <summary>
        /// Gets and sets the starting point of the bytes selected in the hex box.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long SelectionStart
        {
            get { return _bytePos; }
            set
            {
                SetPosition(value, 0);
                ScrollByteIntoView();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets and sets the number of bytes selected in the hex box.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long SelectionLength
        {
            get { return _selectionLength; }
            set
            {
                SetSelectionLength(value);
                ScrollByteIntoView();
                Invalidate();
            }
        }

        /// <summary>
        /// Contains the size of a single character in pixel
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF CharSize
        {
            get { return _charSize; }
            private set
            {
                if (_charSize == value)
                {
                    return;
                }
                _charSize = value;
                if (CharSizeChanged != null)
                {
                    CharSizeChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the width required for the content
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(0)]
        public int RequiredWidth
        {
            get { return _requiredWidth; }
            private set
            {
                if (_requiredWidth == value)
                {
                    return;
                }
                _requiredWidth = value;
                if (RequiredWidthChanged != null)
                {
                    RequiredWidthChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the number bytes drawn horizontally.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HorizontalByteCount
        {
            get { return _iHexMaxHBytes; }
        }

        /// <summary>
        /// Gets the number bytes drawn vertically.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int VerticalByteCount
        {
            get { return _iHexMaxVBytes; }
        }

        /// <summary>
        /// Gets the current line
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long CurrentLine
        {
            get { return _currentLine; }
        }

        /// <summary>
        /// Gets the current position in the current line
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long CurrentPositionInLine
        {
            get { return _currentPositionInLine; }
        }

        /// <summary>
        /// Gets the a value if insertion mode is active or not.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InsertActive
        {
            get { return _insertActive; }
            set
            {
                if (_insertActive == value)
                {
                    return;
                }

                _insertActive = value;

                // recreate caret
                DestroyCaret();
                CreateCaret();

                // raise change event
                OnInsertActiveChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the built-in context menu.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal HexBoxContextMenu BuiltInContextMenu
        {
            get { return _builtInContextMenu; }
        }

        /// <summary>
        /// Gets or sets the converter that will translate between byte and character values.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IByteCharConverter ByteCharConverter
        {
            get { return _byteCharConverter ?? (_byteCharConverter = new DefaultByteCharConverter()); }
            set
            {
                if (value == null || value == _byteCharConverter)
                {
                    return;
                }
                _byteCharConverter = value;
                Invalidate();
            }
        }

        #endregion

        #region �����������

        /// <summary>
        /// Initializes a new instance of a HexBox class.
        /// </summary>
        public HexBox()
        {
            _readOnly = DEFAULT_READ_ONLY;
            _useFixedBytesPerLine = DEFAULT_USE_FIXED_BYTES_PER_LINE;
            _groupSeparatorVisible = DEFAULT_GROUP_SEPARATOR_VISIBLE;
            _headerOffsetVisible = DEFAULT_HEADER_OFFSET_VISIBLE;
            _columnAddrVisible = DEFAULT_COLUMN_ADDR_VISIBLE;
            _vScrollBarVisible = DEFAULT_VSCROLL_BAR_VISIBLE;
            _bytePos = -1;
            _bytesPerLine = DEFAULT_BYTE_PES_LINE;
            _selectionBackColor = Color.Blue;
            _selectionForeColor = Color.White;
            _shadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
            _shadowSelectionVisible = true;
            _recBorderBottom = SystemInformation.Border3DSize.Height;
            _recBorderLeft = SystemInformation.Border3DSize.Width;
            _recBorderRight = SystemInformation.Border3DSize.Width;
            _recBorderTop = SystemInformation.Border3DSize.Height;
            _infoForeColor = Color.Gray;
            _groupSize = DEFAULT_GROUP_SIZE;
            _hexStringFormat = "X";
            _thumbTrackTimer = new Timer();
            _borderStyle = BorderStyle.Fixed3D;

            BackColorDisabled = Color.FromName(DEFAULT_DISABLE_COLOR_NAME);
            _vScrollBar = new VScrollBar();
            _vScrollBar.Scroll += ScrollBarOnScroll;

            _builtInContextMenu = new HexBoxContextMenu(this);

            BackColor = Color.White;
            Font = SystemFonts.MessageBoxFont;
            _stringFormat = new StringFormat(StringFormat.GenericTypographic);
            _stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

            ActivateEmptyKeyInterpreter();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            _thumbTrackTimer.Interval = THUMB_TRACK_DELAY;
            _thumbTrackTimer.Tick += ThumbTrackTimerOnTick;

            if (DesignHelper.IsDesigntime)
            {
                // � ������ ��������� ����������� ���������� ����� ������ ��� 
                // ������� ����������� ��������� �������� ����
                var bytes = Rand.GetBuffer(300);
                var provider = new HexBoxBufferProvider(bytes);
                ByteProvider = provider;
            }
        }

        #endregion

        #region ������

        private void UpdateScrollSize()
        {
            // calc scroll bar info
            if (VScrollBarVisible && _byteProvider != null && _byteProvider.Length > 0 && _iHexMaxHBytes != 0)
            {
                long scrollmax = (long)Math.Ceiling((_byteProvider.Length + 1) / (double)_iHexMaxHBytes - _iHexMaxVBytes);
                scrollmax = Math.Max(0, scrollmax);

                long scrollpos = _startByte / _iHexMaxHBytes;

                if (scrollmax < _scrollVmax)
                {
                    /* Data size has been decreased. */
                    if (_scrollVpos == _scrollVmax)
                    {
                        /* Scroll one line up if we at bottom. */
                        PerformScrollLineUp();
                    }
                }

                if (scrollmax == _scrollVmax && scrollpos == _scrollVpos)
                {
                    return;
                }

                _scrollVmin = 0;
                _scrollVmax = scrollmax;
                _scrollVpos = Math.Min(scrollpos, scrollmax);
                UpdateVScroll();
            }
            else if (VScrollBarVisible)
            {
                // disable scroll bar
                _scrollVmin = 0;
                _scrollVmax = 0;
                _scrollVpos = 0;
                UpdateVScroll();
            }
        }

        private void UpdateVScroll()
        {
            int max = ToScrollMax(_scrollVmax);

            if (max > 0)
            {
                _vScrollBar.Minimum = 0;
                _vScrollBar.Maximum = max;
                _vScrollBar.Value = ToScrollPos(_scrollVpos);
                _vScrollBar.Visible = true;
            }
            else
            {
                _vScrollBar.Visible = false;
            }
        }

        private int ToScrollPos(long value)
        {
            int max = 65535;

            if (_scrollVmax < max)
            {
                return (int)value;
            }
            double valperc = value / (double)_scrollVmax * 100;
            int res = (int)Math.Floor(max / (double)100 * valperc);
            res = (int)Math.Max(_scrollVmin, res);
            res = (int)Math.Min(_scrollVmax, res);
            return res;
        }

        private long FromScrollPos(int value)
        {
            int max = 65535;
            if (_scrollVmax < max)
            {
                return value;
            }
            double valperc = value / (double)max * 100;
            long res = (int)Math.Floor(_scrollVmax / (double)100 * valperc);
            return res;
        }

        private int ToScrollMax(long value)
        {
            long max = 65535;
            if (value > max)
            {
                return (int)max;
            }
            return (int)value;
        }

        private void PerformScrollToLine(long pos)
        {
            if (pos < _scrollVmin || pos > _scrollVmax || pos == _scrollVpos)
            {
                return;
            }

            _scrollVpos = pos;

            UpdateVScroll();
            UpdateVisibilityBytes();
            UpdateCaret();
            Invalidate();
        }

        private void PerformScrollLines(int lines)
        {
            long pos;
            if (lines > 0)
            {
                pos = Math.Min(_scrollVmax, _scrollVpos + lines);
            }
            else if (lines < 0)
            {
                pos = Math.Max(_scrollVmin, _scrollVpos + lines);
            }
            else
            {
                return;
            }

            PerformScrollToLine(pos);
        }

        private void PerformScrollLineDown()
        {
            PerformScrollLines(1);
        }

        private void PerformScrollLineUp()
        {
            PerformScrollLines(-1);
        }

        private void PerformScrollPageDown()
        {
            PerformScrollLines(_iHexMaxVBytes);
        }

        private void PerformScrollPageUp()
        {
            PerformScrollLines(-_iHexMaxVBytes);
        }

        private void PerformScrollThumpPosition(long pos)
        {
            // Bug fix: Scroll to end, do not scroll to end
            int difference = (_scrollVmax > 65535) ? 10 : 9;

            if (ToScrollPos(pos) == ToScrollMax(_scrollVmax) - difference)
            {
                pos = _scrollVmax;
            }
            // End Bug fix

            PerformScrollToLine(pos);
        }

        /// <summary>
        /// Scrolls the selection start byte into view
        /// </summary>
        public void ScrollByteIntoView()
        {
            ScrollByteIntoView(_bytePos);
        }

        /// <summary>
        /// Scrolls the specific byte into view
        /// </summary>
        /// <param name="index">the index of the byte</param>
        public void ScrollByteIntoView(long index)
        {
            if (_byteProvider == null || _keyInterpreter == null)
            {
                return;
            }

            if (index < _startByte)
            {
                long line = (long)Math.Floor(index / (double)_iHexMaxHBytes);
                PerformScrollThumpPosition(line);
            }
            else if (index > _endByte)
            {
                long line = (long)Math.Floor(index / (double)_iHexMaxHBytes);
                line -= _iHexMaxVBytes - 1;
                PerformScrollThumpPosition(line);
            }
        }

        private void ReleaseSelection()
        {
            if (_selectionLength == 0)
            {
                return;
            }
            _selectionLength = 0;
            OnSelectionLengthChanged(EventArgs.Empty);

            if (!_caretVisible)
            {
                CreateCaret();
            }
            else
            {
                UpdateCaret();
            }

            Invalidate();
        }

        /// <summary>
        /// Returns true if Select method could be invoked.
        /// </summary>
        public bool CanSelectAll()
        {
            if (!Enabled)
            {
                return false;
            }
            if (_byteProvider == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Selects all bytes.
        /// </summary>
        public void SelectAll()
        {
            if (ByteProvider == null)
            {
                return;
            }
            Select(0, ByteProvider.Length);
        }

        /// <summary>
        /// Selects the hex box.
        /// </summary>
        /// <param name="start">the start index of the selection</param>
        /// <param name="length">the length of the selection</param>
        public void Select(long start, long length)
        {
            if (ByteProvider == null)
            {
                return;
            }
            if (!Enabled)
            {
                return;
            }

            InternalSelect(start, length);
            ScrollByteIntoView();
        }

        private void InternalSelect(long start, long length)
        {
            long pos = start;
            long sel = length;
            int cp = 0;

            if (sel > 0 && _caretVisible)
            {
                DestroyCaret();
            }
            else if (sel == 0 && !_caretVisible)
            {
                CreateCaret();
            }

            SetPosition(pos, cp);
            SetSelectionLength(sel);

            UpdateCaret();
            Invalidate();
        }

        private void ActivateEmptyKeyInterpreter()
        {
            if (_eki == null)
            {
                _eki = new EmptyKeyInterpreter(this);
            }

            if (_eki == _keyInterpreter)
            {
                return;
            }

            if (_keyInterpreter != null)
            {
                _keyInterpreter.Deactivate();
            }

            _keyInterpreter = _eki;
            _keyInterpreter.Activate();
        }

        private void ActivateKeyInterpreter()
        {
            if (_ki == null)
            {
                _ki = new KeyInterpreter(this);
            }

            if (_ki == _keyInterpreter)
            {
                return;
            }

            if (_keyInterpreter != null)
            {
                _keyInterpreter.Deactivate();
            }

            _keyInterpreter = _ki;
            _keyInterpreter.Activate();
        }

        private void ActivateStringKeyInterpreter()
        {
            if (_ski == null)
            {
                _ski = new StringKeyInterpreter(this);
            }

            if (_ski == _keyInterpreter)
            {
                return;
            }

            if (_keyInterpreter != null)
            {
                _keyInterpreter.Deactivate();
            }

            _keyInterpreter = _ski;
            _keyInterpreter.Activate();
        }

        private void CreateCaret()
        {
            if (_byteProvider == null || _keyInterpreter == null || _caretVisible || !Focused)
            {
                return;
            }

            // define the caret width depending on InsertActive mode
            int caretWidth = (InsertActive) ? 1 : (int)_charSize.Width;
            int caretHeight = (int)_charSize.Height;
            NativeMethods.CreateCaret(Handle, IntPtr.Zero, caretWidth, caretHeight);

            UpdateCaret();

            NativeMethods.ShowCaret(Handle);

            _caretVisible = true;
        }

        private void UpdateCaret()
        {
            if (_byteProvider == null || _keyInterpreter == null)
            {
                return;
            }

            long byteIndex = _bytePos - _startByte;
            PointF p = _keyInterpreter.GetCaretPointF(byteIndex);
            p.X += _byteCharacterPos * _charSize.Width;
            NativeMethods.SetCaretPos((int)p.X, (int)p.Y);
        }

        private void DestroyCaret()
        {
            if (!_caretVisible)
            {
                return;
            }

            NativeMethods.DestroyCaret();
            _caretVisible = false;
        }

        private void SetCaretPosition(Point p)
        {
            if (_byteProvider == null || _keyInterpreter == null)
            {
                return;
            }

            if (_recHex.Contains(p))
            {
                BytePositionInfo bpi = GetHexBytePositionInfo(p);
                long pos = bpi.Index;
                int cp = bpi.CharacterPosition;

                SetPosition(pos, cp);

                ActivateKeyInterpreter();
                UpdateCaret();
                Invalidate();
            }
            else if (_recStringView.Contains(p))
            {
                BytePositionInfo bpi = GetStringBytePositionInfo(p);
                long pos = bpi.Index;
                int cp = bpi.CharacterPosition;

                SetPosition(pos, cp);

                ActivateStringKeyInterpreter();
                UpdateCaret();
                Invalidate();
            }
        }

        private BytePositionInfo GetHexBytePositionInfo(Point p)
        {
            float x = ((p.X - _recHex.X) / _charSize.Width);
            float y = ((p.Y - _recHex.Y) / _charSize.Height);
            int iX = (int)x;
            int iY = (int)y;

            int hPos = (iX / 3 + 1);

            long bytePos = Math.Min(_byteProvider.Length,
                _startByte + (_iHexMaxHBytes * (iY + 1) - _iHexMaxHBytes) + hPos - 1);
            int byteCharaterPos = (iX % 3);
            if (byteCharaterPos > 1)
            {
                byteCharaterPos = 1;
            }

            if (bytePos == _byteProvider.Length)
            {
                byteCharaterPos = 0;
            }

            if (bytePos < 0)
            {
                return new BytePositionInfo(0, 0);
            }
            return new BytePositionInfo(bytePos, byteCharaterPos);
        }

        private BytePositionInfo GetStringBytePositionInfo(Point p)
        {
            float x = ((p.X - _recStringView.X) / _charSize.Width);
            float y = ((p.Y - _recStringView.Y) / _charSize.Height);
            int iX = (int)x;
            int iY = (int)y;

            int hPos = iX + 1;

            long bytePos = Math.Min(_byteProvider.Length, _startByte + (_iHexMaxHBytes * (iY + 1) - _iHexMaxHBytes) + hPos - 1);
            int byteCharacterPos = 0;

            return bytePos < 0
                ? new BytePositionInfo(0, 0)
                : new BytePositionInfo(bytePos, byteCharacterPos);
        }

        /// <summary>
        /// Searches the current ByteProvider
        /// </summary>
        /// <param name="options">contains all find options</param>
        /// <returns>
        /// the SelectionStart property value if find was successfull or
        /// -1 if there is no match
        /// -2 if Find was aborted.
        /// </returns>
        public long Find(FindOptions options)
        {
            var startIndex = SelectionStart + SelectionLength;
            int match = 0;

            byte[] buffer1 = null;
            byte[] buffer2 = null;
            if (options.Type == FindType.Text && options.MatchCase)
            {
                if (options.FindBuffer == null || options.FindBuffer.Length == 0)
                {
                    throw new ArgumentException("FindBuffer can not be null when Type: Text and MatchCase: false");
                }
                buffer1 = options.FindBuffer;
            }
            else if (options.Type == FindType.Text && !options.MatchCase)
            {
                if (options.FindBufferLowerCase == null || options.FindBufferLowerCase.Length == 0)
                {
                    throw new ArgumentException("FindBufferLowerCase can not be null when Type is Text and MatchCase is true");
                }
                if (options.FindBufferUpperCase == null || options.FindBufferUpperCase.Length == 0)
                {
                    throw new ArgumentException("FindBufferUpperCase can not be null when Type is Text and MatchCase is true");
                }
                if (options.FindBufferLowerCase.Length != options.FindBufferUpperCase.Length)
                {
                    throw new ArgumentException("FindBufferUpperCase and FindBufferUpperCase must have the same size when Type is Text and MatchCase is true");
                }
                buffer1 = options.FindBufferLowerCase;
                buffer2 = options.FindBufferUpperCase;
            }
            else if (options.Type == FindType.Hex)
            {
                if (options.Hex == null || options.Hex.Length == 0)
                {
                    throw new ArgumentException("Hex can not be null when Type is Hex");
                }
                buffer1 = options.Hex;
            }

            int buffer1Length = buffer1.Length;

            _abortFind = false;

            for (long pos = startIndex; pos < _byteProvider.Length; pos++)
            {
                if (_abortFind)
                {
                    return -2;
                }

                if (pos % 1000 == 0) // for performance reasons: DoEvents only 1 times per 1000 loops
                {
                    Application.DoEvents();
                }

                byte compareByte = _byteProvider.ReadByte(pos);
                bool buffer1Match = compareByte == buffer1[match];
                bool hasBuffer2 = buffer2 != null;
                bool buffer2Match = hasBuffer2 && compareByte == buffer2[match];
                bool isMatch = buffer1Match || buffer2Match;
                if (!isMatch)
                {
                    pos -= match;
                    match = 0;
                    _findingPos = pos;
                    continue;
                }

                match++;

                if (match == buffer1Length)
                {
                    long bytePos = pos - buffer1Length + 1;
                    Select(bytePos, buffer1Length);
                    ScrollByteIntoView(_bytePos + _selectionLength);
                    ScrollByteIntoView(_bytePos);

                    return bytePos;
                }
            }

            return -1;
        }

        /// <summary>
        /// Aborts a working Find method.
        /// </summary>
        public void AbortFind()
        {
            _abortFind = true;
        }

        private byte[] GetCopyData()
        {
            if (!CanCopy())
            {
                return new byte[0];
            }

            // put bytes into buffer
            byte[] buffer = new byte[_selectionLength];
            int id = -1;
            for (long i = _bytePos; i < _bytePos + _selectionLength; i++)
            {
                id++;

                buffer[id] = _byteProvider.ReadByte(i);
            }
            return buffer;
        }

        /// <summary>
        /// Copies the current selection in the hex box to the Clipboard.
        /// </summary>
        public void Copy()
        {
            if (!CanCopy())
            {
                return;
            }

            // put bytes into buffer
            byte[] buffer = GetCopyData();

            DataObject da = new DataObject();

            // set string buffer clipbard data
            string sBuffer = System.Text.Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            da.SetData(typeof(string), sBuffer);

            //set memorystream (BinaryData) clipboard data
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, true);
            da.SetData("BinaryData", ms);

            Clipboard.SetDataObject(da, true);
            UpdateCaret();
            ScrollByteIntoView();
            Invalidate();

            OnCopied(EventArgs.Empty);
        }

        /// <summary>
        /// Return true if Copy method could be invoked.
        /// </summary>
        public bool CanCopy()
        {
            if (_selectionLength < 1 || _byteProvider == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves the current selection in the hex box to the Clipboard.
        /// </summary>
        public void Cut()
        {
            if (!CanCut())
            {
                return;
            }

            Copy();

            _byteProvider.DeleteBytes(_bytePos, _selectionLength);
            _byteCharacterPos = 0;
            UpdateCaret();
            ScrollByteIntoView();
            ReleaseSelection();
            Invalidate();
            Refresh();
        }

        /// <summary>
        /// Return true if Cut method could be invoked.
        /// </summary>
        public bool CanCut()
        {
            if (ReadOnly || !Enabled)
            {
                return false;
            }
            if (_byteProvider == null)
            {
                return false;
            }
            if (_selectionLength < 1 || !_byteProvider.IsCanDeleteBytes)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Replaces the current selection in the hex box with the contents of the Clipboard.
        /// </summary>
        public void Paste()
        {
            if (!CanPaste())
            {
                return;
            }

            if (_selectionLength > 0)
            {
                _byteProvider.DeleteBytes(_bytePos, _selectionLength);
            }

            byte[] buffer;
            var da = Clipboard.GetDataObject();
            if (da == null)
            {
                return;
            }

            if (da.GetDataPresent("BinaryData"))
            {
                System.IO.MemoryStream ms = (System.IO.MemoryStream)da.GetData("BinaryData");
                buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
            }
            else if (da.GetDataPresent(typeof(string)))
            {
                string sBuffer = (string)da.GetData(typeof(string));
                buffer = System.Text.Encoding.ASCII.GetBytes(sBuffer);
            }
            else
            {
                return;
            }

            _byteProvider.InsertBytes(_bytePos, buffer);

            SetPosition(_bytePos + buffer.Length, 0);

            ReleaseSelection();
            ScrollByteIntoView();
            UpdateCaret();
            Invalidate();
        }

        /// <summary>
        /// Return true if Paste method could be invoked.
        /// </summary>
        public bool CanPaste()
        {
            if (ReadOnly || !Enabled)
            {
                return false;
            }

            if (_byteProvider == null || !_byteProvider.IsCanInsertBytes)
            {
                return false;
            }

            if (!_byteProvider.IsCanDeleteBytes && _selectionLength > 0)
            {
                return false;
            }

            var da = Clipboard.GetDataObject();
            if (da == null)
            {
                return false;
            }
            if (da.GetDataPresent("BinaryData"))
            {
                return true;
            }
            if (da.GetDataPresent(typeof(string)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if PasteHex method could be invoked.
        /// </summary>
        public bool CanPasteHex()
        {
            if (!CanPaste())
            {
                return false;
            }

            var da = Clipboard.GetDataObject();
            if (da != null && da.GetDataPresent(typeof(string)))
            {
                string hexString = (string)da.GetData(typeof(string));
                byte[] buffer = ConvertHexToBytes(hexString);
                return (buffer != null);
            }

            return false;
        }

        /// <summary>
        /// Replaces the current selection in the hex box with the hex string data of the Clipboard.
        /// </summary>
        public void PasteHex()
        {
            if (!CanPaste())
            {
                return;
            }

            byte[] buffer;
            var da = Clipboard.GetDataObject();
            if (da != null && da.GetDataPresent(typeof(string)))
            {
                string hexString = (string)da.GetData(typeof(string));
                buffer = ConvertHexToBytes(hexString);
                if (buffer == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            if (_selectionLength > 0)
            {
                _byteProvider.DeleteBytes(_bytePos, _selectionLength);
            }

            _byteProvider.InsertBytes(_bytePos, buffer);

            SetPosition(_bytePos + buffer.Length, 0);

            ReleaseSelection();
            ScrollByteIntoView();
            UpdateCaret();
            Invalidate();
        }

        /// <summary>
        /// Copies the current selection in the hex box to the Clipboard in hex format.
        /// </summary>
        public void CopyHex()
        {
            if (!CanCopy())
            {
                return;
            }

            // put bytes into buffer
            byte[] buffer = GetCopyData();

            DataObject da = new DataObject();

            // set string buffer clipbard data
            string hexString = ConvertBytesToHex(buffer);
            da.SetData(typeof(string), hexString);

            //set memorystream (BinaryData) clipboard data
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, true);
            da.SetData("BinaryData", ms);

            Clipboard.SetDataObject(da, true);
            UpdateCaret();
            ScrollByteIntoView();
            Invalidate();

            OnCopiedHex(EventArgs.Empty);
        }

        private void PaintLineInfo(Graphics g, long startByte, long endByte)
        {
            // Ensure endByte isn't > length of array.
            endByte = Math.Min(_byteProvider.Length - 1, endByte);

            Color lineInfoColor = (InfoForeColor != Color.Empty) ? InfoForeColor : ForeColor;
            Brush brush = new SolidBrush(lineInfoColor);

            int maxLine = GetGridBytePoint(endByte - startByte).Y + 1;

            for (int i = 0; i < maxLine; i++)
            {
                long firstLineByte = (startByte + (_iHexMaxHBytes) * i) + _lineInfoOffset;

                PointF bytePointF = GetBytePointF(new Point(0, 0 + i));
                string info = firstLineByte.ToString(_hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);
                int nulls = 8 - info.Length;
                string formattedInfo;
                if (nulls > -1)
                {
                    formattedInfo = new string('0', 8 - info.Length) + info;
                }
                else
                {
                    formattedInfo = new string('~', 8);
                }

                g.DrawString(formattedInfo, Font, brush, new PointF(_recLineInfo.X, bytePointF.Y), _stringFormat);
            }
        }

        private void PaintHeaderRow(Graphics g)
        {
            Brush brush = new SolidBrush(InfoForeColor);
            for (int col = 0; col < _iHexMaxHBytes; col++)
            {
                PaintColumnInfo(g, (byte)col, brush, col);
            }
        }

        private void PaintColumnSeparator(Graphics g)
        {
            for (int col = GroupSize; col < _iHexMaxHBytes; col += GroupSize)
            {
                var pen = new Pen(new SolidBrush(InfoForeColor), 1);
                PointF headerPointF = GetColumnInfoPointF(col);
                headerPointF.X -= _charSize.Width / 2;
                g.DrawLine(pen, headerPointF, new PointF(headerPointF.X, headerPointF.Y + _recColumnInfo.Height + _recHex.Height));
                if (ColumnAsciiVisible)
                {
                    PointF byteStringPointF = GetByteStringPointF(new Point(col, 0));
                    headerPointF.X -= 2;
                    g.DrawLine(pen, new PointF(byteStringPointF.X, byteStringPointF.Y), new PointF(byteStringPointF.X, byteStringPointF.Y + _recHex.Height));
                }
            }
        }

        private void PaintHex(Graphics g, long startByte, long endByte)
        {
            Brush brush = new SolidBrush(GetDefaultForeColor());
            Brush selBrush = new SolidBrush(_selectionForeColor);
            Brush selBrushBack = new SolidBrush(_selectionBackColor);

            int counter = -1;
            long internEndByte = Math.Min(_byteProvider.Length - 1, endByte + _iHexMaxHBytes);

            bool isKeyInterpreterActive = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(KeyInterpreter);

            for (long i = startByte; i < internEndByte + 1; i++)
            {
                counter++;
                Point gridPoint = GetGridBytePoint(counter);
                byte b = _byteProvider.ReadByte(i);

                bool isSelectedByte = i >= _bytePos && i <= (_bytePos + _selectionLength - 1) && _selectionLength != 0;

                if (isSelectedByte && isKeyInterpreterActive)
                {
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint);
                }
                else
                {
                    PaintHexString(g, b, brush, gridPoint);
                }
            }
        }

        private void PaintHexString(Graphics g, byte b, Brush brush, Point gridPoint)
        {
            PointF bytePointF = GetBytePointF(gridPoint);

            string sB = ConvertByteToHex(b);

            g.DrawString(sB.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
            bytePointF.X += _charSize.Width;
            g.DrawString(sB.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
        }

        private void PaintColumnInfo(Graphics g, byte b, Brush brush, int col)
        {
            PointF headerPointF = GetColumnInfoPointF(col);

            string sB = ConvertByteToHex(b);

            g.DrawString(sB.Substring(0, 1), Font, brush, headerPointF, _stringFormat);
            headerPointF.X += _charSize.Width;
            g.DrawString(sB.Substring(1, 1), Font, brush, headerPointF, _stringFormat);
        }

        private void PaintHexStringSelected(Graphics g, byte b, Brush brush, Brush brushBack, Point gridPoint)
        {
            string sB = b.ToString(_hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (sB.Length == 1)
            {
                sB = "0" + sB;
            }

            PointF bytePointF = GetBytePointF(gridPoint);

            bool isLastLineChar = (gridPoint.X + 1 == _iHexMaxHBytes);
            float bcWidth = (isLastLineChar) ? _charSize.Width * 2 : _charSize.Width * 3;

            g.FillRectangle(brushBack, bytePointF.X, bytePointF.Y, bcWidth, _charSize.Height);
            g.DrawString(sB.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
            bytePointF.X += _charSize.Width;
            g.DrawString(sB.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
        }

        private void PaintHexAndStringView(Graphics g, long startByte, long endByte)
        {
            Brush brush = new SolidBrush(GetDefaultForeColor());
            Brush selBrush = new SolidBrush(_selectionForeColor);
            Brush selBrushBack = new SolidBrush(_selectionBackColor);

            int counter = -1;
            long internEndByte = Math.Min(_byteProvider.Length - 1, endByte + _iHexMaxHBytes);

            bool isKeyInterpreterActive = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(KeyInterpreter);
            bool isStringKeyInterpreterActive = _keyInterpreter != null && _keyInterpreter.GetType() == typeof(StringKeyInterpreter);

            for (long i = startByte; i < internEndByte + 1; i++)
            {
                counter++;
                Point gridPoint = GetGridBytePoint(counter);
                PointF byteStringPointF = GetByteStringPointF(gridPoint);
                byte b = _byteProvider.ReadByte(i);

                bool isSelectedByte = i >= _bytePos && i <= (_bytePos + _selectionLength - 1) && _selectionLength != 0;

                if (isSelectedByte && isKeyInterpreterActive)
                {
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint);
                }
                else
                {
                    PaintHexString(g, b, brush, gridPoint);
                }

                string s = new String(ByteCharConverter.ToChar(b), 1);

                if (isSelectedByte && isStringKeyInterpreterActive)
                {
                    g.FillRectangle(selBrushBack, byteStringPointF.X, byteStringPointF.Y, _charSize.Width, _charSize.Height);
                    g.DrawString(s, Font, selBrush, byteStringPointF, _stringFormat);
                }
                else
                {
                    g.DrawString(s, Font, brush, byteStringPointF, _stringFormat);
                }
            }
        }

        private void PaintCurrentBytesSign(Graphics g)
        {
            if (_keyInterpreter != null && _bytePos != -1 && Enabled)
            {
                if (_keyInterpreter.GetType() == typeof(KeyInterpreter))
                {
                    if (_selectionLength == 0)
                    {
                        Point gp = GetGridBytePoint(_bytePos - _startByte);
                        PointF pf = GetByteStringPointF(gp);
                        Size s = new Size((int)_charSize.Width, (int)_charSize.Height);
                        Rectangle r = new Rectangle((int)pf.X, (int)pf.Y, s.Width, s.Height);
                        if (r.IntersectsWith(_recStringView))
                        {
                            r.Intersect(_recStringView);
                            PaintCurrentByteSign(g, r);
                        }
                    }
                    else
                    {
                        int lineWidth = (int)(_recStringView.Width - _charSize.Width);

                        Point startSelGridPoint = GetGridBytePoint(_bytePos - _startByte);
                        PointF startSelPointF = GetByteStringPointF(startSelGridPoint);

                        Point endSelGridPoint = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1);
                        PointF endSelPointF = GetByteStringPointF(endSelGridPoint);

                        int multiLine = endSelGridPoint.Y - startSelGridPoint.Y;
                        if (multiLine == 0)
                        {
                            Rectangle singleLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(endSelPointF.X - startSelPointF.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (singleLine.IntersectsWith(_recStringView))
                            {
                                singleLine.Intersect(_recStringView);
                                PaintCurrentByteSign(g, singleLine);
                            }
                        }
                        else
                        {
                            Rectangle firstLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(_recStringView.X + lineWidth - startSelPointF.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (firstLine.IntersectsWith(_recStringView))
                            {
                                firstLine.Intersect(_recStringView);
                                PaintCurrentByteSign(g, firstLine);
                            }

                            if (multiLine > 1)
                            {
                                Rectangle betweenLines = new Rectangle(
                                    _recStringView.X,
                                    (int)(startSelPointF.Y + _charSize.Height),
                                    _recStringView.Width,
                                    (int)(_charSize.Height * (multiLine - 1)));
                                if (betweenLines.IntersectsWith(_recStringView))
                                {
                                    betweenLines.Intersect(_recStringView);
                                    PaintCurrentByteSign(g, betweenLines);
                                }
                            }

                            Rectangle lastLine = new Rectangle(
                                _recStringView.X,
                                (int)endSelPointF.Y,
                                (int)(endSelPointF.X - _recStringView.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (lastLine.IntersectsWith(_recStringView))
                            {
                                lastLine.Intersect(_recStringView);
                                PaintCurrentByteSign(g, lastLine);
                            }
                        }
                    }
                }
                else
                {
                    if (_selectionLength == 0)
                    {
                        Point gp = GetGridBytePoint(_bytePos - _startByte);
                        PointF pf = GetBytePointF(gp);
                        Size s = new Size((int)_charSize.Width * 2, (int)_charSize.Height);
                        Rectangle r = new Rectangle((int)pf.X, (int)pf.Y, s.Width, s.Height);
                        PaintCurrentByteSign(g, r);
                    }
                    else
                    {
                        int lineWidth = (int)(_recHex.Width - _charSize.Width * 5);

                        Point startSelGridPoint = GetGridBytePoint(_bytePos - _startByte);
                        PointF startSelPointF = GetBytePointF(startSelGridPoint);

                        Point endSelGridPoint = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1);
                        PointF endSelPointF = GetBytePointF(endSelGridPoint);

                        int multiLine = endSelGridPoint.Y - startSelGridPoint.Y;
                        if (multiLine == 0)
                        {
                            Rectangle singleLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(endSelPointF.X - startSelPointF.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (singleLine.IntersectsWith(_recHex))
                            {
                                singleLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, singleLine);
                            }
                        }
                        else
                        {
                            Rectangle firstLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(_recHex.X + lineWidth - startSelPointF.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (firstLine.IntersectsWith(_recHex))
                            {
                                firstLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, firstLine);
                            }

                            if (multiLine > 1)
                            {
                                Rectangle betweenLines = new Rectangle(
                                    _recHex.X,
                                    (int)(startSelPointF.Y + _charSize.Height),
                                    (int)(lineWidth + _charSize.Width * 2),
                                    (int)(_charSize.Height * (multiLine - 1)));
                                if (betweenLines.IntersectsWith(_recHex))
                                {
                                    betweenLines.Intersect(_recHex);
                                    PaintCurrentByteSign(g, betweenLines);
                                }
                            }

                            Rectangle lastLine = new Rectangle(
                                _recHex.X,
                                (int)endSelPointF.Y,
                                (int)(endSelPointF.X - _recHex.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (lastLine.IntersectsWith(_recHex))
                            {
                                lastLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, lastLine);
                            }
                        }
                    }
                }
            }
        }

        private void PaintCurrentByteSign(Graphics g, Rectangle rec)
        {
            // stack overflowexception on big files - workaround
            if (rec.Top < 0 || rec.Left < 0 || rec.Width <= 0 || rec.Height <= 0)
            {
                return;
            }

            Bitmap myBitmap = new Bitmap(rec.Width, rec.Height);
            Graphics bitmapGraphics = Graphics.FromImage(myBitmap);

            SolidBrush greenBrush = new SolidBrush(_shadowSelectionColor);

            bitmapGraphics.FillRectangle(greenBrush, 0,
                0, rec.Width, rec.Height);

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;

            g.DrawImage(myBitmap, rec.Left, rec.Top);
        }

        private Color GetDefaultForeColor()
        {
            if (Enabled)
            {
                return ForeColor;
            }
            return Color.Gray;
        }

        private void UpdateVisibilityBytes()
        {
            if (_byteProvider == null || _byteProvider.Length == 0)
            {
                return;
            }

            _startByte = (_scrollVpos + 1) * _iHexMaxHBytes - _iHexMaxHBytes;
            _endByte = Math.Min(_byteProvider.Length - 1, _startByte + _iHexMaxBytes);
        }

        private void UpdateRectanglePositioning()
        {
            // calc char size
            SizeF charSize;
            using (CreateGraphics())
            {
                charSize = CreateGraphics().MeasureString("A", Font, 100, _stringFormat);
            }
            CharSize = new SizeF((float)Math.Ceiling(charSize.Width), (float)Math.Ceiling(charSize.Height));

            int requiredWidth = 0;

            // calc content bounds
            _recContent = ClientRectangle;
            _recContent.X += _recBorderLeft;
            _recContent.Y += _recBorderTop;
            _recContent.Width -= _recBorderRight + _recBorderLeft;
            _recContent.Height -= _recBorderBottom + _recBorderTop;

            if (_vScrollBarVisible)
            {
                _recContent.Width -= _vScrollBar.Width;
                _vScrollBar.Left = _recContent.X + _recContent.Width;
                _vScrollBar.Top = _recContent.Y;
                _vScrollBar.Height = _recContent.Height;
                requiredWidth += _vScrollBar.Width;
            }

            int marginLeft = 4;

            // calc line info bounds
            if (_columnAddrVisible)
            {
                _recLineInfo = new Rectangle(_recContent.X + marginLeft,
                    _recContent.Y,
                    (int)(_charSize.Width * 10),
                    _recContent.Height);
                requiredWidth += _recLineInfo.Width;
            }
            else
            {
                _recLineInfo = Rectangle.Empty;
                _recLineInfo.X = marginLeft;
                requiredWidth += marginLeft;
            }

            // calc line info bounds
            _recColumnInfo = new Rectangle(_recLineInfo.X + _recLineInfo.Width, _recContent.Y, _recContent.Width - _recLineInfo.Width, (int)charSize.Height + 4);
            if (_headerOffsetVisible)
            {
                _recLineInfo.Y += (int)charSize.Height + 4;
                _recLineInfo.Height -= (int)charSize.Height + 4;
            }
            else
            {
                _recColumnInfo.Height = 0;
            }

            // calc hex bounds and grid
            _recHex = new Rectangle(_recLineInfo.X + _recLineInfo.Width,
                _recLineInfo.Y,
                _recContent.Width - _recLineInfo.Width,
                _recContent.Height - _recColumnInfo.Height);

            if (UseFixedBytesPerLine)
            {
                SetHorizontalByteCount(_bytesPerLine);
                _recHex.Width = (int)Math.Floor(((double)_iHexMaxHBytes) * _charSize.Width * 3 + (2 * _charSize.Width));
                requiredWidth += _recHex.Width;
            }
            else
            {
                int hmax = (int)Math.Floor(_recHex.Width / (double)_charSize.Width);
                if (_columnAsciiVisible)
                {
                    hmax -= 2;
                    if (hmax > 1)
                    {
                        SetHorizontalByteCount((int)Math.Floor((double)hmax / 4));
                    }
                    else
                    {
                        SetHorizontalByteCount(1);
                    }
                }
                else
                {
                    if (hmax > 1)
                    {
                        SetHorizontalByteCount((int)Math.Floor((double)hmax / 3));
                    }
                    else
                    {
                        SetHorizontalByteCount(1);
                    }
                }
                _recHex.Width = (int)Math.Floor(((double)_iHexMaxHBytes) * _charSize.Width * 3 + (2 * _charSize.Width));
                requiredWidth += _recHex.Width;
            }

            if (_columnAsciiVisible)
            {
                _recStringView = new Rectangle(_recHex.X + _recHex.Width,
                    _recHex.Y,
                    (int)(_charSize.Width * _iHexMaxHBytes),
                    _recHex.Height);
                requiredWidth += _recStringView.Width;
            }
            else
            {
                _recStringView = Rectangle.Empty;
            }

            RequiredWidth = requiredWidth;

            int vmax = (int)Math.Floor(_recHex.Height / (double)_charSize.Height);
            SetVerticalByteCount(vmax);

            _iHexMaxBytes = _iHexMaxHBytes * _iHexMaxVBytes;

            UpdateScrollSize();
        }

        private PointF GetBytePointF(long byteIndex)
        {
            Point gp = GetGridBytePoint(byteIndex);

            return GetBytePointF(gp);
        }

        private PointF GetBytePointF(Point gp)
        {
            float x = (3 * _charSize.Width) * gp.X + _recHex.X;
            float y = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recHex.Y;

            return new PointF(x, y);
        }

        private PointF GetColumnInfoPointF(int col)
        {
            Point gp = GetGridBytePoint(col);
            float x = (3 * _charSize.Width) * gp.X + _recColumnInfo.X;
            float y = _recColumnInfo.Y;

            return new PointF(x, y);
        }

        private PointF GetByteStringPointF(Point gp)
        {
            float x = (_charSize.Width) * gp.X + _recStringView.X;
            float y = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recStringView.Y;

            return new PointF(x, y);
        }

        private Point GetGridBytePoint(long byteIndex)
        {
            int row = (int)Math.Floor(byteIndex / (double)_iHexMaxHBytes);
            int column = (int)(byteIndex + _iHexMaxHBytes - _iHexMaxHBytes * (row + 1));

            Point res = new Point(column, row);
            return res;
        }

        /// <summary>
        /// Converts a byte array to a hex string. For example: {10,11} = "0A 0B"
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <returns>the hex string</returns>
        private string ConvertBytesToHex(IEnumerable<byte> data)
        {
            var sb = new StringBuilder();
            foreach (byte b in data)
            {
                string hex = ConvertByteToHex(b);
                sb.Append(hex);
                sb.Append(" ");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            string result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Converts the byte to a hex string. For example: "10" = "0A";
        /// </summary>
        /// <param name="b">the byte to format</param>
        /// <returns>the hex string</returns>
        private string ConvertByteToHex(byte b)
        {
            string sB = b.ToString(_hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (sB.Length == 1)
            {
                sB = "0" + sB;
            }
            return sB;
        }

        /// <summary>
        /// Converts the hex string to an byte array. The hex string must be separated by a space char ' '. If there is any invalid
        /// hex information in the string the result will be null.
        /// </summary>
        /// <param name="hex">the hex string separated by ' '. For example: "0A 0B 0C"</param>
        /// <returns>the byte array. null if hex is invalid or empty</returns>
        private byte[] ConvertHexToBytes(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return null;
            }
            hex = hex.Trim();
            var hexArray = hex.Split(' ');
            var byteArray = new byte[hexArray.Length];

            for (int i = 0; i < hexArray.Length; i++)
            {
                var hexValue = hexArray[i];

                byte b;
                var isByte = ConvertHexToByte(hexValue, out b);
                if (!isByte)
                {
                    return null;
                }
                byteArray[i] = b;
            }

            return byteArray;
        }

        private bool ConvertHexToByte(string hex, out byte b)
        {
            bool isByte = byte.TryParse(hex, NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out b);
            return isByte;
        }

        private void SetPosition(long bytePos)
        {
            SetPosition(bytePos, _byteCharacterPos);
        }

        private void SetPosition(long bytePos, int byteCharacterPos)
        {
            _byteCharacterPos = byteCharacterPos;

            if (bytePos != _bytePos)
            {
                _bytePos = bytePos;
                CheckCurrentLineChanged();
                CheckCurrentPositionInLineChanged();

                OnSelectionStartChanged(EventArgs.Empty);
            }
        }

        private void SetSelectionLength(long selectionLength)
        {
            if (selectionLength != _selectionLength)
            {
                _selectionLength = selectionLength;
                OnSelectionLengthChanged(EventArgs.Empty);
            }
        }

        private void SetHorizontalByteCount(int value)
        {
            if (_iHexMaxHBytes == value)
            {
                return;
            }

            _iHexMaxHBytes = value;
            OnHorizontalByteCountChanged(EventArgs.Empty);
        }

        private void SetVerticalByteCount(int value)
        {
            if (_iHexMaxVBytes == value)
            {
                return;
            }

            _iHexMaxVBytes = value;
            OnVerticalByteCountChanged(EventArgs.Empty);
        }

        private void CheckCurrentLineChanged()
        {
            long currentLine = (long)Math.Floor(_bytePos / (double)_iHexMaxHBytes) + 1;

            if (_byteProvider == null && _currentLine != 0)
            {
                _currentLine = 0;
                OnCurrentLineChanged(EventArgs.Empty);
            }
            else if (currentLine != _currentLine)
            {
                _currentLine = currentLine;
                OnCurrentLineChanged(EventArgs.Empty);
            }
        }

        private void CheckCurrentPositionInLineChanged()
        {
            Point gb = GetGridBytePoint(_bytePos);
            int currentPositionInLine = gb.X + 1;

            if (_byteProvider == null && _currentPositionInLine != 0)
            {
                _currentPositionInLine = 0;
                OnCurrentPositionInLineChanged(EventArgs.Empty);
            }
            else if (currentPositionInLine != _currentPositionInLine)
            {
                _currentPositionInLine = currentPositionInLine;
                OnCurrentPositionInLineChanged(EventArgs.Empty);
            }
        }

        private void _byteProvider_LengthChanged(object sender, EventArgs e)
        {
            UpdateScrollSize();
        }

        /// <summary>
        /// For high resolution screen support
        /// </summary>
        /// <param name="factor">the factor</param>
        /// <param name="specified">bounds</param>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            BeginInvoke(new MethodInvoker(() =>
            {
                UpdateRectanglePositioning();
                if (_caretVisible)
                {
                    DestroyCaret();
                    CreateCaret();
                }
                Invalidate();
            }));
        }

        #endregion

        #region ��������� �����: EmptyKeyInterpreter

        /// <summary>
        /// Represents an empty input handler without any functionality.
        /// If is set ByteProvider to null, then this interpreter is used.
        /// </summary>
        private class EmptyKeyInterpreter : IKeyInterpreter
        {
            #region ����

            private readonly HexBox _hexBox;

            #endregion

            #region �����������

            public EmptyKeyInterpreter(HexBox hexBox)
            {
                _hexBox = hexBox;
            }

            #endregion

            #region ������

            public void Activate()
            {
            }

            public void Deactivate()
            {
            }

            public bool PreProcessWmKeyUp(ref Message m)
            {
                return _hexBox.BasePreProcessMessage(ref m);
            }

            public bool PreProcessWmChar(ref Message m)
            {
                return _hexBox.BasePreProcessMessage(ref m);
            }

            public bool PreProcessWmKeyDown(ref Message m)
            {
                return _hexBox.BasePreProcessMessage(ref m);
            }

            public PointF GetCaretPointF(long byteIndex)
            {
                return new PointF();
            }

            #endregion
        }

        #endregion

        #region ��������� �����: IKeyInterpreter

        /// <summary>
        /// Defines a user input handler such as for mouse and keyboard input
        /// </summary>
        private interface IKeyInterpreter
        {
            #region ������

            /// <summary>
            /// Activates mouse events
            /// </summary>
            void Activate();

            /// <summary>
            /// Deactivate mouse events
            /// </summary>
            void Deactivate();

            /// <summary>
            /// Preprocesses WM_KEYUP window message.
            /// </summary>
            /// <param name="m">the Message object to process.</param>
            /// <returns>True, if the message was processed.</returns>
            bool PreProcessWmKeyUp(ref Message m);

            /// <summary>
            /// Preprocesses WM_CHAR window message.
            /// </summary>
            /// <param name="m">the Message object to process.</param>
            /// <returns>True, if the message was processed.</returns>
            bool PreProcessWmChar(ref Message m);

            /// <summary>
            /// Preprocesses WM_KEYDOWN window message.
            /// </summary>
            /// <param name="m">the Message object to process.</param>
            /// <returns>True, if the message was processed.</returns>
            bool PreProcessWmKeyDown(ref Message m);

            /// <summary>
            /// Gives some information about where to place the caret.
            /// </summary>
            /// <param name="byteIndex">the index of the byte</param>
            /// <returns>the position where the caret is to place.</returns>
            PointF GetCaretPointF(long byteIndex);

            #endregion
        }

        #endregion

        #region ��������� �����: KeyInterpreter

        /// <summary>
        /// Handles user input such as mouse and keyboard input during hex view edit
        /// </summary>
        private class KeyInterpreter : IKeyInterpreter
        {
            #region ��������

            /// <summary>
            /// Delegate for key-down processing.
            /// </summary>
            /// <param name="m">the message object contains key data information</param>
            /// <returns>True, if the message was processed</returns>
            private delegate bool MessageDelegate(ref Message m);

            #endregion

            #region ����

            /// <summary>
            /// Contains the parent HexBox control
            /// </summary>
            protected readonly HexBox _hexBox;

            /// <summary>
            /// Contains the current mouse selection position info
            /// </summary>
            private BytePositionInfo _bpi;

            /// <summary>
            /// Contains the selection start position info
            /// </summary>
            private BytePositionInfo _bpiStart;

            /// <summary>
            /// Contains all message handlers of key interpreter key down message
            /// </summary>
            private Dictionary<Keys, MessageDelegate> _messageHandlers;

            /// <summary>
            /// Contains True, if mouse is down
            /// </summary>
            private bool _mouseDown;

            /// <summary>
            /// Contains True, if shift key is down
            /// </summary>
            private bool _shiftDown;

            #endregion

            #region ��������

            private Dictionary<Keys, MessageDelegate> MessageHandlers
            {
                get
                {
                    if (_messageHandlers == null)
                    {
                        _messageHandlers = new Dictionary<Keys, MessageDelegate>();
                        _messageHandlers.Add(Keys.Left, PreProcessWmKeyDown_Left); // move left
                        _messageHandlers.Add(Keys.Up, PreProcessWmKeyDown_Up); // move up
                        _messageHandlers.Add(Keys.Right, PreProcessWmKeyDown_Right); // move right
                        _messageHandlers.Add(Keys.Down, PreProcessWmKeyDown_Down); // move down
                        _messageHandlers.Add(Keys.PageUp, PreProcessWmKeyDown_PageUp); // move pageup
                        _messageHandlers.Add(Keys.PageDown, PreProcessWmKeyDown_PageDown); // move page down
                        _messageHandlers.Add(Keys.Left | Keys.Shift, PreProcessWmKeyDown_ShiftLeft); // move left with selection
                        _messageHandlers.Add(Keys.Up | Keys.Shift, PreProcessWmKeyDown_ShiftUp); // move up with selection
                        _messageHandlers.Add(Keys.Right | Keys.Shift, PreProcessWmKeyDown_ShiftRight); // move right with selection
                        _messageHandlers.Add(Keys.Down | Keys.Shift, PreProcessWmKeyDown_ShiftDown); // move down with selection
                        _messageHandlers.Add(Keys.Tab, PreProcessWmKeyDown_Tab); // switch to string view
                        _messageHandlers.Add(Keys.Back, PreProcessWmKeyDown_Back); // back
                        _messageHandlers.Add(Keys.Delete, PreProcessWmKeyDown_Delete); // delete
                        _messageHandlers.Add(Keys.Home, PreProcessWmKeyDown_Home); // move to home
                        _messageHandlers.Add(Keys.End, PreProcessWmKeyDown_End); // move to end
                        _messageHandlers.Add(Keys.ShiftKey | Keys.Shift, PreProcessWmKeyDown_ShiftShiftKey); // begin selection process
                        _messageHandlers.Add(Keys.C | Keys.Control, PreProcessWmKeyDown_ControlC); // copy 
                        _messageHandlers.Add(Keys.X | Keys.Control, PreProcessWmKeyDown_ControlX); // cut
                        _messageHandlers.Add(Keys.V | Keys.Control, PreProcessWmKeyDown_ControlV); // paste
                    }
                    return _messageHandlers;
                }
            }

            #endregion

            #region �����������

            public KeyInterpreter(HexBox hexBox)
            {
                _hexBox = hexBox;
            }

            #endregion

            #region ������

            public virtual void Activate()
            {
                _hexBox.MouseDown += BeginMouseSelection;
                _hexBox.MouseMove += UpdateMouseSelection;
                _hexBox.MouseUp += EndMouseSelection;
            }

            public virtual void Deactivate()
            {
                _hexBox.MouseDown -= BeginMouseSelection;
                _hexBox.MouseMove -= UpdateMouseSelection;
                _hexBox.MouseUp -= EndMouseSelection;
            }

            private void BeginMouseSelection(object sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Left)
                {
                    return;
                }

                _mouseDown = true;

                if (!_shiftDown)
                {
                    _bpiStart = new BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos);
                    _hexBox.ReleaseSelection();
                }
                else
                {
                    UpdateMouseSelection(this, e);
                }
            }

            private void UpdateMouseSelection(object sender, MouseEventArgs e)
            {
                if (!_mouseDown)
                {
                    return;
                }

                _bpi = GetBytePositionInfo(new Point(e.X, e.Y));
                long selEnd = _bpi.Index;
                long realselStart;
                long realselLength;

                if (selEnd < _bpiStart.Index)
                {
                    realselStart = selEnd;
                    realselLength = _bpiStart.Index - selEnd;
                }
                else if (selEnd > _bpiStart.Index)
                {
                    realselStart = _bpiStart.Index;
                    realselLength = selEnd - realselStart;
                }
                else
                {
                    realselStart = _hexBox._bytePos;
                    realselLength = 0;
                }

                if (realselStart != _hexBox._bytePos || realselLength != _hexBox._selectionLength)
                {
                    _hexBox.InternalSelect(realselStart, realselLength);
                    _hexBox.ScrollByteIntoView(_bpi.Index);
                }
            }

            private void EndMouseSelection(object sender, MouseEventArgs e)
            {
                _mouseDown = false;
            }

            public virtual bool PreProcessWmKeyDown(ref Message m)
            {
                Keys vc = (Keys)m.WParam.ToInt32();

                Keys keyData = vc | ModifierKeys;

                // detect whether key down event should be raised
                var hasMessageHandler = MessageHandlers.ContainsKey(keyData);
                if (hasMessageHandler && RaiseKeyDown(keyData))
                {
                    return true;
                }

                var messageHandler = hasMessageHandler
                    ? MessageHandlers[keyData]
                    : PreProcessWmKeyDown_Default;

                return messageHandler(ref m);
            }

            private bool PreProcessWmKeyDown_Default(ref Message m)
            {
                _hexBox.ScrollByteIntoView();
                return _hexBox.BasePreProcessMessage(ref m);
            }

            protected bool RaiseKeyDown(Keys keyData)
            {
                KeyEventArgs e = new KeyEventArgs(keyData);
                _hexBox.OnKeyDown(e);
                return e.Handled;
            }

            protected virtual bool PreProcessWmKeyDown_Left(ref Message m)
            {
                return PerformPosMoveLeft();
            }

            protected virtual bool PreProcessWmKeyDown_Up(ref Message m)
            {
                long pos = _hexBox._bytePos;
                int cp = _hexBox._byteCharacterPos;

                if (!(pos == 0 && cp == 0))
                {
                    pos = Math.Max(-1, pos - _hexBox._iHexMaxHBytes);
                    if (pos == -1)
                    {
                        return true;
                    }

                    _hexBox.SetPosition(pos);

                    if (pos < _hexBox._startByte)
                    {
                        _hexBox.PerformScrollLineUp();
                    }

                    _hexBox.UpdateCaret();
                    _hexBox.Invalidate();
                }

                _hexBox.ScrollByteIntoView();
                _hexBox.ReleaseSelection();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Right(ref Message m)
            {
                return PerformPosMoveRight();
            }

            protected virtual bool PreProcessWmKeyDown_Down(ref Message m)
            {
                long pos = _hexBox._bytePos;
                int cp = _hexBox._byteCharacterPos;

                if (pos == _hexBox._byteProvider.Length && cp == 0)
                {
                    return true;
                }

                pos = Math.Min(_hexBox._byteProvider.Length, pos + _hexBox._iHexMaxHBytes);

                if (pos == _hexBox._byteProvider.Length)
                {
                    cp = 0;
                }

                _hexBox.SetPosition(pos, cp);

                if (pos > _hexBox._endByte - 1)
                {
                    _hexBox.PerformScrollLineDown();
                }

                _hexBox.UpdateCaret();
                _hexBox.ScrollByteIntoView();
                _hexBox.ReleaseSelection();
                _hexBox.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_PageUp(ref Message m)
            {
                long pos = _hexBox._bytePos;
                int cp = _hexBox._byteCharacterPos;

                if (pos == 0 && cp == 0)
                {
                    return true;
                }

                pos = Math.Max(0, pos - _hexBox._iHexMaxBytes);
                if (pos == 0)
                {
                    return true;
                }

                _hexBox.SetPosition(pos);

                if (pos < _hexBox._startByte)
                {
                    _hexBox.PerformScrollPageUp();
                }

                _hexBox.ReleaseSelection();
                _hexBox.UpdateCaret();
                _hexBox.Invalidate();
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_PageDown(ref Message m)
            {
                long pos = _hexBox._bytePos;
                int cp = _hexBox._byteCharacterPos;

                if (pos == _hexBox._byteProvider.Length && cp == 0)
                {
                    return true;
                }

                pos = Math.Min(_hexBox._byteProvider.Length, pos + _hexBox._iHexMaxBytes);

                if (pos == _hexBox._byteProvider.Length)
                {
                    cp = 0;
                }

                _hexBox.SetPosition(pos, cp);

                if (pos > _hexBox._endByte - 1)
                {
                    _hexBox.PerformScrollPageDown();
                }

                _hexBox.ReleaseSelection();
                _hexBox.UpdateCaret();
                _hexBox.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftLeft(ref Message m)
            {
                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;

                if (pos + sel < 1)
                {
                    return true;
                }

                if (pos + sel <= _bpiStart.Index)
                {
                    if (pos == 0)
                    {
                        return true;
                    }

                    pos--;
                    sel++;
                }
                else
                {
                    sel = Math.Max(0, sel - 1);
                }

                _hexBox.ScrollByteIntoView();
                _hexBox.InternalSelect(pos, sel);

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftUp(ref Message m)
            {
                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;

                if (pos - _hexBox._iHexMaxHBytes < 0 && pos <= _bpiStart.Index)
                {
                    return true;
                }

                if (_bpiStart.Index >= pos + sel)
                {
                    pos = pos - _hexBox._iHexMaxHBytes;
                    sel += _hexBox._iHexMaxHBytes;
                    _hexBox.InternalSelect(pos, sel);
                    _hexBox.ScrollByteIntoView();
                }
                else
                {
                    sel -= _hexBox._iHexMaxHBytes;
                    if (sel < 0)
                    {
                        pos = _bpiStart.Index + sel;
                        sel = -sel;
                        _hexBox.InternalSelect(pos, sel);
                        _hexBox.ScrollByteIntoView();
                    }
                    else
                    {
                        sel -= _hexBox._iHexMaxHBytes;
                        _hexBox.InternalSelect(pos, sel);
                        _hexBox.ScrollByteIntoView(pos + sel);
                    }
                }

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftRight(ref Message m)
            {
                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;

                if (pos + sel >= _hexBox._byteProvider.Length)
                {
                    return true;
                }

                if (_bpiStart.Index <= pos)
                {
                    sel++;
                    _hexBox.InternalSelect(pos, sel);
                    _hexBox.ScrollByteIntoView(pos + sel);
                }
                else
                {
                    pos++;
                    sel = Math.Max(0, sel - 1);
                    _hexBox.InternalSelect(pos, sel);
                    _hexBox.ScrollByteIntoView();
                }

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftDown(ref Message m)
            {
                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;

                long max = _hexBox._byteProvider.Length;

                if (pos + sel + _hexBox._iHexMaxHBytes > max)
                {
                    return true;
                }

                if (_bpiStart.Index <= pos)
                {
                    sel += _hexBox._iHexMaxHBytes;
                    _hexBox.InternalSelect(pos, sel);
                    _hexBox.ScrollByteIntoView(pos + sel);
                }
                else
                {
                    sel -= _hexBox._iHexMaxHBytes;
                    if (sel < 0)
                    {
                        pos = _bpiStart.Index;
                        sel = -sel;
                    }
                    else
                    {
                        pos += _hexBox._iHexMaxHBytes;
                        //sel -= _hexBox._iHexMaxHBytes;
                    }

                    _hexBox.InternalSelect(pos, sel);
                    _hexBox.ScrollByteIntoView();
                }

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Tab(ref Message m)
            {
                if (_hexBox._columnAsciiVisible && _hexBox._keyInterpreter.GetType() == typeof(KeyInterpreter))
                {
                    _hexBox.ActivateStringKeyInterpreter();
                    _hexBox.ScrollByteIntoView();
                    _hexBox.ReleaseSelection();
                    _hexBox.UpdateCaret();
                    _hexBox.Invalidate();
                    return true;
                }

                if (_hexBox.Parent == null)
                {
                    return true;
                }
                _hexBox.Parent.SelectNextControl(_hexBox, true, true, true, true);
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftTab(ref Message m)
            {
                if (_hexBox._keyInterpreter is StringKeyInterpreter)
                {
                    _shiftDown = false;
                    _hexBox.ActivateKeyInterpreter();
                    _hexBox.ScrollByteIntoView();
                    _hexBox.ReleaseSelection();
                    _hexBox.UpdateCaret();
                    _hexBox.Invalidate();
                    return true;
                }

                if (_hexBox.Parent == null)
                {
                    return true;
                }
                _hexBox.Parent.SelectNextControl(_hexBox, false, true, true, true);
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Back(ref Message m)
            {
                if (!_hexBox._byteProvider.IsCanDeleteBytes)
                {
                    return true;
                }

                if (_hexBox.ReadOnly)
                {
                    return true;
                }

                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;
                int cp = _hexBox._byteCharacterPos;

                long startDelete = (cp == 0 && sel == 0) ? pos - 1 : pos;
                if (startDelete < 0 && sel < 1)
                {
                    return true;
                }

                long bytesToDelete = (sel > 0) ? sel : 1;
                _hexBox._byteProvider.DeleteBytes(Math.Max(0, startDelete), bytesToDelete);
                _hexBox.UpdateScrollSize();

                if (sel == 0)
                {
                    PerformPosMoveLeftByte();
                }

                _hexBox.ReleaseSelection();
                _hexBox.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Delete(ref Message m)
            {
                if (!_hexBox._byteProvider.IsCanDeleteBytes)
                {
                    return true;
                }

                if (_hexBox.ReadOnly)
                {
                    return true;
                }

                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;

                if (pos >= _hexBox._byteProvider.Length)
                {
                    return true;
                }

                long bytesToDelete = (sel > 0) ? sel : 1;
                _hexBox._byteProvider.DeleteBytes(pos, bytesToDelete);

                _hexBox.UpdateScrollSize();
                _hexBox.ReleaseSelection();
                _hexBox.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Home(ref Message m)
            {
                long pos = _hexBox._bytePos;

                if (pos < 1)
                {
                    return true;
                }

                pos = 0;
                int cp = 0;
                _hexBox.SetPosition(pos, cp);

                _hexBox.ScrollByteIntoView();
                _hexBox.UpdateCaret();
                _hexBox.ReleaseSelection();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_End(ref Message m)
            {
                long pos = _hexBox._bytePos;

                if (pos >= _hexBox._byteProvider.Length - 1)
                {
                    return true;
                }

                pos = _hexBox._byteProvider.Length;
                int cp = 0;
                _hexBox.SetPosition(pos, cp);

                _hexBox.ScrollByteIntoView();
                _hexBox.UpdateCaret();
                _hexBox.ReleaseSelection();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftShiftKey(ref Message m)
            {
                if (_mouseDown)
                {
                    return true;
                }
                if (_shiftDown)
                {
                    return true;
                }

                _shiftDown = true;

                if (_hexBox._selectionLength > 0)
                {
                    return true;
                }

                _bpiStart = new BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos);

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ControlC(ref Message m)
            {
                _hexBox.Copy();
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ControlX(ref Message m)
            {
                _hexBox.Cut();
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ControlV(ref Message m)
            {
                _hexBox.Paste();
                return true;
            }

            public virtual bool PreProcessWmChar(ref Message m)
            {
                if (ModifierKeys == Keys.Control)
                {
                    return _hexBox.BasePreProcessMessage(ref m);
                }

                bool sw = _hexBox._byteProvider.IsCanWriteByte;
                bool si = _hexBox._byteProvider.IsCanInsertBytes;
                bool sd = _hexBox._byteProvider.IsCanDeleteBytes;

                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;
                int cp = _hexBox._byteCharacterPos;

                if (
                    (!sw && pos != _hexBox._byteProvider.Length) ||
                    (!si && pos == _hexBox._byteProvider.Length))
                {
                    return _hexBox.BasePreProcessMessage(ref m);
                }

                char c = (char)m.WParam.ToInt32();

                if (Uri.IsHexDigit(c))
                {
                    if (RaiseKeyPress(c))
                    {
                        return true;
                    }

                    if (_hexBox.ReadOnly)
                    {
                        return true;
                    }

                    bool isInsertMode = (pos == _hexBox._byteProvider.Length);

                    // do insert when insertActive = true
                    if (!isInsertMode && si && _hexBox.InsertActive && cp == 0)
                    {
                        isInsertMode = true;
                    }

                    if (sd && si && sel > 0)
                    {
                        _hexBox._byteProvider.DeleteBytes(pos, sel);
                        isInsertMode = true;
                        cp = 0;
                        _hexBox.SetPosition(pos, cp);
                    }

                    _hexBox.ReleaseSelection();

                    byte currentByte = (byte)(isInsertMode ? 0 : _hexBox._byteProvider.ReadByte(pos));

                    string sCb = currentByte.ToString("X", System.Threading.Thread.CurrentThread.CurrentCulture);
                    if (sCb.Length == 1)
                    {
                        sCb = "0" + sCb;
                    }

                    string sNewCb = c.ToString(CultureInfo.InvariantCulture);
                    if (cp == 0)
                    {
                        sNewCb += sCb.Substring(1, 1);
                    }
                    else
                    {
                        sNewCb = sCb.Substring(0, 1) + sNewCb;
                    }
                    byte newcb = byte.Parse(sNewCb, NumberStyles.AllowHexSpecifier, System.Threading.Thread.CurrentThread.CurrentCulture);

                    if (isInsertMode)
                    {
                        _hexBox._byteProvider.InsertBytes(pos, new[] { newcb });
                    }
                    else
                    {
                        _hexBox._byteProvider.WriteByte(pos, newcb);
                    }

                    PerformPosMoveRight();

                    _hexBox.Invalidate();
                    return true;
                }
                return _hexBox.BasePreProcessMessage(ref m);
            }

            protected bool RaiseKeyPress(char keyChar)
            {
                KeyPressEventArgs e = new KeyPressEventArgs(keyChar);
                _hexBox.OnKeyPress(e);
                return e.Handled;
            }

            public virtual bool PreProcessWmKeyUp(ref Message m)
            {
                Keys vc = (Keys)m.WParam.ToInt32();

                Keys keyData = vc | ModifierKeys;

                switch (keyData)
                {
                    case Keys.ShiftKey:
                    case Keys.Insert:
                        if (RaiseKeyUp(keyData))
                        {
                            return true;
                        }
                        break;
                }

                switch (keyData)
                {
                    case Keys.ShiftKey:
                        _shiftDown = false;
                        return true;
                    case Keys.Insert:
                        return PreProcessWmKeyUp_Insert(ref m);
                    default:
                        return _hexBox.BasePreProcessMessage(ref m);
                }
            }

            protected virtual bool PreProcessWmKeyUp_Insert(ref Message m)
            {
                _hexBox.InsertActive = !_hexBox.InsertActive;
                return true;
            }

            private bool RaiseKeyUp(Keys keyData)
            {
                KeyEventArgs e = new KeyEventArgs(keyData);
                _hexBox.OnKeyUp(e);
                return e.Handled;
            }

            protected virtual bool PerformPosMoveLeft()
            {
                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;
                int cp = _hexBox._byteCharacterPos;

                if (sel != 0)
                {
                    cp = 0;
                    _hexBox.SetPosition(pos, cp);
                    _hexBox.ReleaseSelection();
                }
                else
                {
                    if (pos == 0 && cp == 0)
                    {
                        return true;
                    }

                    if (cp > 0)
                    {
                        cp--;
                    }
                    else
                    {
                        pos = Math.Max(0, pos - 1);
                        cp++;
                    }

                    _hexBox.SetPosition(pos, cp);

                    if (pos < _hexBox._startByte)
                    {
                        _hexBox.PerformScrollLineUp();
                    }
                    _hexBox.UpdateCaret();
                    _hexBox.Invalidate();
                }

                _hexBox.ScrollByteIntoView();
                return true;
            }

            protected virtual bool PerformPosMoveRight()
            {
                long pos = _hexBox._bytePos;
                int cp = _hexBox._byteCharacterPos;
                long sel = _hexBox._selectionLength;

                if (sel != 0)
                {
                    pos += sel;
                    cp = 0;
                    _hexBox.SetPosition(pos, cp);
                    _hexBox.ReleaseSelection();
                }
                else
                {
                    if (!(pos == _hexBox._byteProvider.Length && cp == 0))
                    {
                        if (cp > 0)
                        {
                            pos = Math.Min(_hexBox._byteProvider.Length, pos + 1);
                            cp = 0;
                        }
                        else
                        {
                            cp++;
                        }

                        _hexBox.SetPosition(pos, cp);

                        if (pos > _hexBox._endByte - 1)
                        {
                            _hexBox.PerformScrollLineDown();
                        }
                        _hexBox.UpdateCaret();
                        _hexBox.Invalidate();
                    }
                }

                _hexBox.ScrollByteIntoView();
                return true;
            }

            protected virtual bool PerformPosMoveLeftByte()
            {
                long pos = _hexBox._bytePos;

                if (pos == 0)
                {
                    return true;
                }

                pos = Math.Max(0, pos - 1);
                int cp = 0;

                _hexBox.SetPosition(pos, cp);

                if (pos < _hexBox._startByte)
                {
                    _hexBox.PerformScrollLineUp();
                }
                _hexBox.UpdateCaret();
                _hexBox.ScrollByteIntoView();
                _hexBox.Invalidate();

                return true;
            }

            protected virtual bool PerformPosMoveRightByte()
            {
                long pos = _hexBox._bytePos;

                if (pos == _hexBox._byteProvider.Length)
                {
                    return true;
                }

                pos = Math.Min(_hexBox._byteProvider.Length, pos + 1);
                int cp = 0;

                _hexBox.SetPosition(pos, cp);

                if (pos > _hexBox._endByte - 1)
                {
                    _hexBox.PerformScrollLineDown();
                }
                _hexBox.UpdateCaret();
                _hexBox.ScrollByteIntoView();
                _hexBox.Invalidate();

                return true;
            }

            public virtual PointF GetCaretPointF(long byteIndex)
            {
                return _hexBox.GetBytePointF(byteIndex);
            }

            protected virtual BytePositionInfo GetBytePositionInfo(Point p)
            {
                return _hexBox.GetHexBytePositionInfo(p);
            }

            #endregion
        }

        #endregion

        #region ��������� �����: StringKeyInterpreter

        /// <summary>
        /// Handles user input such as mouse and keyboard input during string view edit
        /// </summary>
        private class StringKeyInterpreter : KeyInterpreter
        {
            #region �����������

            public StringKeyInterpreter(HexBox hexBox)
                : base(hexBox)
            {
                _hexBox._byteCharacterPos = 0;
            }

            #endregion

            #region ������

            public override bool PreProcessWmKeyDown(ref Message m)
            {
                Keys vc = (Keys)m.WParam.ToInt32();

                Keys keyData = vc | ModifierKeys;

                switch (keyData)
                {
                    case Keys.Tab | Keys.Shift:
                    case Keys.Tab:
                        if (RaiseKeyDown(keyData))
                        {
                            return true;
                        }
                        break;
                }

                switch (keyData)
                {
                    case Keys.Tab | Keys.Shift:
                        return PreProcessWmKeyDown_ShiftTab(ref m);
                    case Keys.Tab:
                        return PreProcessWmKeyDown_Tab(ref m);
                    default:
                        return base.PreProcessWmKeyDown(ref m);
                }
            }

            protected override bool PreProcessWmKeyDown_Left(ref Message m)
            {
                return PerformPosMoveLeftByte();
            }

            protected override bool PreProcessWmKeyDown_Right(ref Message m)
            {
                return PerformPosMoveRightByte();
            }

            public override bool PreProcessWmChar(ref Message m)
            {
                if (ModifierKeys == Keys.Control)
                {
                    return _hexBox.BasePreProcessMessage(ref m);
                }

                bool sw = _hexBox._byteProvider.IsCanWriteByte;
                bool si = _hexBox._byteProvider.IsCanInsertBytes;
                bool sd = _hexBox._byteProvider.IsCanDeleteBytes;

                long pos = _hexBox._bytePos;
                long sel = _hexBox._selectionLength;

                if (
                    (!sw && pos != _hexBox._byteProvider.Length) ||
                    (!si && pos == _hexBox._byteProvider.Length))
                {
                    return _hexBox.BasePreProcessMessage(ref m);
                }

                char c = (char)m.WParam.ToInt32();

                if (RaiseKeyPress(c))
                {
                    return true;
                }

                if (_hexBox.ReadOnly)
                {
                    return true;
                }

                bool isInsertMode = (pos == _hexBox._byteProvider.Length);

                // do insert when insertActive = true
                if (!isInsertMode && si && _hexBox.InsertActive)
                {
                    isInsertMode = true;
                }

                if (sd && si && sel > 0)
                {
                    _hexBox._byteProvider.DeleteBytes(pos, sel);
                    isInsertMode = true;
                    int cp = 0;
                    _hexBox.SetPosition(pos, cp);
                }

                _hexBox.ReleaseSelection();

                byte b = _hexBox.ByteCharConverter.ToByte(c);
                if (isInsertMode)
                {
                    _hexBox._byteProvider.InsertBytes(pos, new[] { b });
                }
                else
                {
                    _hexBox._byteProvider.WriteByte(pos, b);
                }

                PerformPosMoveRightByte();
                _hexBox.Invalidate();

                return true;
            }

            public override PointF GetCaretPointF(long byteIndex)
            {
                Point gp = _hexBox.GetGridBytePoint(byteIndex);
                return _hexBox.GetByteStringPointF(gp);
            }

            protected override BytePositionInfo GetBytePositionInfo(Point p)
            {
                return _hexBox.GetStringBytePositionInfo(p);
            }

            #endregion
        }

        #endregion
    }
}