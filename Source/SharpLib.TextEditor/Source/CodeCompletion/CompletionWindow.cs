using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
    public class CompletionWindow : CompletionWindowBase
    {
        #region Поля

        private readonly CompletionList completionList = new CompletionList();

        private ToolTip toolTip = new ToolTip();

        #endregion

        #region Свойства

        public CompletionList CompletionList
        {
            get { return completionList; }
        }

        public bool CloseAutomatically { get; set; }

        protected override bool CloseOnFocusLost
        {
            get { return CloseAutomatically; }
        }

        public bool CloseWhenCaretAtBeginning { get; set; }

        #endregion

        #region Конструктор

        public CompletionWindow(TextArea textArea)
            : base(textArea)
        {
            CloseAutomatically = true;
            SizeToContent = SizeToContent.Height;
            MaxHeight = 300;
            Width = 175;
            Content = completionList;

            MinHeight = 15;
            MinWidth = 30;

            toolTip.PlacementTarget = this;
            toolTip.Placement = PlacementMode.Right;
            toolTip.Closed += toolTip_Closed;

            AttachEvents();
        }

        #endregion

        #region Методы

        private void toolTip_Closed(object sender, RoutedEventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.Content = null;
            }
        }

        private void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = completionList.SelectedItem;
            if (item == null)
            {
                return;
            }
            var description = item.Description;
            if (description != null)
            {
                string descriptionText = description as string;
                if (descriptionText != null)
                {
                    toolTip.Content = new TextBlock
                    {
                        Text = descriptionText,
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    toolTip.Content = description;
                }
                toolTip.IsOpen = true;
            }
            else
            {
                toolTip.IsOpen = false;
            }
        }

        private void completionList_InsertionRequested(object sender, EventArgs e)
        {
            Close();

            var item = completionList.SelectedItem;
            if (item != null)
            {
                item.Complete(TextArea, new AnchorSegment(TextArea.Document, StartOffset, EndOffset - StartOffset), e);
            }
        }

        private void AttachEvents()
        {
            completionList.InsertionRequested += completionList_InsertionRequested;
            completionList.SelectionChanged += completionList_SelectionChanged;
            TextArea.Caret.PositionChanged += CaretPositionChanged;
            TextArea.MouseWheel += textArea_MouseWheel;
            TextArea.PreviewTextInput += textArea_PreviewTextInput;
        }

        protected override void DetachEvents()
        {
            completionList.InsertionRequested -= completionList_InsertionRequested;
            completionList.SelectionChanged -= completionList_SelectionChanged;
            TextArea.Caret.PositionChanged -= CaretPositionChanged;
            TextArea.MouseWheel -= textArea_MouseWheel;
            TextArea.PreviewTextInput -= textArea_PreviewTextInput;
            base.DetachEvents();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                toolTip = null;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                completionList.HandleKey(e);
            }
        }

        private void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
                new TextCompositionEventArgs(e.Device, e.TextComposition));
        }

        private void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = RaiseEventPair(GetScrollEventTarget(),
                PreviewMouseWheelEvent, MouseWheelEvent,
                new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
        }

        private UIElement GetScrollEventTarget()
        {
            if (completionList == null)
            {
                return this;
            }
            return completionList.ScrollViewer ?? completionList.ListBox ?? (UIElement)completionList;
        }

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            int offset = TextArea.Caret.Offset;
            if (offset == StartOffset)
            {
                if (CloseAutomatically && CloseWhenCaretAtBeginning)
                {
                    Close();
                }
                else
                {
                    completionList.SelectItem(string.Empty);
                }
                return;
            }
            if (offset < StartOffset || offset > EndOffset)
            {
                if (CloseAutomatically)
                {
                    Close();
                }
            }
            else
            {
                var document = TextArea.Document;
                if (document != null)
                {
                    completionList.SelectItem(document.GetText(StartOffset, offset - StartOffset));
                }
            }
        }

        #endregion
    }
}