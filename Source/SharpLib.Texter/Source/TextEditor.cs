using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

using SharpLib.Texter.Document;
using SharpLib.Texter.Editing;
using SharpLib.Texter.Highlighting;
using SharpLib.Texter.Rendering;
using SharpLib.Texter.Search;
using SharpLib.Texter.Utils;
using SharpLib.Wpf;

namespace SharpLib.Texter
{
    [Localizability(LocalizationCategory.Text), ContentProperty("Text")]
    public class TextEditor : Control, ITextEditorComponent, IWeakEventListener
    {
        #region Поля

        public static readonly DependencyProperty DocumentProperty;

        public static readonly DependencyProperty EncodingProperty;

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty;

        public static readonly DependencyProperty IsModifiedProperty;

        public static readonly DependencyProperty IsReadOnlyProperty;

        public static readonly DependencyProperty LineNumbersForegroundProperty;

        public static readonly RoutedEvent MouseHoverEvent;

        public static readonly RoutedEvent MouseHoverStoppedEvent;

        public static readonly DependencyProperty OptionsProperty;

        public static readonly RoutedEvent PreviewMouseHoverEvent;

        public static readonly RoutedEvent PreviewMouseHoverStoppedEvent;

        public static readonly DependencyProperty ShowLineNumbersProperty;

        public static readonly DependencyProperty SyntaxHighlightingProperty;

        public static readonly DependencyProperty SyntaxHighlightingTypProperty;

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty;

        public static readonly DependencyProperty WordWrapProperty;

        private readonly TextArea _textArea;

        private IVisualLineTransformer _colorizer;

        private ScrollViewer _scrollViewer;

        #endregion

        #region Свойства

