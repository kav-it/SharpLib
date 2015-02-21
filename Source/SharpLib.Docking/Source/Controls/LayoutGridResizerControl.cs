using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SharpLib.Docking.Controls
{
    public class LayoutGridResizerControl : Thumb
    {
        #region Поля

        public static readonly DependencyProperty BackgroundWhileDraggingProperty;

        public static readonly DependencyProperty OpacityWhileDraggingProperty;

        #endregion

        #region Свойства

        public Brush BackgroundWhileDragging
        {
            get { return (Brush)GetValue(BackgroundWhileDraggingProperty); }
            set { SetValue(BackgroundWhileDraggingProperty, value); }
        }

        public double OpacityWhileDragging
        {
            get { return (double)GetValue(OpacityWhileDraggingProperty); }
            set { SetValue(OpacityWhileDraggingProperty, value); }
        }

        #endregion

        #region Конструктор

        static LayoutGridResizerControl()
        {
            BackgroundWhileDraggingProperty = DependencyProperty.Register("BackgroundWhileDragging", typeof(Brush), typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(Brushes.Black));
            OpacityWhileDraggingProperty = DependencyProperty.Register("OpacityWhileDragging", typeof(double), typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(0.5));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(typeof(LayoutGridResizerControl)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            VerticalAlignmentProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            BackgroundProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(Brushes.Transparent));
            IsHitTestVisibleProperty.OverrideMetadata(typeof(LayoutGridResizerControl), new FrameworkPropertyMetadata(true, null));
        }

        #endregion
    }
}