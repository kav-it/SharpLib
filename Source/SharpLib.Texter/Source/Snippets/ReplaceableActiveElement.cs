using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using SharpLib.Texter.Document;
using SharpLib.Texter.Rendering;

namespace SharpLib.Texter.Snippets
{
    internal sealed class ReplaceableActiveElement : IReplaceableActiveElement, IWeakEventListener
    {
        #region Поля

        private readonly InsertionContext context;

        private readonly int endOffset;

        private readonly int startOffset;

        private Renderer background;

        private TextAnchor end;

        private Renderer foreground;

        private bool isCaretInside;

        private TextAnchor start;

        #endregion

        #region Свойства

        public string Text { get; private set; }

        public bool IsEditable
        {
            get { return true; }
        }

        public ISegment Segment
        {
            get
            {
                if (start.IsDeleted || end.IsDeleted)
                {
                    return null;
                }
                return new SimpleSegment(start.Offset, Math.Max(0, end.Offset - start.Offset));
            }
        }

        #endregion

        #region События

        public event EventHandler TextChanged;

        #endregion

        #region Конструктор

        public ReplaceableActiveElement(InsertionContext context, int startOffset, int endOffset)
        {
            this.context = context;
            this.startOffset = startOffset;
            this.endOffset = endOffset;
        }

        #endregion

        #region Методы

        private void AnchorDeleted(object sender, EventArgs e)
        {
            context.Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
        }

        public void OnInsertionCompleted()
        {
            start = context.Document.CreateAnchor(startOffset);
            start.MovementType = AnchorMovementType.BeforeInsertion;
            end = context.Document.CreateAnchor(endOffset);
            end.MovementType = AnchorMovementType.AfterInsertion;
            start.Deleted += AnchorDeleted;
            end.Deleted += AnchorDeleted;

            TextDocumentWeakEventManager.TextChanged.AddListener(context.Document, this);

            background = new Renderer
            {
                Layer = KnownLayer.Background,
                element = this
            };
            foreground = new Renderer
            {
                Layer = KnownLayer.Text,
                element = this
            };
            context.TextArea.TextView.BackgroundRenderers.Add(background);
            context.TextArea.TextView.BackgroundRenderers.Add(foreground);
            context.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            Caret_PositionChanged(null, null);

            Text = GetText();
        }

        public void Deactivate(SnippetEventArgs e)
        {
            TextDocumentWeakEventManager.TextChanged.RemoveListener(context.Document, this);
            context.TextArea.TextView.BackgroundRenderers.Remove(background);
            context.TextArea.TextView.BackgroundRenderers.Remove(foreground);
            context.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            var s = Segment;
            if (s != null)
            {
                bool newIsCaretInside = s.Contains(context.TextArea.Caret.Offset, 0);
                if (newIsCaretInside != isCaretInside)
                {
                    isCaretInside = newIsCaretInside;
                    context.TextArea.TextView.InvalidateLayer(foreground.Layer);
                }
            }
        }

        private string GetText()
        {
            if (start.IsDeleted || end.IsDeleted)
            {
                return string.Empty;
            }
            return context.Document.GetText(start.Offset, Math.Max(0, end.Offset - start.Offset));
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextDocumentWeakEventManager.TextChanged))
            {
                string newText = GetText();
                if (Text != newText)
                {
                    Text = newText;
                    if (TextChanged != null)
                    {
                        TextChanged(this, e);
                    }
                }
                return true;
            }
            return false;
        }

        #endregion

        #region Вложенный класс: Renderer

        private sealed class Renderer : IBackgroundRenderer
        {
            #region Поля

            private static readonly Pen activeBorderPen = CreateBorderPen();

            private static readonly Brush backgroundBrush = CreateBackgroundBrush();

            internal ReplaceableActiveElement element;

            #endregion

            #region Свойства

            public KnownLayer Layer { get; set; }

            #endregion

            #region Методы

            private static Brush CreateBackgroundBrush()
            {
                var b = new SolidColorBrush(Colors.LimeGreen);
                b.Opacity = 0.4;
                b.Freeze();
                return b;
            }

            private static Pen CreateBorderPen()
            {
                var p = new Pen(Brushes.Black, 1);
                p.DashStyle = DashStyles.Dot;
                p.Freeze();
                return p;
            }

            public void Draw(TextView textView, System.Windows.Media.DrawingContext drawingContext)
            {
                var s = element.Segment;
                if (s != null)
                {
                    var geoBuilder = new BackgroundGeometryBuilder();
                    geoBuilder.AlignToMiddleOfPixels = true;
                    if (Layer == KnownLayer.Background)
                    {
                        geoBuilder.AddSegment(textView, s);
                        drawingContext.DrawGeometry(backgroundBrush, null, geoBuilder.CreateGeometry());
                    }
                    else
                    {
                        if (element.isCaretInside)
                        {
                            geoBuilder.AddSegment(textView, s);
                            foreach (BoundActiveElement boundElement in element.context.ActiveElements.OfType<BoundActiveElement>())
                            {
                                if (boundElement.targetElement == element)
                                {
                                    geoBuilder.AddSegment(textView, boundElement.Segment);
                                    geoBuilder.CloseFigure();
                                }
                            }
                            drawingContext.DrawGeometry(null, activeBorderPen, geoBuilder.CreateGeometry());
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}