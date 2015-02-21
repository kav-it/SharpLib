using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;

namespace SharpLib.Docking
{
    [ContentProperty("Content")]
    [Serializable]
    public abstract class LayoutContent : LayoutElement, IXmlSerializable, ILayoutElementForFloatingWindow, IComparable<LayoutContent>, ILayoutPreviousContainer
    {
        #region Поля

        public static readonly DependencyProperty TitleProperty;

        private bool _canClose;

        private bool _canFloat;

        [NonSerialized]
        private object _content;

        private string _contentId;

        private double _floatingHeight;

        private double _floatingLeft;

        private double _floatingTop;

        private double _floatingWidth;

        private ImageSource _iconSource;

        [field: NonSerialized]
        private bool _isActive;

        private bool _isLastFocusedDocument;

        private bool _isMaximized;

        private bool _isSelected;

        private DateTime? _lastActivationTimeStamp;

        [field: NonSerialized]
        private ILayoutContainer _previousContainer;

        [field: NonSerialized]
        private int _previousContainerIndex = -1;

        private object _toolTip;

        #endregion

        #region Свойства

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        [XmlIgnore]
        public object Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    RaisePropertyChanging("Content");
                    _content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }

        public string ContentId
        {
            get
            {
                if (_contentId == null)
                {
                    var contentAsControl = _content as FrameworkElement;
                    if (contentAsControl != null && !string.IsNullOrWhiteSpace(contentAsControl.Name))
                    {
                        return contentAsControl.Name;
                    }
                }
                return _contentId;
            }
            set
            {
                if (_contentId != value)
                {
                    _contentId = value;
                    RaisePropertyChanged("ContentId");
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    bool oldValue = _isSelected;
                    RaisePropertyChanging("IsSelected");
                    _isSelected = value;
                    var parentSelector = (Parent as ILayoutContentSelector);
                    if (parentSelector != null)
                    {
                        parentSelector.SelectedContentIndex = _isSelected ? parentSelector.IndexOf(this) : -1;
                    }
                    OnIsSelectedChanged(oldValue, value);
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        [XmlIgnore]
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    RaisePropertyChanging("IsActive");
                    bool oldValue = _isActive;

                    _isActive = value;

                    var root = Root;
                    if (root != null && _isActive)
                    {
                        root.ActiveContent = this;
                    }

                    if (_isActive)
                    {
                        IsSelected = true;
                    }

                    OnIsActiveChanged(oldValue, value);
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        public bool IsLastFocusedDocument
        {
            get { return _isLastFocusedDocument; }
            internal set
            {
                if (_isLastFocusedDocument != value)
                {
                    RaisePropertyChanging("IsLastFocusedDocument");
                    _isLastFocusedDocument = value;
                    RaisePropertyChanged("IsLastFocusedDocument");
                }
            }
        }

        [XmlIgnore]
        ILayoutContainer ILayoutPreviousContainer.PreviousContainer
        {
            get { return _previousContainer; }
            set
            {
                if (_previousContainer != value)
                {
                    _previousContainer = value;
                    RaisePropertyChanged("PreviousContainer");

                    var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                    if (paneSerializable != null &&
                        paneSerializable.Id == null)
                    {
                        paneSerializable.Id = Guid.NewGuid().ToString();
                    }
                }
            }
        }

        protected ILayoutContainer PreviousContainer
        {
            get { return ((ILayoutPreviousContainer)this).PreviousContainer; }
            set { ((ILayoutPreviousContainer)this).PreviousContainer = value; }
        }

        [XmlIgnore]
        string ILayoutPreviousContainer.PreviousContainerId { get; set; }

        protected string PreviousContainerId
        {
            get { return ((ILayoutPreviousContainer)this).PreviousContainerId; }
            set { ((ILayoutPreviousContainer)this).PreviousContainerId = value; }
        }

        [XmlIgnore]
        public int PreviousContainerIndex
        {
            get { return _previousContainerIndex; }
            set
            {
                if (_previousContainerIndex != value)
                {
                    _previousContainerIndex = value;
                    RaisePropertyChanged("PreviousContainerIndex");
                }
            }
        }

        public DateTime? LastActivationTimeStamp
        {
            get { return _lastActivationTimeStamp; }
            set
            {
                if (_lastActivationTimeStamp != value)
                {
                    _lastActivationTimeStamp = value;
                    RaisePropertyChanged("LastActivationTimeStamp");
                }
            }
        }

        public double FloatingWidth
        {
            get { return _floatingWidth; }
            set
            {
                if (_floatingWidth.NotEqualEx(value))
                {
                    RaisePropertyChanging("FloatingWidth");
                    _floatingWidth = value;
                    RaisePropertyChanged("FloatingWidth");
                }
            }
        }

        public double FloatingHeight
        {
            get { return _floatingHeight; }
            set
            {
                if (_floatingHeight.NotEqualEx(value))
                {
                    RaisePropertyChanging("FloatingHeight");
                    _floatingHeight = value;
                    RaisePropertyChanged("FloatingHeight");
                }
            }
        }

        public double FloatingLeft
        {
            get { return _floatingLeft; }
            set
            {
                if (_floatingLeft.NotEqualEx(value))
                {
                    RaisePropertyChanging("FloatingLeft");
                    _floatingLeft = value;
                    RaisePropertyChanged("FloatingLeft");
                }
            }
        }

        public double FloatingTop
        {
            get { return _floatingTop; }
            set
            {
                if (_floatingTop.NotEqualEx(value))
                {
                    RaisePropertyChanging("FloatingTop");
                    _floatingTop = value;
                    RaisePropertyChanged("FloatingTop");
                }
            }
        }

        public bool IsMaximized
        {
            get { return _isMaximized; }
            set
            {
                if (_isMaximized != value)
                {
                    RaisePropertyChanging("IsMaximized");
                    _isMaximized = value;
                    RaisePropertyChanged("IsMaximized");
                }
            }
        }

        public object ToolTip
        {
            get { return _toolTip; }
            set
            {
                if (_toolTip != value)
                {
                    _toolTip = value;
                    RaisePropertyChanged("ToolTip");
                }
            }
        }

        public bool IsFloating
        {
            get { return this.FindParent<LayoutFloatingWindow>() != null; }
        }

        public ImageSource IconSource
        {
            get { return _iconSource; }
            set
            {
                if (!Equals(_iconSource, value))
                {
                    _iconSource = value;
                    RaisePropertyChanged("IconSource");
                }
            }
        }

        public bool CanClose
        {
            get { return _canClose; }
            set
            {
                if (_canClose != value)
                {
                    _canClose = value;
                    RaisePropertyChanged("CanClose");
                }
            }
        }

        public bool CanFloat
        {
            get { return _canFloat; }
            set
            {
                if (_canFloat != value)
                {
                    _canFloat = value;
                    RaisePropertyChanged("CanFloat");
                }
            }
        }

        #endregion

        #region События

        public event EventHandler Closed;

        public event EventHandler<CancelEventArgs> Closing;

        public event EventHandler IsActiveChanged;

        public event EventHandler IsSelectedChanged;

        #endregion

        #region Конструктор

        static LayoutContent()
        {
            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LayoutContent), new UIPropertyMetadata(null, OnTitlePropertyChanged, CoerceTitleValue));
        }

        internal LayoutContent()
        {
            _canFloat = true;
            _canClose = true;
        }

        #endregion

        #region Методы

        private static object CoerceTitleValue(DependencyObject obj, object value)
        {
            var lc = (LayoutContent)obj;
            if (((string)value) != lc.Title)
            {
                lc.RaisePropertyChanging(TitleProperty.Name);
            }
            return value;
        }

        private static void OnTitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((LayoutContent)obj).RaisePropertyChanged(TitleProperty.Name);
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            if (IsSelectedChanged != null)
            {
                IsSelectedChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                LastActivationTimeStamp = DateTime.Now;
            }

            if (IsActiveChanged != null)
            {
                IsActiveChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            var root = Root;

            if (oldValue != null)
            {
                IsSelected = false;
            }

            base.OnParentChanging(oldValue, newValue);
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            if (IsSelected && Parent is ILayoutContentSelector)
            {
                var parentSelector = (Parent as ILayoutContentSelector);
                parentSelector.SelectedContentIndex = parentSelector.IndexOf(this);
            }

            base.OnParentChanged(oldValue, newValue);
        }

        internal bool TestCanClose()
        {
            var args = new CancelEventArgs();

            OnClosing(args);

            if (args.Cancel)
            {
                return false;
            }

            return true;
        }

        public void Close()
        {
            var root = Root;
            var parentAsContainer = Parent;
            parentAsContainer.RemoveChild(this);
            if (root != null)
            {
                root.CollectGarbage();
            }

            OnClosed();
        }

        protected virtual void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        protected virtual void OnClosing(CancelEventArgs args)
        {
            if (Closing != null)
            {
                Closing(this, args);
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Title"))
            {
                Title = reader.Value;
            }

            if (reader.MoveToAttribute("IsSelected"))
            {
                IsSelected = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("ContentId"))
            {
                ContentId = reader.Value;
            }
            if (reader.MoveToAttribute("IsLastFocusedDocument"))
            {
                IsLastFocusedDocument = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("PreviousContainerId"))
            {
                PreviousContainerId = reader.Value;
            }
            if (reader.MoveToAttribute("PreviousContainerIndex"))
            {
                PreviousContainerIndex = int.Parse(reader.Value);
            }

            if (reader.MoveToAttribute("FloatingLeft"))
            {
                FloatingLeft = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("FloatingTop"))
            {
                FloatingTop = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("FloatingWidth"))
            {
                FloatingWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("FloatingHeight"))
            {
                FloatingHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("IsMaximized"))
            {
                IsMaximized = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("CanClose"))
            {
                CanClose = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("CanFloat"))
            {
                CanFloat = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("LastActivationTimeStamp"))
            {
                LastActivationTimeStamp = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture);
            }

            reader.Read();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(Title))
            {
                writer.WriteAttributeString("Title", Title);
            }

            if (IsSelected)
            {
                writer.WriteAttributeString("IsSelected", IsSelected.ToString());
            }

            if (IsLastFocusedDocument)
            {
                writer.WriteAttributeString("IsLastFocusedDocument", IsLastFocusedDocument.ToString());
            }

            if (!string.IsNullOrWhiteSpace(ContentId))
            {
                writer.WriteAttributeString("ContentId", ContentId);
            }

            if (ToolTip is string)
            {
                if (!string.IsNullOrWhiteSpace((string)ToolTip))
                {
                    writer.WriteAttributeString("ToolTip", (string)ToolTip);
                }
            }

            if (FloatingLeft.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingLeft", FloatingLeft.ToString(CultureInfo.InvariantCulture));
            }
            if (FloatingTop.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingTop", FloatingTop.ToString(CultureInfo.InvariantCulture));
            }
            if (FloatingWidth.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingWidth", FloatingWidth.ToString(CultureInfo.InvariantCulture));
            }
            if (FloatingHeight.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingHeight", FloatingHeight.ToString(CultureInfo.InvariantCulture));
            }

            if (IsMaximized)
            {
                writer.WriteAttributeString("IsMaximized", IsMaximized.ToString());
            }
            if (!CanClose)
            {
                writer.WriteAttributeString("CanClose", CanClose.ToString());
            }
            if (!CanFloat)
            {
                writer.WriteAttributeString("CanFloat", CanFloat.ToString());
            }

            if (LastActivationTimeStamp != null)
            {
                writer.WriteAttributeString("LastActivationTimeStamp", LastActivationTimeStamp.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (_previousContainer != null)
            {
                var paneSerializable = _previousContainer as ILayoutPaneSerializable;
                if (paneSerializable != null)
                {
                    writer.WriteAttributeString("PreviousContainerId", paneSerializable.Id);
                    writer.WriteAttributeString("PreviousContainerIndex", _previousContainerIndex.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public int CompareTo(LayoutContent other)
        {
            var contentAsComparable = Content as IComparable;
            if (contentAsComparable != null)
            {
                return contentAsComparable.CompareTo(other.Content);
            }

            return string.CompareOrdinal(Title, other.Title);
        }

        public void Float()
        {
            if (PreviousContainer != null &&
                PreviousContainer.FindParent<LayoutFloatingWindow>() != null)
            {
                var currentContainer = Parent as ILayoutPane;
                var layoutGroup = currentContainer as ILayoutGroup;
                if (layoutGroup != null)
                {
                    var currentContainerIndex = layoutGroup.IndexOfChild(this);
                    var previousContainerAsLayoutGroup = PreviousContainer as ILayoutGroup;

                    if (previousContainerAsLayoutGroup != null)
                    {
                        previousContainerAsLayoutGroup.InsertChildAt(
                            PreviousContainerIndex < previousContainerAsLayoutGroup.ChildrenCount ? PreviousContainerIndex : previousContainerAsLayoutGroup.ChildrenCount, this);
                    }

                    PreviousContainer = currentContainer;
                    PreviousContainerIndex = currentContainerIndex;
                }

                IsSelected = true;
                IsActive = true;

                Root.CollectGarbage();
            }
            else
            {
                Root.Manager.StartDraggingFloatingWindowForContent(this, false);

                IsSelected = true;
                IsActive = true;
            }
        }

        public void DockAsDocument()
        {
            var root = Root as LayoutRoot;
            if (root == null)
            {
                throw new InvalidOperationException();
            }
            if (Parent is LayoutDocumentPane)
            {
                return;
            }

            if (PreviousContainer is LayoutDocumentPane)
            {
                Dock();
                return;
            }

            LayoutDocumentPane newParentPane;
            if (root.LastFocusedDocument != null)
            {
                newParentPane = root.LastFocusedDocument.Parent as LayoutDocumentPane;
            }
            else
            {
                newParentPane = root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            }

            if (newParentPane != null)
            {
                newParentPane.Children.Add(this);
                root.CollectGarbage();
            }

            IsSelected = true;
            IsActive = true;
        }

        public void Dock()
        {
            if (PreviousContainer != null)
            {
                var currentContainer = Parent;
                var currentContainerIndex = (currentContainer is ILayoutGroup) ? (currentContainer as ILayoutGroup).IndexOfChild(this) : -1;
                var previousContainerAsLayoutGroup = PreviousContainer as ILayoutGroup;

                if (previousContainerAsLayoutGroup != null)
                {
                    previousContainerAsLayoutGroup.InsertChildAt(
                        PreviousContainerIndex < previousContainerAsLayoutGroup.ChildrenCount ? PreviousContainerIndex : previousContainerAsLayoutGroup.ChildrenCount, this);
                }

                if (currentContainerIndex > -1)
                {
                    PreviousContainer = currentContainer;
                    PreviousContainerIndex = currentContainerIndex;
                }
                else
                {
                    PreviousContainer = null;
                    PreviousContainerIndex = 0;
                }

                IsSelected = true;
                IsActive = true;
            }
            else
            {
                InternalDock();
            }

            Root.CollectGarbage();
        }

        protected virtual void InternalDock()
        {
        }

        #endregion
    }
}