using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search
{
    public class SearchPanel : Control
    {
        #region Поля

        public static readonly DependencyProperty LocalizationProperty =
            DependencyProperty.Register("Localization", typeof(Localization), typeof(SearchPanel),
                new FrameworkPropertyMetadata(new Localization()));

        public static readonly DependencyProperty MarkerBrushProperty =
            DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(SearchPanel),
                new FrameworkPropertyMetadata(Brushes.LightGreen, MarkerBrushChangedCallback));

        public static readonly DependencyProperty MatchCaseProperty =
            DependencyProperty.Register("MatchCase", typeof(bool), typeof(SearchPanel),
                new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));

        public static readonly DependencyProperty SearchPatternProperty =
            DependencyProperty.Register("SearchPattern", typeof(string), typeof(SearchPanel),
                new FrameworkPropertyMetadata("", SearchPatternChangedCallback));

        public static readonly DependencyProperty UseRegexProperty =
            DependencyProperty.Register("UseRegex", typeof(bool), typeof(SearchPanel),
                new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));

        public static readonly DependencyProperty WholeWordsProperty =
            DependencyProperty.Register("WholeWords", typeof(bool), typeof(SearchPanel),
                new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));

        private readonly ToolTip messageView = new ToolTip
        {
            Placement = PlacementMode.Bottom,
            StaysOpen = true,
            Focusable = false
        };

        private SearchPanelAdorner adorner;

        private TextDocument currentDocument;

        private SearchInputHandler handler;

        private SearchResultBackgroundRenderer renderer;

        private TextBox searchTextBox;

        private ISearchStrategy strategy;

        private TextArea textArea;

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
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchPanel), new FrameworkPropertyMetadata(typeof(SearchPanel)));
        }

        private SearchPanel()
        {
        }

        #endregion

        #region Методы

        private static void MarkerBrushChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as SearchPanel;
            if (panel != null)
            {
                panel.renderer.MarkerBrush = (Brush)e.NewValue;
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
            if (renderer.CurrentResults.Any())
            {
                messageView.IsOpen = false;
            }
            strategy = SearchStrategyFactory.Create(SearchPattern ?? "", !MatchCase, WholeWords, UseRegex ? SearchMode.RegEx : SearchMode.Normal);
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
            panel.handler = new SearchInputHandler(textArea, panel);
            textArea.DefaultInputHandler.NestedInputHandlers.Add(panel.handler);
            return panel;
        }

        public void RegisterCommands(CommandBindingCollection commandBindings)
        {
            handler.RegisterGlobalCommands(commandBindings);
        }

        public void Uninstall()
        {
            CloseAndRemove();
            textArea.DefaultInputHandler.NestedInputHandlers.Remove(handler);
        }

        private void AttachInternal(TextArea textArea)
        {
            this.textArea = textArea;
            adorner = new SearchPanelAdorner(textArea, this);
            DataContext = this;

            renderer = new SearchResultBackgroundRenderer();
            currentDocument = textArea.Document;
            if (currentDocument != null)
            {
                currentDocument.TextChanged += textArea_Document_TextChanged;
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
            if (currentDocument != null)
            {
                currentDocument.TextChanged -= textArea_Document_TextChanged;
            }
            currentDocument = textArea.Document;
            if (currentDocument != null)
            {
                currentDocument.TextChanged += textArea_Document_TextChanged;
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
            searchTextBox = Template.FindName("PART_searchTextBox", this) as TextBox;
        }

        private void ValidateSearchText()
        {
            if (searchTextBox == null)
            {
                return;
            }
            var be = searchTextBox.GetBindingExpression(TextBox.TextProperty);
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
            if (searchTextBox == null)
            {
                return;
            }
            searchTextBox.Focus();
            searchTextBox.SelectAll();
        }

        public void FindNext()
        {
            var result = renderer.CurrentResults.FindFirstSegmentWithStartAfter(textArea.Caret.Offset + 1);
            if (result == null)
            {
                result = renderer.CurrentResults.FirstSegment;
            }
            if (result != null)
            {
                SelectResult(result);
            }
        }

        public void FindPrevious()
        {
            var result = renderer.CurrentResults.FindFirstSegmentWithStartAfter(textArea.Caret.Offset);
            if (result != null)
            {
                result = renderer.CurrentResults.GetPreviousSegment(result);
            }
            if (result == null)
            {
                result = renderer.CurrentResults.LastSegment;
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
            renderer.CurrentResults.Clear();

            if (!string.IsNullOrEmpty(SearchPattern))
            {
                int offset = textArea.Caret.Offset;
                if (changeSelection)
                {
                    textArea.ClearSelection();
                }

                foreach (SearchResult result in strategy.FindAll(textArea.Document, 0, textArea.Document.TextLength))
                {
                    if (changeSelection && result.StartOffset >= offset)
                    {
                        SelectResult(result);
                        changeSelection = false;
                    }
                    renderer.CurrentResults.Add(result);
                }
                if (!renderer.CurrentResults.Any())
                {
                    messageView.IsOpen = true;
                    messageView.Content = Localization.NoMatchesFoundText;
                    messageView.PlacementTarget = searchTextBox;
                }
                else
                {
                    messageView.IsOpen = false;
                }
            }
            textArea.TextView.InvalidateLayer(KnownLayer.Selection);
        }

        private void SelectResult(SearchResult result)
        {
            textArea.Caret.Offset = result.StartOffset;
            textArea.Selection = Selection.Create(textArea, result.StartOffset, result.EndOffset);
            textArea.Caret.BringCaretToView();

            textArea.Caret.Show();
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
                    if (searchTextBox != null)
                    {
                        var error = Validation.GetErrors(searchTextBox).FirstOrDefault();
                        if (error != null)
                        {
                            messageView.Content = Localization.ErrorText + " " + error.ErrorContent;
                            messageView.PlacementTarget = searchTextBox;
                            messageView.IsOpen = true;
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

            var layer = AdornerLayer.GetAdornerLayer(textArea);
            if (layer != null)
            {
                layer.Remove(adorner);
            }
            messageView.IsOpen = false;
            textArea.TextView.BackgroundRenderers.Remove(renderer);
            if (hasFocus)
            {
                textArea.Focus();
            }
            IsClosed = true;

            renderer.CurrentResults.Clear();
        }

        [Obsolete("Use the Uninstall method instead!")]
        public void CloseAndRemove()
        {
            Close();
            textArea.DocumentChanged -= textArea_DocumentChanged;
            if (currentDocument != null)
            {
                currentDocument.TextChanged -= textArea_Document_TextChanged;
            }
        }

        public void Open()
        {
            if (!IsClosed)
            {
                return;
            }
            var layer = AdornerLayer.GetAdornerLayer(textArea);
            if (layer != null)
            {
                layer.Add(adorner);
            }
            textArea.TextView.BackgroundRenderers.Add(renderer);
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