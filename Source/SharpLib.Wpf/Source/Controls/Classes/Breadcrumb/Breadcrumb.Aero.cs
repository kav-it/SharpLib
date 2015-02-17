using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    public class BreadcrumbAero : ContentControl
    {
        #region Поля

        public static readonly DependencyProperty MouseOverBackgroundProperty;

        public static readonly DependencyProperty MousePressedBackgroundProperty;

        public static readonly DependencyProperty RenderMouseOverProperty;

        public static readonly DependencyProperty RenderPressedProperty;

        #endregion

        #region Свойства

        public bool RenderPressed
        {
            get { return (bool)GetValue(RenderPressedProperty); }
            set { SetValue(RenderPressedProperty, value); }
        }

        public bool RenderMouseOver
        {
            get { return (bool)GetValue(RenderMouseOverProperty); }
            set { SetValue(RenderMouseOverProperty, value); }
        }

        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public Brush MousePressedBackground
        {
            get { return (Brush)GetValue(MousePressedBackgroundProperty); }
            set { SetValue(MousePressedBackgroundProperty, value); }
        }

        #endregion

        #region Конструктор

        static BreadcrumbAero()
        {
            MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(BreadcrumbAero), new UIPropertyMetadata(null));
            MousePressedBackgroundProperty = DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(BreadcrumbAero), new UIPropertyMetadata(null));
            RenderMouseOverProperty = DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(BreadcrumbAero), new UIPropertyMetadata(false));
            RenderPressedProperty = DependencyProperty.Register("RenderPressed", typeof(bool), typeof(BreadcrumbAero), new UIPropertyMetadata(false));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbAero), new FrameworkPropertyMetadata(typeof(BreadcrumbAero)));
        }

        #endregion
    }
}