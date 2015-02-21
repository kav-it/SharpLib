using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;

using Standard;

namespace Microsoft.Windows.Shell
{
    public class WindowChrome : Freezable
    {
        #region Поля

        public static readonly DependencyProperty CaptionHeightProperty;

        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty GlassFrameThicknessProperty;

        public static readonly DependencyProperty IsHitTestVisibleInChromeProperty;

        public static readonly DependencyProperty ResizeBorderThicknessProperty;

        public static readonly DependencyProperty WindowChromeProperty;

        private static readonly List<SystemParameterBoundProperty> _boundProperties;

        #endregion

        #region Свойства

        public static Thickness GlassFrameCompleteThickness
        {
            get { return new Thickness(-1); }
        }

        public double CaptionHeight
        {
            get { return (double)GetValue(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }

        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }

        public Thickness GlassFrameThickness
        {
            get { return (Thickness)GetValue(GlassFrameThicknessProperty); }
            set { SetValue(GlassFrameThicknessProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public bool ShowSystemMenu { get; set; }

        #endregion

        #region События

        internal event EventHandler PropertyChangedThatRequiresRepaint;

        #endregion

        #region Конструктор

        static WindowChrome()
        {
            CaptionHeightProperty = DependencyProperty.Register("CaptionHeight", typeof(double), typeof(WindowChrome),
                new PropertyMetadata(0d, (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()), value => (double)value >= 0d);
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(WindowChrome),
                new PropertyMetadata(default(CornerRadius), (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint()), value => Utility.IsCornerRadiusValid((CornerRadius)value));
            GlassFrameThicknessProperty = DependencyProperty.Register("GlassFrameThickness", typeof(Thickness), typeof(WindowChrome),
                new PropertyMetadata(default(Thickness), (d, e) => ((WindowChrome)d)._OnPropertyChangedThatRequiresRepaint(), (d, o) => _CoerceGlassFrameThickness((Thickness)o)));
            IsHitTestVisibleInChromeProperty = DependencyProperty.RegisterAttached("IsHitTestVisibleInChrome", typeof(bool), typeof(WindowChrome),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
            ResizeBorderThicknessProperty = DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(WindowChrome), new PropertyMetadata(default(Thickness)),
                value => Utility.IsThicknessNonNegative((Thickness)value));
            WindowChromeProperty = DependencyProperty.RegisterAttached("WindowChrome", typeof(WindowChrome), typeof(WindowChrome), new PropertyMetadata(null, _OnChromeChanged));

            _boundProperties = new List<SystemParameterBoundProperty>
            {
                new SystemParameterBoundProperty
                {
                    DependencyProperty = CornerRadiusProperty,
                    SystemParameterPropertyName = "WindowCornerRadius"
                },
                new SystemParameterBoundProperty
                {
                    DependencyProperty = CaptionHeightProperty,
                    SystemParameterPropertyName = "WindowCaptionHeight"
                },
                new SystemParameterBoundProperty
                {
                    DependencyProperty = ResizeBorderThicknessProperty,
                    SystemParameterPropertyName = "WindowResizeBorderThickness"
                },
                new SystemParameterBoundProperty
                {
                    DependencyProperty = GlassFrameThicknessProperty,
                    SystemParameterPropertyName = "WindowNonClientFrameThickness"
                },
            };
        }

        public WindowChrome()
        {
            foreach (var bp in _boundProperties)
            {
                Assert.IsNotNull(bp.DependencyProperty);
                BindingOperations.SetBinding(
                    this,
                    bp.DependencyProperty,
                    new Binding
                    {
                        Source = SystemParameters2.Current,
                        Path = new PropertyPath(bp.SystemParameterPropertyName),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    });
            }
        }

        #endregion

        #region Методы

        private static void _OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            var window = (Window)d;
            var newChrome = (WindowChrome)e.NewValue;

            Assert.IsNotNull(window);

            var chromeWorker = WindowChromeWorker.GetWindowChromeWorker(window);
            if (chromeWorker == null)
            {
                chromeWorker = new WindowChromeWorker();
                WindowChromeWorker.SetWindowChromeWorker(window, chromeWorker);
            }

            chromeWorker.SetWindowChrome(newChrome);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static WindowChrome GetWindowChrome(Window window)
        {
            Verify.IsNotNull(window, "window");
            return (WindowChrome)window.GetValue(WindowChromeProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetWindowChrome(Window window, WindowChrome chrome)
        {
            Verify.IsNotNull(window, "window");
            window.SetValue(WindowChromeProperty, chrome);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            return (bool)dobj.GetValue(IsHitTestVisibleInChromeProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
        {
            Verify.IsNotNull(inputElement, "inputElement");
            var dobj = inputElement as DependencyObject;
            if (dobj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject", "inputElement");
            }
            dobj.SetValue(IsHitTestVisibleInChromeProperty, hitTestVisible);
        }

        private static object _CoerceGlassFrameThickness(Thickness thickness)
        {
            if (!Utility.IsThicknessNonNegative(thickness))
            {
                return GlassFrameCompleteThickness;
            }

            return thickness;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new WindowChrome();
        }

        private void _OnPropertyChangedThatRequiresRepaint()
        {
            var handler = PropertyChangedThatRequiresRepaint;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Вложенный класс: _SystemParameterBoundProperty

        private struct SystemParameterBoundProperty
        {
            #region Свойства

            public string SystemParameterPropertyName { get; set; }

            public DependencyProperty DependencyProperty { get; set; }

            #endregion
        }

        #endregion
    }
}