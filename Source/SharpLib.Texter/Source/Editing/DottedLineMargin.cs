using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SharpLib.Texter.Editing
{
    public static class DottedLineMargin
    {
        #region Поля

        private static readonly object tag = new object();

        #endregion

        #region Методы

        public static UIElement Create()
        {
            var line = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = 1,
                StrokeDashArray =
                {
                    0,
                    2
                },
                Stretch = Stretch.Fill,
                StrokeThickness = 1,
                StrokeDashCap = PenLineCap.Round,
                Margin = new Thickness(2, 0, 2, 0),
                Tag = tag
            };

            return line;
        }

        [Obsolete("This method got published accidentally; and will be removed again in a future version. Use the parameterless overload instead.")]
        public static UIElement Create(TextEditor editor)
        {
            var line = (Line)Create();

            line.SetBinding(
                Shape.StrokeProperty,
                new Binding("LineNumbersForeground")
                {
                    Source = editor
                }
                );

            return line;
        }

        public static bool IsDottedLineMargin(UIElement element)
        {
            var l = element as Line;
            return l != null && l.Tag == tag;
        }

        #endregion
    }
}