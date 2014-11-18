using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SharpLibWpf.Controls
{
    [TemplatePart(Name = "FlipButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "FlipButtonAlternate", Type = typeof(ToggleButton))]
    [TemplateVisualState(Name = "Normal", GroupName = "ViewStates")]
    [TemplateVisualState(Name = "Flipped", GroupName = "ViewStates")]
    public class FlipPanel : Control
    {
        #region Поля

        public static readonly DependencyProperty BackContentProperty;

        public static readonly DependencyProperty CornerRadiusProperty;

        public static readonly DependencyProperty FrontContentProperty;

        public static readonly DependencyProperty IsFlippedProperty;

        #endregion

        #region Свойства

        public object FrontContent
        {
            get { return GetValue(FrontContentProperty); }
            set { SetValue(FrontContentProperty, value); }
        }

        public object BackContent
        {
            get { return GetValue(BackContentProperty); }
            set { SetValue(BackContentProperty, value); }
        }

        public bool IsFlipped
        {
            get { return (bool)GetValue(IsFlippedProperty); }
            set
            {
                SetValue(IsFlippedProperty, value);

                ChangeVisualState(true);
            }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #region Конструктор

        static FlipPanel()
        {
            FrontContentProperty = DependencyProperty.Register("FrontContent", typeof(object), typeof(FlipPanel), null);
            BackContentProperty = DependencyProperty.Register("BackContent", typeof(object), typeof(FlipPanel), null);
            IsFlippedProperty = DependencyProperty.Register("IsFlipped", typeof(bool), typeof(FlipPanel), null);
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FlipPanel), null);

            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipPanel), new FrameworkPropertyMetadata(typeof(FlipPanel)));
        }

        #endregion

        #region Методы

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ToggleButton flipButton = GetTemplateChild("FlipButton") as ToggleButton;
            if (flipButton != null)
            {
                flipButton.Click += flipButton_Click;
            }

            var flipButtonAlternate = GetTemplateChild("FlipButtonAlternate") as ToggleButton;
            if (flipButtonAlternate != null)
            {
                flipButtonAlternate.Click += flipButton_Click;
            }

            ChangeVisualState(false);
        }

        private void flipButton_Click(object sender, RoutedEventArgs e)
        {
            IsFlipped = !IsFlipped;
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, !IsFlipped ? "Normal" : "Flipped", useTransitions);

            var front = FrontContent as UIElement;
            if (front != null)
            {
                front.Visibility = IsFlipped ? Visibility.Hidden : Visibility.Visible;
            }

            var back = BackContent as UIElement;
            if (back == null)
            {
                return;
            }

            back.Visibility = IsFlipped ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion
    }
}