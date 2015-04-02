using System;
using System.Windows;

using SharpLib.Texter.Editing;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.CodeCompletion
{
    public class InsightWindow : CompletionWindowBase
    {
        #region Свойства

        public bool CloseAutomatically { get; set; }

        protected override bool CloseOnFocusLost
        {
            get { return CloseAutomatically; }
        }

        #endregion

        #region Конструктор

        static InsightWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InsightWindow),
                new FrameworkPropertyMetadata(typeof(InsightWindow)));
            AllowsTransparencyProperty.OverrideMetadata(typeof(InsightWindow),
                new FrameworkPropertyMetadata(Boxes.True));
        }

        public InsightWindow(TextArea textArea)
            : base(textArea)
        {
            CloseAutomatically = true;
            AttachEvents();
        }

        #endregion

        #region Методы

        protected override void OnSourceInitialized(EventArgs e)
        {
            var caret = TextArea.Caret.CalculateCaretRectangle();
            var pointOnScreen = TextArea.TextView.PointToScreen(caret.Location - TextArea.TextView.ScrollOffset);
            var workingArea = System.Windows.Forms.Screen.FromPoint(pointOnScreen.ToSystemDrawing()).WorkingArea.ToWpf().TransformFromDevice(this);

            MaxHeight = workingArea.Height;
            MaxWidth = Math.Min(workingArea.Width, Math.Max(1000, workingArea.Width * 0.6));

            base.OnSourceInitialized(e);
        }

        private void AttachEvents()
        {
            TextArea.Caret.PositionChanged += CaretPositionChanged;
        }

        protected override void DetachEvents()
        {
            TextArea.Caret.PositionChanged -= CaretPositionChanged;
            base.DetachEvents();
        }

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            if (CloseAutomatically)
            {
                int offset = TextArea.Caret.Offset;
                if (offset < StartOffset || offset > EndOffset)
                {
                    Close();
                }
            }
        }

        #endregion
    }
}