using System;
using System.Globalization;
using System.Windows;

namespace SharpLib.Docking.Layout
{
    [Serializable]
    public abstract class LayoutPositionableGroup<T> : LayoutGroup<T>, ILayoutPositionableElement, ILayoutPositionableElementWithActualSize where T : class, ILayoutElement
    {
        #region Поля

        private static GridLengthConverter _gridLengthConverter;

        [NonSerialized]
        private double _actualHeight;

        [NonSerialized]
        private double _actualWidth;

        private GridLength _dockHeight;

        private double _dockMinHeight;

        private double _dockMinWidth;

        private GridLength _dockWidth;

        private double _floatingHeight;

        private double _floatingLeft;

        private double _floatingTop;

        private double _floatingWidth;

        private bool _isMaximized;

        #endregion

        #region Свойства

        public GridLength DockWidth
        {
            get { return _dockWidth; }
            set
            {
                if (DockWidth != value)
                {
                    RaisePropertyChanging("DockWidth");
                    _dockWidth = value;
                    RaisePropertyChanged("DockWidth");

                    OnDockWidthChanged();
                }
            }
        }

        public GridLength DockHeight
        {
            get { return _dockHeight; }
            set
            {
                if (DockHeight != value)
                {
                    RaisePropertyChanging("DockHeight");
                    _dockHeight = value;
                    RaisePropertyChanged("DockHeight");

                    OnDockHeightChanged();
                }
            }
        }

        public double DockMinWidth
        {
            get { return _dockMinWidth; }
            set
            {
                if (_dockMinWidth.NotEqualEx(value))
                {
                    MathHelper.AssertIsPositiveOrZero(value);
                    RaisePropertyChanging("DockMinWidth");
                    _dockMinWidth = value;
                    RaisePropertyChanged("DockMinWidth");
                }
            }
        }

        public double DockMinHeight
        {
            get { return _dockMinHeight; }
            set
            {
                if (_dockMinHeight.NotEqualEx(value))
                {
                    MathHelper.AssertIsPositiveOrZero(value);
                    RaisePropertyChanging("DockMinHeight");
                    _dockMinHeight = value;
                    RaisePropertyChanged("DockMinHeight");
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
                    _isMaximized = value;
                    RaisePropertyChanged("IsMaximized");
                }
            }
        }

        double ILayoutPositionableElementWithActualSize.ActualWidth
        {
            get { return _actualWidth; }
            set { _actualWidth = value; }
        }

        double ILayoutPositionableElementWithActualSize.ActualHeight
        {
            get { return _actualHeight; }
            set { _actualHeight = value; }
        }

        #endregion

        #region Конструктор

        static LayoutPositionableGroup()
        {
            _gridLengthConverter = new GridLengthConverter();
        }

        protected LayoutPositionableGroup()
        {
            _dockWidth = new GridLength(1.0, GridUnitType.Star);
            _dockMinWidth = 25.0;
            _dockMinHeight = 25.0;
            _dockHeight = new GridLength(1.0, GridUnitType.Star);
        }

        #endregion

        #region Методы

        protected virtual void OnDockWidthChanged()
        {
        }

        protected virtual void OnDockHeightChanged()
        {
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (DockWidth.Value.NotEqualEx(1.0) || !DockWidth.IsStar)
            {
                writer.WriteAttributeString("DockWidth", _gridLengthConverter.ConvertToInvariantString(DockWidth));
            }
            if (DockHeight.Value.NotEqualEx(1.0) || !DockHeight.IsStar)
            {
                writer.WriteAttributeString("DockHeight", _gridLengthConverter.ConvertToInvariantString(DockHeight));
            }

            if (DockMinWidth.NotEqualEx(25.0))
            {
                writer.WriteAttributeString("DocMinWidth", DockMinWidth.ToString(CultureInfo.InvariantCulture));
            }
            if (DockMinHeight.NotEqualEx(25.0))
            {
                writer.WriteAttributeString("DockMinHeight", DockMinHeight.ToString(CultureInfo.InvariantCulture));
            }

            if (FloatingWidth.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingWidth", FloatingWidth.ToString(CultureInfo.InvariantCulture));
            }
            if (FloatingHeight.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingHeight", FloatingHeight.ToString(CultureInfo.InvariantCulture));
            }
            if (FloatingLeft.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingLeft", FloatingLeft.ToString(CultureInfo.InvariantCulture));
            }
            if (FloatingTop.NotZeroEx())
            {
                writer.WriteAttributeString("FloatingTop", FloatingTop.ToString(CultureInfo.InvariantCulture));
            }
            if (IsMaximized)
            {
                writer.WriteAttributeString("IsMaximized", IsMaximized.ToString());
            }

            base.WriteXml(writer);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("DockWidth"))
            {
                _dockWidth = (GridLength)_gridLengthConverter.ConvertFromInvariantString(reader.Value);
            }
            if (reader.MoveToAttribute("DockHeight"))
            {
                _dockHeight = (GridLength)_gridLengthConverter.ConvertFromInvariantString(reader.Value);
            }

            if (reader.MoveToAttribute("DocMinWidth"))
            {
                _dockMinWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("DocMinHeight"))
            {
                _dockMinHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }

            if (reader.MoveToAttribute("FloatingWidth"))
            {
                _floatingWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("FloatingHeight"))
            {
                _floatingHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("FloatingLeft"))
            {
                _floatingLeft = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("FloatingTop"))
            {
                _floatingTop = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("IsMaximized"))
            {
                _isMaximized = bool.Parse(reader.Value);
            }

            base.ReadXml(reader);
        }

        #endregion
    }
}