        [Browsable(false)]
        public TextDocument Document
        {
            get { return (TextDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        [Category("SharpLib")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TextEditorOptions Options
        {
            get { return (TextEditorOptions)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        [Localizability(LocalizationCategory.Text), DefaultValue("")]
        public string Text
        {
            get
            {
                var document = Document;
                return document != null ? document.Text : string.Empty;
            }
            set
            {
                var document = GetDocument();
                document.Text = value ?? string.Empty;

                CaretOffset = 0;
                document.UndoStack.ClearAll();
            }
        }

        [Browsable(false)]
        public TextArea TextArea
        {
            get { return _textArea; }
        }

        internal ScrollViewer ScrollViewer
        {
            get { return _scrollViewer; }
        }

        [Category("SharpLib")]
        public HighlightTyp SyntaxHighlightingTyp
        {
            get { return (HighlightTyp)GetValue(SyntaxHighlightingTypProperty); }
            set { SetValue(SyntaxHighlightingTypProperty, value); }
        }

        [Browsable(false)]
        public IHighlightingDefinition SyntaxHighlighting
        {
            get { return (IHighlightingDefinition)GetValue(SyntaxHighlightingProperty); }
            set { SetValue(SyntaxHighlightingProperty, value); }
        }

        [Category("SharpLib")]
        public bool WordWrap
        {
            get { return (bool)GetValue(WordWrapProperty); }
            set { SetValue(WordWrapProperty, Boxes.Box(value)); }
        }

        [Category("SharpLib")]
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, Boxes.Box(value)); }
        }

        [Category("SharpLib")]
        public bool IsModified
        {
            get { return (bool)GetValue(IsModifiedProperty); }
            set { SetValue(IsModifiedProperty, Boxes.Box(value)); }
        }

        [Category("SharpLib")]
        public bool ShowLineNumbers
        {
            get { return (bool)GetValue(ShowLineNumbersProperty); }
            set { SetValue(ShowLineNumbersProperty, Boxes.Box(value)); }
        }

        [Category("SharpLib")]
        public Brush LineNumbersForeground
        {
            get { return (Brush)GetValue(LineNumbersForegroundProperty); }
            set { SetValue(LineNumbersForegroundProperty, value); }
        }

        [Browsable(false)]
        public bool CanRedo
        {
            get { return CanExecute(ApplicationCommands.Redo); }
        }

        [Browsable(false)]
        public bool CanUndo
        {
            get { return CanExecute(ApplicationCommands.Undo); }
        }

        [Browsable(false)]
        public double ExtentHeight
        {
            get { return _scrollViewer != null ? _scrollViewer.ExtentHeight : 0; }
        }

        [Browsable(false)]
        public double ExtentWidth
        {
            get { return _scrollViewer != null ? _scrollViewer.ExtentWidth : 0; }
        }

        [Browsable(false)]
        public double ViewportHeight
        {
            get { return _scrollViewer != null ? _scrollViewer.ViewportHeight : 0; }
        }

        [Browsable(false)]
        public double ViewportWidth
        {
            get { return _scrollViewer != null ? _scrollViewer.ViewportWidth : 0; }
        }

        [Browsable(false)]
        public double VerticalOffset
        {
            get { return _scrollViewer != null ? _scrollViewer.VerticalOffset : 0; }
        }

        [Browsable(false)]
        public double HorizontalOffset
        {
            get { return _scrollViewer != null ? _scrollViewer.HorizontalOffset : 0; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get
            {
                var textArea = TextArea;

                if (textArea != null && textArea.Document != null && !textArea.Selection.IsEmpty)
                {
                    return textArea.Document.GetText(textArea.Selection.SurroundingSegment);
                }
                return string.Empty;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                var textArea = TextArea;
                if (textArea != null && textArea.Document != null)
                {
                    int offset = SelectionStart;
                    int length = SelectionLength;
                    textArea.Document.Replace(offset, length, value);

                    textArea.Selection = Selection.Create(textArea, offset, offset + value.Length);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CaretOffset
        {
            get
            {
                var textArea = TextArea;
                if (textArea != null)
                {
                    return textArea.Caret.Offset;
                }
                return 0;
            }
            set
            {
                var textArea = TextArea;
                if (textArea != null)
                {
                    textArea.Caret.Offset = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart
        {
            get
            {
                var textArea = TextArea;
                if (textArea != null)
                {
                    if (textArea.Selection.IsEmpty)
                    {
                        return textArea.Caret.Offset;
                    }
                    return textArea.Selection.SurroundingSegment.Offset;
                }
                return 0;
            }
            set { Select(value, SelectionLength); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get
            {
                var textArea = TextArea;
                if (textArea != null && !textArea.Selection.IsEmpty)
                {
                    return textArea.Selection.SurroundingSegment.Length;
                }
                return 0;
            }
            set { Select(SelectionStart, value); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineCount
        {
            get
            {
                var document = Document;
                if (document != null)
                {
                    return document.LineCount;
                }
                return 1;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Encoding Encoding
        {
            get { return (Encoding)GetValue(EncodingProperty); }
            set { SetValue(EncodingProperty, value); }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        #endregion

        #region События

        public event EventHandler DocumentChanged;

        public event MouseEventHandler MouseHover
        {
            add { AddHandler(MouseHoverEvent, value); }
            remove { RemoveHandler(MouseHoverEvent, value); }
        }

        public event MouseEventHandler MouseHoverStopped
        {
            add { AddHandler(MouseHoverStoppedEvent, value); }
            remove { RemoveHandler(MouseHoverStoppedEvent, value); }
        }

        public event PropertyChangedEventHandler OptionChanged;

        public event MouseEventHandler PreviewMouseHover
        {
            add { AddHandler(PreviewMouseHoverEvent, value); }
            remove { RemoveHandler(PreviewMouseHoverEvent, value); }
        }

        public event MouseEventHandler PreviewMouseHoverStopped
        {
            add { AddHandler(PreviewMouseHoverStoppedEvent, value); }
            remove { RemoveHandler(PreviewMouseHoverStoppedEvent, value); }
        }

        public event EventHandler TextChanged;

        #endregion

        #region Конструктор

        static TextEditor()
        {
            DocumentProperty = TextView.DocumentProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(OnDocumentChanged));
            EncodingProperty = DependencyProperty.Register("Encoding", typeof(Encoding), typeof(TextEditor));
            HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible));
            IsModifiedProperty = DependencyProperty.Register("IsModified", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False, OnIsModifiedChanged));
            IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False, OnIsReadOnlyChanged));
            LineNumbersForegroundProperty = DependencyProperty.Register("LineNumbersForeground", typeof(Brush), typeof(TextEditor),
                new FrameworkPropertyMetadata(Brushes.Gray, OnLineNumbersForegroundChanged));
            MouseHoverEvent = TextView.MouseHoverEvent.AddOwner(typeof(TextEditor));
            MouseHoverStoppedEvent = TextView.MouseHoverStoppedEvent.AddOwner(typeof(TextEditor));
            OptionsProperty = TextView.OptionsProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(OnOptionsChanged));
            PreviewMouseHoverEvent = TextView.PreviewMouseHoverEvent.AddOwner(typeof(TextEditor));
            PreviewMouseHoverStoppedEvent = TextView.PreviewMouseHoverStoppedEvent.AddOwner(typeof(TextEditor));
            ShowLineNumbersProperty = DependencyProperty.Register("ShowLineNumbers", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False, OnShowLineNumbersChanged));
            SyntaxHighlightingProperty = DependencyProperty.Register("SyntaxHighlighting", typeof(IHighlightingDefinition), typeof(TextEditor),
                new FrameworkPropertyMetadata(OnSyntaxHighlightingChanged));

            SyntaxHighlightingTypProperty = DependencyProperty.Register("SyntaxHighlightingTyp", typeof(HighlightTyp), typeof(TextEditor), new FrameworkPropertyMetadata(OnSyntaxHighlightingTypChanged));

            VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible));
            WordWrapProperty = DependencyProperty.Register("WordWrap", typeof(bool), typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.False));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(typeof(TextEditor)));
            FocusableProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.True));
        }

        public TextEditor() : this(new TextArea())
        {
            FontFamily = new FontFamily("Consolas");
            FontSize = FontHelper.PointToPixelFontSize("10pt");

            SearchPanel.Install(this);
        }

        protected TextEditor(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            _textArea = textArea;

            textArea.TextView.Services.AddService(typeof(TextEditor), this);

            SetCurrentValue(OptionsProperty, textArea.Options);
            SetCurrentValue(DocumentProperty, new TextDocument());
        }

        #endregion

        #region Методы

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TextEditorAutomationPeer(this);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            if (Equals(e.NewFocus, this))
            {
                Keyboard.Focus(TextArea);
                e.Handled = true;
            }
        }

        protected virtual void OnDocumentChanged(EventArgs e)
        {
            if (DocumentChanged != null)
            {
                DocumentChanged(this, e);
            }
        }

        private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
        }

