using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using SharpLib.Notepad.Rendering;
using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Editing
{
    internal sealed class CaretLayer : Layer
    {
        #region Поля

        private readonly DispatcherTimer caretBlinkTimer = new DispatcherTimer();

        private readonly TextArea textArea;

        internal Brush CaretBrush;

        private bool blink;

        private Rect caretRectangle;

        private bool isVisible;

        #endregion

        #region Конструктор

        public CaretLayer(TextArea textArea)
            : base(textArea.TextView, KnownLayer.Caret)
        {
            this.textArea = textArea;
            IsHitTestVisible = false;
            caretBlinkTimer.Tick += caretBlinkTimer_Tick;
        }

        #endregion

        #region Методы

        private void caretBlinkTimer_Tick(object sender, EventArgs e)
        {
            blink = !blink;
            InvalidateVisual();
        }

        public void Show(Rect caretRectangle)
        {
            this.caretRectangle = caretRectangle;
            isVisible = true;
            StartBlinkAnimation();
            InvalidateVisual();
        }

        public void Hide()
        {
            if (isVisible)
            {
                isVisible = false;
                StopBlinkAnimation();
                InvalidateVisual();
            }
        }

        private void StartBlinkAnimation()
        {
            var blinkTime = Win32.CaretBlinkTime;
            blink = true;

            if (blinkTime.TotalMilliseconds > 0)
            {
                caretBlinkTimer.Interval = blinkTime;
                caretBlinkTimer.Start();
            }
        }

        private void StopBlinkAnimation()
        {
            caretBlinkTimer.Stop();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (isVisible && blink)
            {
                var caretBrush = CaretBrush;
                if (caretBrush == null)
                {
                    caretBrush = (Brush)textView.GetValue(TextBlock.ForegroundProperty);
                }

                if (textArea.OverstrikeMode)
                {
                    var scBrush = caretBrush as SolidColorBrush;
                    if (scBrush != null)
                    {
                        var brushColor = scBrush.Color;
                        var newColor = Color.FromArgb(100, brushColor.R, brushColor.G, brushColor.B);
                        caretBrush = new SolidColorBrush(newColor);
                        caretBrush.Freeze();
                    }
                }

                var r = new Rect(caretRectangle.X - textView.HorizontalOffset,
                    caretRectangle.Y - textView.VerticalOffset,
                    caretRectangle.Width,
                    caretRectangle.Height);
                drawingContext.DrawRectangle(caretBrush, null, PixelSnapHelpers.Round(r, PixelSnapHelpers.GetPixelSize(this)));
            }
        }

        #endregion
    }
}