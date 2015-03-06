using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Xml.Serialization;

namespace SharpLib.Docking
{
    [Serializable]
    public abstract class LayoutElement : DependencyObject, ILayoutElement
    {
        #region Поля

        [NonSerialized]
        private ILayoutContainer _parent;

        [NonSerialized]
        private ILayoutRoot _root;

        #endregion

        #region Свойства

        /// <summary>
        /// Тестовое описание (для внутреннего использования в отладке)
        /// </summary>
        [XmlIgnore]
        public string InternalDesc { get; set; }

        [XmlIgnore]
        public ILayoutContainer Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    var oldValue = _parent;
                    var oldRoot = _root;
                    RaisePropertyChanging("Parent");
                    OnParentChanging(oldValue, value);
                    _parent = value;
                    OnParentChanged(oldValue, value);

                    _root = Root;
                    if (oldRoot != _root)
                    {
                        OnRootChanged(oldRoot, _root);
                    }

                    RaisePropertyChanged("Parent");

                    var root = Root as LayoutRoot;
                    if (root != null)
                    {
                        root.FireLayoutUpdated();
                    }
                }
            }
        }

        public ILayoutRoot Root
        {
            get
            {
                var parent = Parent;

                while (parent != null && (!(parent is ILayoutRoot)))
                {
                    parent = parent.Parent;
                }

                return parent as ILayoutRoot;
            }
        }

        #endregion

        #region События

        [field: NonSerialized]
        [field: XmlIgnore]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        [field: XmlIgnore]
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Конструктор

        internal LayoutElement()
        {
        }

        #endregion

        #region Методы

        protected virtual void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
        }

        protected virtual void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
        }

        protected virtual void OnRootChanged(ILayoutRoot oldRoot, ILayoutRoot newRoot)
        {
            if (oldRoot != null)
            {
                ((LayoutRoot)oldRoot).OnLayoutElementRemoved(this);
            }
            if (newRoot != null)
            {
                ((LayoutRoot)newRoot).OnLayoutElementAdded(this);
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void RaisePropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        public virtual void ConsoleDump(int tab)
        {
            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine(ToString());
        }

        /// <summary>
        /// Текстовое представление объекта
        /// </summary>
        public override string ToString()
        {
            if (InternalDesc.IsValid())
            {
                return string.Format("{0} ({1})", InternalDesc, base.ToString());
            }

            return base.ToString();
        }

        #endregion
    }
}