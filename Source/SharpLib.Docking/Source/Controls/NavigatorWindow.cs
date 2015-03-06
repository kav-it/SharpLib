using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SharpLib.Docking.Controls
{
    public class NavigatorWindow : Window
    {
        #region Поля

        public static readonly DependencyProperty AnchorablesProperty;

        public static readonly DependencyProperty DocumentsProperty;

        public static readonly DependencyProperty SelectedAnchorableProperty;

        public static readonly DependencyProperty SelectedDocumentProperty;

        private static readonly DependencyPropertyKey _anchorablesPropertyKey;

        private static readonly DependencyPropertyKey _documentsPropertyKey;

        private readonly DockingManager _manager;

        private bool _internalSetSelectedDocument;

        #endregion

        #region Свойства

        public LayoutDocumentItem[] Documents
        {
            get { return (LayoutDocumentItem[])GetValue(DocumentsProperty); }
        }

        public IEnumerable<LayoutAnchorableItem> Anchorables
        {
            get { return (IEnumerable<LayoutAnchorableItem>)GetValue(AnchorablesProperty); }
        }

        public LayoutDocumentItem SelectedDocument
        {
            get { return (LayoutDocumentItem)GetValue(SelectedDocumentProperty); }
            set { SetValue(SelectedDocumentProperty, value); }
        }

        public LayoutAnchorableItem SelectedAnchorable
        {
            get { return (LayoutAnchorableItem)GetValue(SelectedAnchorableProperty); }
            set { SetValue(SelectedAnchorableProperty, value); }
        }

        #endregion

        #region Конструктор

        static NavigatorWindow()
        {
            _anchorablesPropertyKey = DependencyProperty.RegisterReadOnly("Anchorables", typeof(IEnumerable<LayoutAnchorableItem>), typeof(NavigatorWindow),
                new FrameworkPropertyMetadata((IEnumerable<LayoutAnchorableItem>)null));
            _documentsPropertyKey = DependencyProperty.RegisterReadOnly("Documents", typeof(IEnumerable<LayoutDocumentItem>), typeof(NavigatorWindow), new FrameworkPropertyMetadata(null));
            SelectedAnchorableProperty = DependencyProperty.Register("SelectedAnchorable", typeof(LayoutAnchorableItem), typeof(NavigatorWindow),
                new FrameworkPropertyMetadata(null, OnSelectedAnchorableChanged));
            SelectedDocumentProperty = DependencyProperty.Register("SelectedDocument", typeof(LayoutDocumentItem), typeof(NavigatorWindow),
                new FrameworkPropertyMetadata(null, OnSelectedDocumentChanged));
            DocumentsProperty = _documentsPropertyKey.DependencyProperty;
            AnchorablesProperty = _anchorablesPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(typeof(NavigatorWindow)));
            ShowActivatedProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(false));
            ShowInTaskbarProperty.OverrideMetadata(typeof(NavigatorWindow), new FrameworkPropertyMetadata(false));
        }

        internal NavigatorWindow(DockingManager manager)
        {
            _manager = manager;

            _internalSetSelectedDocument = true;

            var layouts = _manager.Layout.Descendents().ToList();

            var anchorableItems = layouts.OfType<LayoutAnchorable>()
                .Where(a => a.IsVisible)
                .Select(d => (LayoutAnchorableItem)_manager.GetLayoutItemFromModel(d));

            var documents = layouts.OfType<LayoutDocument>()
                .OrderByDescending(d => d.LastActivationTimeStamp.GetValueOrDefault())
                .Select(d => (LayoutDocumentItem)_manager.GetLayoutItemFromModel(d))
                .ToArray();

            SetAnchorables(anchorableItems);
            SetDocuments(documents);
            _internalSetSelectedDocument = false;

            if (Documents.Length > 1)
            {
                InternalSetSelectedDocument(Documents[1]);
            }

            DataContext = this;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            UpdateThemeResources();
        }

        #endregion

        #region Методы

        internal void UpdateThemeResources(Theme oldTheme = null)
        {
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    Resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                {
                    Resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
                }
            }

            if (_manager.Theme != null)
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = _manager.Theme.GetResourceUri()
                });
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            Focus();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;
        }

        protected void SetDocuments(LayoutDocumentItem[] value)
        {
            SetValue(_documentsPropertyKey, value);
        }

        protected void SetAnchorables(IEnumerable<LayoutAnchorableItem> value)
        {
            SetValue(_anchorablesPropertyKey, value);
        }

        private static void OnSelectedDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigatorWindow)d).OnSelectedDocumentChanged(e);
        }

        protected virtual void OnSelectedDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_internalSetSelectedDocument)
            {
                return;
            }

            if (SelectedDocument == null || !SelectedDocument.ActivateCommand.CanExecute(null))
            {
                return;
            }

            SelectedDocument.ActivateCommand.Execute(null);
            Hide();
        }

        private void InternalSetSelectedDocument(LayoutDocumentItem documentToSelect)
        {
            _internalSetSelectedDocument = true;
            SelectedDocument = documentToSelect;
            _internalSetSelectedDocument = false;
        }

        private static void OnSelectedAnchorableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigatorWindow)d).OnSelectedAnchorableChanged(e);
        }

        protected virtual void OnSelectedAnchorableChanged(DependencyPropertyChangedEventArgs e)
        {
            var selectedAnchorable = e.NewValue as LayoutAnchorableItem;
            if (SelectedAnchorable != null && SelectedAnchorable.ActivateCommand.CanExecute(null))
            {
                SelectedAnchorable.ActivateCommand.Execute(null);
                Close();
            }
        }

        internal void SelectNextDocument()
        {
            if (SelectedDocument != null)
            {
                int docIndex = Documents.IndexOf(SelectedDocument);
                docIndex++;
                if (docIndex == Documents.Length)
                {
                    docIndex = 0;
                }
                InternalSetSelectedDocument(Documents[docIndex]);
            }
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                SelectNextDocument();
                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Tab)
            {
                if (SelectedAnchorable != null &&
                    SelectedAnchorable.ActivateCommand.CanExecute(null))
                {
                    SelectedAnchorable.ActivateCommand.Execute(null);
                }

                if (SelectedAnchorable == null &&
                    SelectedDocument != null &&
                    SelectedDocument.ActivateCommand.CanExecute(null))
                {
                    SelectedDocument.ActivateCommand.Execute(null);
                }
                Close();
                e.Handled = true;
            }

            base.OnPreviewKeyUp(e);
        }

        #endregion
    }
}