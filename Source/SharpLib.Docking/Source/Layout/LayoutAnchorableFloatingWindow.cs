using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace SharpLib.Docking.Layout
{
    [Serializable]
    [ContentProperty("RootPanel")]
    public class LayoutAnchorableFloatingWindow : LayoutFloatingWindow, ILayoutElementWithVisibility
    {
        #region Поля

        [NonSerialized]
        private bool _isVisible = true;

        private LayoutAnchorablePaneGroup _rootPanel;

        #endregion

        #region Свойства

        public LayoutAnchorablePaneGroup RootPanel
        {
            get { return _rootPanel; }
            set
            {
                if (Equals(_rootPanel, value))
                {
                    return;
                }
                RaisePropertyChanging("RootPanel");

                if (_rootPanel != null)
                {
                    _rootPanel.ChildrenTreeChanged -= _rootPanel_ChildrenTreeChanged;
                }

                _rootPanel = value;
                if (_rootPanel != null)
                {
                    _rootPanel.Parent = this;
                }

                if (_rootPanel != null)
                {
                    _rootPanel.ChildrenTreeChanged += _rootPanel_ChildrenTreeChanged;
                }

                RaisePropertyChanged("RootPanel");
                RaisePropertyChanged("IsSinglePane");
                RaisePropertyChanged("SinglePane");
                RaisePropertyChanged("Children");
                RaisePropertyChanged("ChildrenCount");
                ((ILayoutElementWithVisibility)this).ComputeVisibility();
            }
        }

        public bool IsSinglePane
        {
            get { return RootPanel != null && RootPanel.Descendents().OfType<ILayoutAnchorablePane>().Where(p => p.IsVisible).Count() == 1; }
        }

        public ILayoutAnchorablePane SinglePane
        {
            get
            {
                if (!IsSinglePane)
                {
                    return null;
                }

                var singlePane = RootPanel.Descendents().OfType<LayoutAnchorablePane>().Single(p => p.IsVisible);
                singlePane.UpdateIsDirectlyHostedInFloatingWindow();
                return singlePane;
            }
        }

        public override IEnumerable<ILayoutElement> Children
        {
            get
            {
                if (ChildrenCount == 1)
                {
                    yield return RootPanel;
                }
            }
        }

        public override int ChildrenCount
        {
            get
            {
                if (RootPanel == null)
                {
                    return 0;
                }
                return 1;
            }
        }

        [XmlIgnore]
        public bool IsVisible
        {
            get { return _isVisible; }
            private set
            {
                if (_isVisible != value)
                {
                    RaisePropertyChanging("IsVisible");
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                    if (IsVisibleChanged != null)
                    {
                        IsVisibleChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public override bool IsValid
        {
            get { return RootPanel != null; }
        }

        #endregion

        #region События

        public event EventHandler IsVisibleChanged;

        #endregion

        #region Методы

        private void _rootPanel_ChildrenTreeChanged(object sender, ChildrenTreeChangedEventArgs e)
        {
            RaisePropertyChanged("IsSinglePane");
            RaisePropertyChanged("SinglePane");
        }

        public override void RemoveChild(ILayoutElement element)
        {
            Debug.Assert(Equals(element, RootPanel) && element != null);
            RootPanel = null;
        }

        public override void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
        {
            Debug.Assert(Equals(oldElement, RootPanel) && oldElement != null);
            RootPanel = newElement as LayoutAnchorablePaneGroup;
        }

        void ILayoutElementWithVisibility.ComputeVisibility()
        {
            IsVisible = RootPanel != null && RootPanel.IsVisible;
        }

        public override void ConsoleDump(int tab)
        {
            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine("FloatingAnchorableWindow()");

            RootPanel.ConsoleDump(tab + 1);
        }

        #endregion
    }
}