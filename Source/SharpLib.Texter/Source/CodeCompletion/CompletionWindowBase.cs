using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

using SharpLib.Texter.Document;
using SharpLib.Texter.Editing;
using SharpLib.Texter.Rendering;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.CodeCompletion
{
    public class CompletionWindowBase : Window
    {
        #region Поля

        private readonly Window parentWindow;

        private TextDocument document;

        private InputHandler myInputHandler;

        private bool sourceIsInitialized;

        private Point visualLocation, visualLocationTop;

        #endregion

        #region Свойства

        public TextArea TextArea { get; private set; }

        public int StartOffset { get; set; }

        public int EndOffset { get; set; }

        protected bool IsUp { get; private set; }

        protected virtual bool CloseOnFocusLost
        {
            get { return true; }
        }

        private bool IsTextAreaFocused
        {
            get
            {
                if (parentWindow != null && !parentWindow.IsActive)
                {
                    return false;
                }
                return TextArea.IsKeyboardFocused;
            }
        }

        public bool ExpectInsertionBeforeStart { get; set; }

        #endregion

        #region Конструктор

        static CompletionWindowBase()
        {
            WindowStyleProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(WindowStyle.None));
            ShowActivatedProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));
            ShowInTaskbarProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));
        }

        public CompletionWindowBase(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            TextArea = textArea;
            parentWindow = Window.GetWindow(textArea);
            Owner = parentWindow;
            AddHandler(MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);

            StartOffset = EndOffset = TextArea.Caret.Offset;

            AttachEvents();
        }

        #endregion

        #region Методы

        private void AttachEvents()
        {
            document = TextArea.Document;
            if (document != null)
            {
                document.Changing += textArea_Document_Changing;
            }

            TextArea.LostKeyboardFocus += TextAreaLostFocus;
            TextArea.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
            TextArea.DocumentChanged += TextAreaDocumentChanged;
            if (parentWindow != null)
            {
                parentWindow.LocationChanged += parentWindow_LocationChanged;
            }

            foreach (InputHandler x in TextArea.StackedInputHandlers.OfType<InputHandler>())
            {
                if (x.window.GetType() == GetType())
                {
                    TextArea.PopStackedInputHandler(x);
                }
            }

            myInputHandler = new InputHandler(this);
            TextArea.PushStackedInputHandler(myInputHandler);
        }

        protected virtual void DetachEvents()
        {
            if (document != null)
            {
                document.Changing -= textArea_Document_Changing;
            }
            TextArea.LostKeyboardFocus -= TextAreaLostFocus;
            TextArea.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
            TextArea.DocumentChanged -= TextAreaDocumentChanged;
            if (parentWindow != null)
            {
                parentWindow.LocationChanged -= parentWindow_LocationChanged;
            }
            TextArea.PopStackedInputHandler(myInputHandler);
        }

        private void TextViewScrollOffsetChanged(object sender, EventArgs e)
        {
            if (!sourceIsInitialized)
            {
                return;
            }

            IScrollInfo scrollInfo = TextArea.TextView;
            var visibleRect = new Rect(scrollInfo.HorizontalOffset, scrollInfo.VerticalOffset, scrollInfo.ViewportWidth, scrollInfo.ViewportHeight);

            if (visibleRect.Contains(visualLocation) || visibleRect.Contains(visualLocationTop))
            {
                UpdatePosition();
            }
            else
            {
                Close();
            }
        }

        private void TextAreaDocumentChanged(object sender, EventArgs e)
        {
            Close();
        }

        private void TextAreaLostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
        }

        private void parentWindow_LocationChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected static bool RaiseEventPair(UIElement target, RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (previewEvent == null)
            {
                throw new ArgumentNullException("previewEvent");
            }
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            args.RoutedEvent = previewEvent;
            target.RaiseEvent(args);
            args.RoutedEvent = @event;
            target.RaiseEvent(args);
            return args.Handled;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ActivateParentWindow();
        }

        protected virtual void ActivateParentWindow()
        {
            if (parentWindow != null)
            {
                parentWindow.Activate();
            }
        }

        private void CloseIfFocusLost()
        {
            if (CloseOnFocusLost)
            {
                Debug.WriteLine("CloseIfFocusLost: this.IsActive=" + IsActive + " IsTextAreaFocused=" + IsTextAreaFocused);
                if (!IsActive && !IsTextAreaFocused)
                {
                    Close();
                }
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (document != null && StartOffset != TextArea.Caret.Offset)
            {
                SetPosition(new TextViewPosition(document.GetLocation(StartOffset)));
            }
            else
            {
                SetPosition(TextArea.Caret.Position);
            }
            sourceIsInitialized = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DetachEvents();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled && e.Key == Key.Escape)
            {
                e.Handled = true;
                Close();
            }
        }

        protected void SetPosition(TextViewPosition position)
        {
            var textView = TextArea.TextView;

            visualLocation = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
            visualLocationTop = textView.GetVisualPosition(position, VisualYPosition.LineTop);
            UpdatePosition();
        }

        protected void UpdatePosition()
        {
            var textView = TextArea.TextView;

            var location = textView.PointToScreen(visualLocation - textView.ScrollOffset);
            var locationTop = textView.PointToScreen(visualLocationTop - textView.ScrollOffset);

            var completionWindowSize = new Size(ActualWidth, ActualHeight).TransformToDevice(textView);
            var bounds = new Rect(location, completionWindowSize);
            var workingScreen = System.Windows.Forms.Screen.GetWorkingArea(location.ToSystemDrawing()).ToWpf();
            if (!workingScreen.Contains(bounds))
            {
                if (bounds.Left < workingScreen.Left)
                {
                    bounds.X = workingScreen.Left;
                }
                else if (bounds.Right > workingScreen.Right)
                {
                    bounds.X = workingScreen.Right - bounds.Width;
                }
                if (bounds.Bottom > workingScreen.Bottom)
                {
                    bounds.Y = locationTop.Y - bounds.Height;
                    IsUp = true;
                }
                else
                {
                    IsUp = false;
                }
                if (bounds.Y < workingScreen.Top)
                {
                    bounds.Y = workingScreen.Top;
                }
            }

            bounds = bounds.TransformFromDevice(textView);
            Left = bounds.X;
            Top = bounds.Y;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (sizeInfo.HeightChanged && IsUp)
            {
                Top += sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height;
            }
        }

        private void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
        {
            if (e.Offset + e.RemovalLength == StartOffset && e.RemovalLength > 0)
            {
                Close();
            }
            if (e.Offset == StartOffset && e.RemovalLength == 0 && ExpectInsertionBeforeStart)
            {
                StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.AfterInsertion);
                ExpectInsertionBeforeStart = false;
            }
            else
            {
                StartOffset = e.GetNewOffset(StartOffset, AnchorMovementType.BeforeInsertion);
            }
            EndOffset = e.GetNewOffset(EndOffset, AnchorMovementType.AfterInsertion);
        }

        #endregion

        #region Вложенный класс: InputHandler

        private sealed class InputHandler : TextAreaStackedInputHandler
        {
            #region Константы

            private const Key KeyDeadCharProcessed = (Key)0xac;

            #endregion

            #region Поля

            internal readonly CompletionWindowBase window;

            #endregion

            #region Конструктор

            public InputHandler(CompletionWindowBase window)
                : base(window.TextArea)
            {
                Debug.Assert(window != null);
                this.window = window;
            }

            #endregion

            #region Методы

            public override void Detach()
            {
                base.Detach();
                window.Close();
            }

            public override void OnPreviewKeyDown(KeyEventArgs e)
            {
                if (e.Key == KeyDeadCharProcessed)
                {
                    return;
                }
                e.Handled = RaiseEventPair(window, PreviewKeyDownEvent, KeyDownEvent,
                    new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
            }

            public override void OnPreviewKeyUp(KeyEventArgs e)
            {
                if (e.Key == KeyDeadCharProcessed)
                {
                    return;
                }
                e.Handled = RaiseEventPair(window, PreviewKeyUpEvent, KeyUpEvent,
                    new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
            }

            #endregion
        }

        #endregion
    }
}