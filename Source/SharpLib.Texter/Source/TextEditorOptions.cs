using System;
using System.ComponentModel;
using System.Reflection;

namespace SharpLib.Texter
{
    [Serializable]
    public class TextEditorOptions : INotifyPropertyChanged
    {
        #region Поля

        private bool _allowScrollBelowDocument;

        private bool _allowToggleOverstrikeMode;

        private int _columnRulerPosition;

        private bool _convertTabsToSpaces;

        private bool _cutCopyWholeLine;

        private bool _enableEmailHyperlinks;

        private bool _enableHyperlinks;

        private bool _enableImeSupport;

        private bool _enableRectangularSelection;

        private bool _enableTextDragDrop;

        private bool _enableVirtualSpace;

        private bool _hideCursorWhileTyping;

        private bool _highlightCurrentLine;

        private int _indentationSize;

        private bool _inheritWordWrapIndentation;

        private bool _requireControlModifierForHyperlinkClick;

        private bool _showBoxForControlCharacters;

        private bool _showColumnRuler;

        private bool _showEndOfLine;

        private bool _showSpaces;

        private bool _showTabs;

        private double _wordWrapIndentation;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        [DefaultValue(false)]
        public virtual bool ShowSpaces
        {
            get { return _showSpaces; }
            set
            {
                if (_showSpaces != value)
                {
                    _showSpaces = value;
                    OnPropertyChanged("ShowSpaces");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(false)]
        public virtual bool ShowTabs
        {
            get { return _showTabs; }
            set
            {
                if (_showTabs != value)
                {
                    _showTabs = value;
                    OnPropertyChanged("ShowTabs");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(false)]
        public virtual bool ShowEndOfLine
        {
            get { return _showEndOfLine; }
            set
            {
                if (_showEndOfLine != value)
                {
                    _showEndOfLine = value;
                    OnPropertyChanged("ShowEndOfLine");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(true)]
        public virtual bool ShowBoxForControlCharacters
        {
            get { return _showBoxForControlCharacters; }
            set
            {
                if (_showBoxForControlCharacters != value)
                {
                    _showBoxForControlCharacters = value;
                    OnPropertyChanged("ShowBoxForControlCharacters");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(true)]
        public virtual bool EnableHyperlinks
        {
            get { return _enableHyperlinks; }
            set
            {
                if (_enableHyperlinks != value)
                {
                    _enableHyperlinks = value;
                    OnPropertyChanged("EnableHyperlinks");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(true)]
        public virtual bool EnableEmailHyperlinks
        {
            get { return _enableEmailHyperlinks; }
            set
            {
                if (_enableEmailHyperlinks != value)
                {
                    _enableEmailHyperlinks = value;
                    OnPropertyChanged("EnableEMailHyperlinks");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(true)]
        public virtual bool RequireControlModifierForHyperlinkClick
        {
            get { return _requireControlModifierForHyperlinkClick; }
            set
            {
                if (_requireControlModifierForHyperlinkClick != value)
                {
                    _requireControlModifierForHyperlinkClick = value;
                    OnPropertyChanged("RequireControlModifierForHyperlinkClick");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(4)]
        public virtual int IndentationSize
        {
            get { return _indentationSize; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value", value, "value must be positive");
                }

                if (value > 1000)
                {
                    throw new ArgumentOutOfRangeException("value", value, "indentation size is too large");
                }
                if (_indentationSize != value)
                {
                    _indentationSize = value;
                    OnPropertyChanged("IndentationSize");
                    OnPropertyChanged("IndentationString");
                }
            }
        }

        [Category("SharpLib")]
        [DefaultValue(true)]
        public virtual bool ConvertTabsToSpaces
        {
            get { return _convertTabsToSpaces; }
            set
            {
                if (_convertTabsToSpaces != value)
                {
                    _convertTabsToSpaces = value;
                    OnPropertyChanged("ConvertTabsToSpaces");
                    OnPropertyChanged("IndentationString");
                }
            }
        }

        [Browsable(false)]
        public string IndentationString
        {
            get { return GetIndentationString(1); }
        }

        [DefaultValue(true)]
        public virtual bool CutCopyWholeLine
        {
            get { return _cutCopyWholeLine; }
            set
            {
                if (_cutCopyWholeLine != value)
                {
                    _cutCopyWholeLine = value;
                    OnPropertyChanged("CutCopyWholeLine");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public virtual bool AllowScrollBelowDocument
        {
            get { return _allowScrollBelowDocument; }
            set
            {
                if (_allowScrollBelowDocument != value)
                {
                    _allowScrollBelowDocument = value;
                    OnPropertyChanged("AllowScrollBelowDocument");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(0.0)]
        public virtual double WordWrapIndentation
        {
            get { return _wordWrapIndentation; }
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    throw new ArgumentOutOfRangeException("value", value, "value must not be NaN/infinity");
                }
                if (value != _wordWrapIndentation)
                {
                    _wordWrapIndentation = value;
                    OnPropertyChanged("WordWrapIndentation");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(true)]
        public virtual bool InheritWordWrapIndentation
        {
            get { return _inheritWordWrapIndentation; }
            set
            {
                if (value != _inheritWordWrapIndentation)
                {
                    _inheritWordWrapIndentation = value;
                    OnPropertyChanged("InheritWordWrapIndentation");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(true)]
        public bool EnableRectangularSelection
        {
            get { return _enableRectangularSelection; }
            set
            {
                if (_enableRectangularSelection != value)
                {
                    _enableRectangularSelection = value;
                    OnPropertyChanged("EnableRectangularSelection");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(true)]
        public bool EnableTextDragDrop
        {
            get { return _enableTextDragDrop; }
            set
            {
                if (_enableTextDragDrop != value)
                {
                    _enableTextDragDrop = value;
                    OnPropertyChanged("EnableTextDragDrop");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public virtual bool EnableVirtualSpace
        {
            get { return _enableVirtualSpace; }
            set
            {
                if (_enableVirtualSpace != value)
                {
                    _enableVirtualSpace = value;
                    OnPropertyChanged("EnableVirtualSpace");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(true)]
        public virtual bool EnableImeSupport
        {
            get { return _enableImeSupport; }
            set
            {
                if (_enableImeSupport != value)
                {
                    _enableImeSupport = value;
                    OnPropertyChanged("EnableImeSupport");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public virtual bool ShowColumnRuler
        {
            get { return _showColumnRuler; }
            set
            {
                if (_showColumnRuler != value)
                {
                    _showColumnRuler = value;
                    OnPropertyChanged("ShowColumnRuler");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(80)]
        public virtual int ColumnRulerPosition
        {
            get { return _columnRulerPosition; }
            set
            {
                if (_columnRulerPosition != value)
                {
                    _columnRulerPosition = value;
                    OnPropertyChanged("ColumnRulerPosition");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public virtual bool HighlightCurrentLine
        {
            get { return _highlightCurrentLine; }
            set
            {
                if (_highlightCurrentLine != value)
                {
                    _highlightCurrentLine = value;
                    OnPropertyChanged("HighlightCurrentLine");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(true)]
        public bool HideCursorWhileTyping
        {
            get { return _hideCursorWhileTyping; }
            set
            {
                if (_hideCursorWhileTyping != value)
                {
                    _hideCursorWhileTyping = value;
                    OnPropertyChanged("HideCursorWhileTyping");
                }
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public bool AllowToggleOverstrikeMode
        {
            get { return _allowToggleOverstrikeMode; }
            set
            {
                if (_allowToggleOverstrikeMode != value)
                {
                    _allowToggleOverstrikeMode = value;
                    OnPropertyChanged("AllowToggleOverstrikeMode");
                }
            }
        }

        #endregion

        #region События

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Конструктор

        public TextEditorOptions()
        {
            _showBoxForControlCharacters = true;
            _indentationSize = 4;
            _inheritWordWrapIndentation = true;
            _requireControlModifierForHyperlinkClick = true;
            _hideCursorWhileTyping = true;
            _enableImeSupport = true;
            _enableTextDragDrop = true;
            _enableRectangularSelection = true;
            _enableHyperlinks = true;
            _enableEmailHyperlinks = true;
            _cutCopyWholeLine = true;
            _columnRulerPosition = 80;
            _convertTabsToSpaces = true;
        }

        public TextEditorOptions(TextEditorOptions options): this()
        {
            var fields = typeof(TextEditorOptions).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo fi in fields)
            {
                if (!fi.IsNotSerialized)
                {
                    fi.SetValue(this, fi.GetValue(options));
                }
            }
        }

        #endregion

        #region Методы

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public virtual string GetIndentationString(int column)
        {
            if (column < 1)
            {
                throw new ArgumentOutOfRangeException("column", column, "Value must be at least 1.");
            }
            int indentationSize = IndentationSize;
            if (ConvertTabsToSpaces)
            {
                return new string(' ', indentationSize - ((column - 1) % indentationSize));
            }
            return "\t";
        }

        #endregion
    }
}