        private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
        {
            if (oldValue != null)
            {
                TextDocumentWeakEventManager.TextChanged.RemoveListener(oldValue, this);
                PropertyChangedEventManager.RemoveListener(oldValue.UndoStack, this, "IsOriginalFile");
            }
            _textArea.Document = newValue;
            if (newValue != null)
            {
                TextDocumentWeakEventManager.TextChanged.AddListener(newValue, this);
                PropertyChangedEventManager.AddListener(newValue.UndoStack, this, "IsOriginalFile");
            }
            OnDocumentChanged(EventArgs.Empty);
            OnTextChanged(EventArgs.Empty);
        }

        protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
        {
            if (OptionChanged != null)
            {
                OptionChanged(this, e);
            }
        }

        private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)dp).OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
        }

        private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
        {
            if (oldValue != null)
            {
                PropertyChangedWeakEventManager.RemoveListener(oldValue, this);
            }
            _textArea.Options = newValue;
            if (newValue != null)
            {
                PropertyChangedWeakEventManager.AddListener(newValue, this);
            }
            OnOptionChanged(new PropertyChangedEventArgs(null));
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedWeakEventManager))
            {
                OnOptionChanged((PropertyChangedEventArgs)e);
                return true;
            }
            if (managerType == typeof(TextDocumentWeakEventManager.TextChanged))
            {
                OnTextChanged(e);
                return true;
            }
            if (managerType == typeof(PropertyChangedEventManager))
            {
                return HandleIsOriginalChanged((PropertyChangedEventArgs)e);
            }
            return false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ReceiveWeakEvent(managerType, sender, e);
        }

        private TextDocument GetDocument()
        {
            var document = Document;
            if (document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            return document;
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollViewer = (ScrollViewer)Template.FindName("PART_ScrollViewer", this);
        }

        private bool CanExecute(RoutedUICommand command)
        {
            var textArea = TextArea;
            if (textArea == null)
            {
                return false;
            }
            return command.CanExecute(null, textArea);
        }

        private void Execute(RoutedUICommand command)
        {
            var textArea = TextArea;
            if (textArea != null)
            {
                command.Execute(null, textArea);
            }
        }

        private static void OnSyntaxHighlightingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)d).OnSyntaxHighlightingChanged(e.NewValue as IHighlightingDefinition);
        }

        private static void OnSyntaxHighlightingTypChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)d).OnSyntaxHighlightingTypChanged((HighlightTyp)e.NewValue);
        }

        private void OnSyntaxHighlightingTypChanged(HighlightTyp newValue)
        {
            var result = HighlightingManager.Instance.GetDefinition(newValue);

            SyntaxHighlighting = result;
        }

        private void OnSyntaxHighlightingChanged(IHighlightingDefinition newValue)
        {
            if (_colorizer != null)
            {
                TextArea.TextView.LineTransformers.Remove(_colorizer);
                _colorizer = null;
            }
            if (newValue != null)
            {
                _colorizer = CreateColorizer(newValue);
                if (_colorizer != null)
                {
                    TextArea.TextView.LineTransformers.Insert(0, _colorizer);
                }
            }
        }

        protected virtual IVisualLineTransformer CreateColorizer(IHighlightingDefinition highlightingDefinition)
        {
            if (highlightingDefinition == null)
            {
                throw new ArgumentNullException("highlightingDefinition");
            }
            return new HighlightingColorizer(highlightingDefinition);
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as TextEditor;
            if (editor != null)
            {
                if ((bool)e.NewValue)
                {
                    editor.TextArea.ReadOnlySectionProvider = ReadOnlySectionDocument.Instance;
                }
                else
                {
                    editor.TextArea.ReadOnlySectionProvider = NoReadOnlySections.Instance;
                }

                var peer = UIElementAutomationPeer.FromElement(editor) as TextEditorAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseIsReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
                }
            }
        }

        private static void OnIsModifiedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as TextEditor;
            if (editor != null)
            {
                var document = editor.Document;
                if (document != null)
                {
                    var undoStack = document.UndoStack;
                    if ((bool)e.NewValue)
                    {
                        if (undoStack.IsOriginalFile)
                        {
                            undoStack.DiscardOriginalFileMarker();
                        }
                    }
                    else
                    {
                        undoStack.MarkAsOriginalFile();
                    }
                }
            }
        }

        private bool HandleIsOriginalChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOriginalFile")
            {
                var document = Document;
                if (document != null)
                {
                    SetCurrentValue(IsModifiedProperty, Boxes.Box(!document.UndoStack.IsOriginalFile));
                }
                return true;
            }
            return false;
        }

        private static void OnShowLineNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TextEditor)d;
            var leftMargins = editor.TextArea.LeftMargins;
            if ((bool)e.NewValue)
            {
                var lineNumbers = new LineNumberMargin();
                var line = (Line)DottedLineMargin.Create();
                leftMargins.Insert(0, lineNumbers);
                leftMargins.Insert(1, line);
                var lineNumbersForeground = new Binding("LineNumbersForeground")
                {
                    Source = editor
                };
                line.SetBinding(Shape.StrokeProperty, lineNumbersForeground);
                lineNumbers.SetBinding(ForegroundProperty, lineNumbersForeground);
            }
            else
            {
                for (int i = 0; i < leftMargins.Count; i++)
                {
                    if (leftMargins[i] is LineNumberMargin)
                    {
                        leftMargins.RemoveAt(i);
                        if (i < leftMargins.Count && DottedLineMargin.IsDottedLineMargin(leftMargins[i]))
                        {
                            leftMargins.RemoveAt(i);
                        }
                        break;
                    }
                }
            }
        }

        private static void OnLineNumbersForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TextEditor)d;
            var lineNumberMargin = editor.TextArea.LeftMargins.FirstOrDefault(margin => margin is LineNumberMargin) as LineNumberMargin;

            if (lineNumberMargin != null)
            {
                lineNumberMargin.SetValue(ForegroundProperty, e.NewValue);
            }
        }

        public void AppendText(string textData)
        {
            var document = GetDocument();
            document.Insert(document.TextLength, textData);
        }

        public void BeginChange()
        {
            GetDocument().BeginUpdate();
        }

        public void Copy()
        {
            Execute(ApplicationCommands.Copy);
        }

        public void Cut()
        {
            Execute(ApplicationCommands.Cut);
        }

        public IDisposable DeclareChangeBlock()
        {
            return GetDocument().RunUpdate();
        }

        public void EndChange()
        {
            GetDocument().EndUpdate();
        }

        public void LineDown()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.LineDown();
            }
        }

        public void LineLeft()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.LineLeft();
            }
        }

        public void LineRight()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.LineRight();
            }
        }

        public void LineUp()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.LineUp();
            }
        }

        public void PageDown()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.PageDown();
            }
        }

        public void PageUp()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.PageUp();
            }
        }

        public void PageLeft()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.PageLeft();
            }
        }

        public void PageRight()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.PageRight();
            }
        }

        public void Paste()
        {
            Execute(ApplicationCommands.Paste);
        }

        public bool Redo()
        {
            if (CanExecute(ApplicationCommands.Redo))
            {
                Execute(ApplicationCommands.Redo);
                return true;
            }
            return false;
        }

        public void ScrollToEnd()
        {
            ApplyTemplate();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToEnd();
            }
        }

        public void ScrollToHome()
        {
            ApplyTemplate();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToHome();
            }
        }

        public void ScrollToHorizontalOffset(double offset)
        {
            ApplyTemplate();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToHorizontalOffset(offset);
            }
        }

        public void ScrollToVerticalOffset(double offset)
        {
            ApplyTemplate();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        public void SelectAll()
        {
            Execute(ApplicationCommands.SelectAll);
        }

        public bool Undo()
        {
            if (CanExecute(ApplicationCommands.Undo))
            {
                Execute(ApplicationCommands.Undo);
                return true;
            }
            return false;
        }

        public void Select(int start, int length)
        {
            int documentLength = Document != null ? Document.TextLength : 0;
            if (start < 0 || start > documentLength)
            {
                throw new ArgumentOutOfRangeException("start", start, "Value must be between 0 and " + documentLength);
            }
            if (length < 0 || start + length > documentLength)
            {
                throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (documentLength - start));
            }
            _textArea.Selection = Selection.Create(_textArea, start, start + length);
            _textArea.Caret.Offset = start + length;
        }

        public void Clear()
        {
            Text = string.Empty;
        }

        public void Load(Stream stream)
        {
            using (var reader = FileReader.OpenStream(stream, Encoding ?? Encoding.UTF8))
            {
                Text = reader.ReadToEnd();
                SetCurrentValue(EncodingProperty, reader.CurrentEncoding);
            }
            SetCurrentValue(IsModifiedProperty, Boxes.False);
        }

        public void Load(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Load(fs);
            }
        }

        public void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var encoding = Encoding;
            var document = Document;
            var writer = encoding != null ? new StreamWriter(stream, encoding) : new StreamWriter(stream);
            if (document != null)
            {
                document.WriteTextTo(writer);
            }
            writer.Flush();

            SetCurrentValue(IsModifiedProperty, Boxes.False);
        }

        public void Save(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(fs);
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _textArea.GetService(serviceType);
        }

        public TextViewPosition? GetPositionFromPoint(Point point)
        {
            if (Document == null)
            {
                return null;
            }
            var textView = TextArea.TextView;
            return textView.GetPosition(TranslatePoint(point, textView) + textView.ScrollOffset);
        }

        public void ScrollToLine(int line)
        {
            ScrollTo(line, -1);
        }

        public void ScrollTo(int line, int column)
        {
            const double MINIMUM_SCROLL_PERCENTAGE = 0.3;

            var textView = _textArea.TextView;
            var document = textView.Document;
            if (_scrollViewer != null && document != null)
            {
                if (line < 1)
                {
                    line = 1;
                }
                if (line > document.LineCount)
                {
                    line = document.LineCount;
                }

                IScrollInfo scrollInfo = textView;
                if (!scrollInfo.CanHorizontallyScroll)
                {
                    var vl = textView.GetOrConstructVisualLine(document.GetLineByNumber(line));
                    double remainingHeight = _scrollViewer.ViewportHeight / 2;
                    while (remainingHeight > 0)
                    {
                        var prevLine = vl.FirstDocumentLine.PreviousLine;
                        if (prevLine == null)
                        {
                            break;
                        }
                        vl = textView.GetOrConstructVisualLine(prevLine);
                        remainingHeight -= vl.Height;
                    }
                }

                var p = _textArea.TextView.GetVisualPosition(new TextViewPosition(line, Math.Max(1, column)), VisualYPosition.LineMiddle);
                double verticalPos = p.Y - _scrollViewer.ViewportHeight / 2;
                if (Math.Abs(verticalPos - _scrollViewer.VerticalOffset) > MINIMUM_SCROLL_PERCENTAGE * _scrollViewer.ViewportHeight)
                {
                    _scrollViewer.ScrollToVerticalOffset(Math.Max(0, verticalPos));
                }
                if (column > 0)
                {
                    if (p.X > _scrollViewer.ViewportWidth - Caret.MinimumDistanceToViewBorder * 2)
                    {
                        double horizontalPos = Math.Max(0, p.X - _scrollViewer.ViewportWidth / 2);
                        if (Math.Abs(horizontalPos - _scrollViewer.HorizontalOffset) > MINIMUM_SCROLL_PERCENTAGE * _scrollViewer.ViewportWidth)
                        {
                            _scrollViewer.ScrollToHorizontalOffset(horizontalPos);
                        }
                    }
                    else
                    {
                        _scrollViewer.ScrollToHorizontalOffset(0);
                    }
                }
            }
        }

        #endregion
    }
}