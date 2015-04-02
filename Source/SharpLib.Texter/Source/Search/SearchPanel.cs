using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using SharpLib.Texter.Document;
using SharpLib.Texter.Editing;
using SharpLib.Texter.Rendering;

namespace SharpLib.Texter.Search
{
    public class SearchPanel : Control
    {
        #region Поля

        public static readonly DependencyProperty LocalizationProperty;

        public static readonly DependencyProperty MarkerBrushProperty;

        public static readonly DependencyProperty MatchCaseProperty;

        public static readonly DependencyProperty SearchPatternProperty;

        public static readonly DependencyProperty UseRegexProperty;

        public static readonly DependencyProperty WholeWordsProperty;

        private readonly ToolTip _messageView;

        private SearchPanelAdorner _adorner;

        private TextDocument _currentDocument;

        private SearchInputHandler _handler;

        private SearchResultBackgroundRenderer _renderer;

        private TextBox _searchTextBox;

        private ISearchStrategy _strategy;

        private TextArea _textArea;

        #endregion

        #region Свойства

        public bool UseRegex
        {
            get { return (bool)GetValue(UseRegexProperty); }
            set { SetValue(UseRegexProperty, value); }
        }

        public bool MatchCase
        {
            get { return (bool)GetValue(MatchCaseProperty); }
            set { SetValue(MatchCaseProperty, value); }
        }

        public bool WholeWords
        {
            get { return (bool)GetValue(WholeWordsProperty); }
            set { SetValue(WholeWordsProperty, value); }
        }

        public string SearchPattern
        {
            get { return (string)GetValue(SearchPatternProperty); }
            set { SetValue(SearchPatternProperty, value); }
        }

        public Brush MarkerBrush
        {
            get { return (Brush)GetValue(MarkerBrushProperty); }
            set { SetValue(MarkerBrushProperty, value); }
        }

        public Localization Localization
        {
            get { return (Localization)GetValue(LocalizationProperty); }
            set { SetValue(LocalizationProperty, value); }
        }

        public bool IsClosed { get; private set; }

        #endregion

        #region События

        public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged;

        #endregion

        #region Конструктор

        static SearchPanel()
        {
            LocalizationProperty = DependencyProperty.Register("Localization", typeof(Localization), typeof(SearchPanel), new FrameworkPropertyMetadata(new Localization()));
            MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(SearchPanel), new FrameworkPropertyMetadata(Brushes.LightGreen, MarkerBrushChangedCallback));
            MatchCaseProperty = DependencyProperty.Register("MatchCase", typeof(bool), typeof(SearchPanel), new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
            SearchPatternProperty = DependencyProperty.Register("SearchPattern", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata("", SearchPatternChangedCallback));
            UseRegexProperty = DependencyProperty.Register("UseRegex", typeof(bool), typeof(SearchPanel), new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
            WholeWordsProperty = DependencyProperty.Register("WholeWords", typeof(bool), typeof(SearchPanel), new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchPanel), new FrameworkPropertyMetadata(typeof(SearchPanel)));
        }

        private SearchPanel()
        {
            _messageView = new ToolTip
            {
                Placement = PlacementMode.Bottom,
                StaysOpen = true,
                Focusable = false
            };
        }

        #endregion

        #region Методы

        private static void MarkerBrushChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as SearchPanel;
            if (panel != null)
            {
                panel._renderer.MarkerBrush = (Brush)e.NewValue;
            }
        }

