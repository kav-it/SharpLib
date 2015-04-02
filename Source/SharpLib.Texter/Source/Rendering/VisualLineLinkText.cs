using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;

namespace SharpLib.Texter.Rendering
{
    public class VisualLineLinkText : VisualLineText
    {
        #region Свойства

        public Uri NavigateUri { get; set; }

        public string TargetName { get; set; }

        public bool RequireControlModifierForClick { get; set; }

        #endregion

        #region Конструктор

        public VisualLineLinkText(VisualLine parentVisualLine, int length)
            : base(parentVisualLine, length)
        {
            RequireControlModifierForClick = true;
        }

        #endregion

        #region Методы

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
            TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
            if (context.TextView.LinkTextUnderline)
            {
                TextRunProperties.SetTextDecorations(TextDecorations.Underline);
            }
            return base.CreateTextRun(startVisualColumn, context);
        }

        protected bool LinkIsClickable()
        {
            if (NavigateUri == null)
            {
                return false;
            }
            if (RequireControlModifierForClick)
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
            return true;
        }

        protected internal override void OnQueryCursor(QueryCursorEventArgs e)
        {
            if (LinkIsClickable())
            {
                e.Handled = true;
                e.Cursor = Cursors.Hand;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "I've seen Process.Start throw undocumented exceptions when the mail client / web browser is installed incorrectly")]
        protected internal override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !e.Handled && LinkIsClickable())
            {
                var args = new RequestNavigateEventArgs(NavigateUri, TargetName);
                args.RoutedEvent = Hyperlink.RequestNavigateEvent;
                var element = e.Source as FrameworkElement;
                if (element != null)
                {
                    element.RaiseEvent(args);
                }
                if (!args.Handled)
                {
                    try
                    {
                        Process.Start(NavigateUri.ToString());
                    }
                    catch
                    {
                    }
                }
                e.Handled = true;
            }
        }

        protected override VisualLineText CreateInstance(int length)
        {
            return new VisualLineLinkText(ParentVisualLine, length)
            {
                NavigateUri = NavigateUri,
                TargetName = TargetName,
                RequireControlModifierForClick = RequireControlModifierForClick
            };
        }

        #endregion
    }
}