        private static void SearchPatternChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as SearchPanel;
            if (panel != null)
            {
                panel.ValidateSearchText();
                panel.UpdateSearch();
            }
        }

        private void UpdateSearch()
        {
            if (_renderer.CurrentResults.Any())
            {
                _messageView.IsOpen = false;
            }
            _strategy = SearchStrategyFactory.Create(SearchPattern ?? "", !MatchCase, WholeWords, UseRegex ? SearchMode.RegEx : SearchMode.Normal);
            OnSearchOptionsChanged(new SearchOptionsChangedEventArgs(SearchPattern, MatchCase, UseRegex, WholeWords));
            DoSearch(true);
        }

        [Obsolete("Use the Install method instead")]
        public void Attach(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            AttachInternal(textArea);
        }

        public static SearchPanel Install(TextEditor editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            return Install(editor.TextArea);
        }

        public static SearchPanel Install(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            var panel = new SearchPanel();
            panel.AttachInternal(textArea);
            panel._handler = new SearchInputHandler(textArea, panel);
            textArea.DefaultInputHandler.NestedInputHandlers.Add(panel._handler);
            return panel;
        }

        public void RegisterCommands(CommandBindingCollection commandBindings)
        {
            _handler.RegisterGlobalCommands(commandBindings);
        }

        public void Uninstall()
        {
            CloseAndRemove();
            _textArea.DefaultInputHandler.NestedInputHandlers.Remove(_handler);
        }

        private void AttachInternal(TextArea textArea)
        {
            _textArea = textArea;
            _adorner = new SearchPanelAdorner(textArea, this);
            DataContext = this;

            _renderer = new SearchResultBackgroundRenderer();
            _currentDocument = textArea.Document;
            if (_currentDocument != null)
            {
                _currentDocument.TextChanged += textArea_Document_TextChanged;
            }
            textArea.DocumentChanged += textArea_DocumentChanged;
            KeyDown += SearchLayerKeyDown;

            CommandBindings.Add(new CommandBinding(SearchCommands.FindNext, (sender, e) => FindNext()));
            CommandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, (sender, e) => FindPrevious()));
            CommandBindings.Add(new CommandBinding(SearchCommands.CloseSearchPanel, (sender, e) => Close()));
            IsClosed = true;
        }

        private void textArea_DocumentChanged(object sender, EventArgs e)
        {
            if (_currentDocument != null)
            {
                _currentDocument.TextChanged -= textArea_Document_TextChanged;
            }
            _currentDocument = _textArea.Document;
            if (_currentDocument != null)
            {
                _currentDocument.TextChanged += textArea_Document_TextChanged;
                DoSearch(false);
            }
        }

        private void textArea_Document_TextChanged(object sender, EventArgs e)
        {
            DoSearch(false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _searchTextBox = Template.FindName("PART_searchTextBox", this) as TextBox;
        }

        private void ValidateSearchText()
        {
            if (_searchTextBox == null)
            {
                return;
            }
            var be = _searchTextBox.GetBindingExpression(TextBox.TextProperty);
            try
            {
                Validation.ClearInvalid(be);
                UpdateSearch();
            }
            catch (SearchPatternException ex)
            {
                var ve = new ValidationError(be.ParentBinding.ValidationRules[0], be, ex.Message, ex);
                Validation.MarkInvalid(be, ve);
            }
        }

        public void Reactivate()
        {
            if (_searchTextBox == null)
            {
                return;
            }
            _searchTextBox.Focus();
            _searchTextBox.SelectAll();
        }

        public void FindNext()
        {
            var result = _renderer.CurrentResults.FindFirstSegmentWithStartAfter(_textArea.Caret.Offset + 1) 
                ?? _renderer.CurrentResults.FirstSegment;
            if (result != null)
            {
                SelectResult(result);
            }
        }

        public void FindPrevious()
        {
            var result = _renderer.CurrentResults.FindFirstSegmentWithStartAfter(_textArea.Caret.Offset);
            if (result != null)
            {
                result = _renderer.CurrentResults.GetPreviousSegment(result);
            }
            if (result == null)
            {
                result = _renderer.CurrentResults.LastSegment;
            }
            if (result != null)
            {
                SelectResult(result);
            }
        }

        private void DoSearch(bool changeSelection)
        {
            if (IsClosed)
            {
                return;
            }
            _renderer.CurrentResults.Clear();

            if (!string.IsNullOrEmpty(SearchPattern))
            {
                int offset = _textArea.Caret.Offset;
                if (changeSelection)
                {
                    _textArea.ClearSelection();
                }

                foreach (var searchResult in _strategy.FindAll(_textArea.Document, 0, _textArea.Document.TextLength))
                {
                    var result = (SearchResult)searchResult;
                    if (changeSelection && result.StartOffset >= offset)
                    {
                        SelectResult(result);
                        changeSelection = false;
                    }
                    _renderer.CurrentResults.Add(result);
                }
                if (!_renderer.CurrentResults.Any())
                {
                    _messageView.IsOpen = true;
                    _messageView.Content = Localization.NoMatchesFoundText;
                    _messageView.PlacementTarget = _searchTextBox;
                }
                else
                {
                    _messageView.IsOpen = false;
                }
            }
            _textArea.TextView.InvalidateLayer(KnownLayer.Selection);
        }

        private void SelectResult(SearchResult result)
        {
            _textArea.Caret.Offset = result.StartOffset;
            _textArea.Selection = Selection.Create(_textArea, result.StartOffset, result.EndOffset);
            _textArea.Caret.BringCaretToView();

            _textArea.Caret.Show();
        }

        private void SearchLayerKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    e.Handled = true;
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        FindPrevious();
                    }
                    else
                    {
                        FindNext();
                    }
                    if (_searchTextBox != null)
                    {
                        var error = Validation.GetErrors(_searchTextBox).FirstOrDefault();
                        if (error != null)
                        {
                            _messageView.Content = Localization.ErrorText + " " + error.ErrorContent;
                            _messageView.PlacementTarget = _searchTextBox;
                            _messageView.IsOpen = true;
                        }
                    }
                    break;
                case Key.Escape:
                    e.Handled = true;
                    Close();
                    break;
            }
        }

        public void Close()
        {
            bool hasFocus = IsKeyboardFocusWithin;

            var layer = AdornerLayer.GetAdornerLayer(_textArea);
            if (layer != null)
            {
                layer.Remove(_adorner);
            }
            _messageView.IsOpen = false;
            _textArea.TextView.BackgroundRenderers.Remove(_renderer);
            if (hasFocus)
            {
                _textArea.Focus();
            }
            IsClosed = true;

            _renderer.CurrentResults.Clear();
        }

        internal void CloseAndRemove()
        {
            Close();
            _textArea.DocumentChanged -= textArea_DocumentChanged;
            if (_currentDocument != null)
            {
                _currentDocument.TextChanged -= textArea_Document_TextChanged;
            }
        }

        public void Open()
        {
            if (!IsClosed)
            {
                return;
            }
            var layer = AdornerLayer.GetAdornerLayer(_textArea);
            if (layer != null)
            {
                layer.Add(_adorner);
            }
            _textArea.TextView.BackgroundRenderers.Add(_renderer);
            IsClosed = false;
            DoSearch(false);
        }

        protected virtual void OnSearchOptionsChanged(SearchOptionsChangedEventArgs e)
        {
            if (SearchOptionsChanged != null)
            {
                SearchOptionsChanged(this, e);
            }
        }

        #endregion
    }
}