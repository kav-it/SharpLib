using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "The user usually doesn't work with TextView but with TextEditor; and nulling the Document property is sufficient to dispose everything.")]
    public class TextView : FrameworkElement, IScrollInfo, IWeakEventListener, ITextEditorComponent, IServiceProvider
    {
        #region Константы

        private const double AdditionalHorizontalScrollAmount = 3;

        #endregion

        #region Поля

        public static readonly DependencyProperty ColumnRulerPenProperty =
            DependencyProperty.Register("ColumnRulerBrush", typeof(Pen), typeof(TextView),
                new FrameworkPropertyMetadata(CreateFrozenPen(Brushes.LightGray)));

        public static readonly DependencyProperty CurrentLineBackgroundProperty =
            DependencyProperty.Register("CurrentLineBackground", typeof(Brush), typeof(TextView));

        public static readonly DependencyProperty CurrentLineBorderProperty =
            DependencyProperty.Register("CurrentLineBorder", typeof(Pen), typeof(TextView));

        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(TextDocument), typeof(TextView),
                new FrameworkPropertyMetadata(OnDocumentChanged));

        public static readonly DependencyProperty LinkTextBackgroundBrushProperty =
            DependencyProperty.Register("LinkTextBackgroundBrush", typeof(Brush), typeof(TextView),
                new FrameworkPropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty LinkTextForegroundBrushProperty =
            DependencyProperty.Register("LinkTextForegroundBrush", typeof(Brush), typeof(TextView),
                new FrameworkPropertyMetadata(Brushes.Blue));

        public static readonly DependencyProperty LinkTextUnderlineProperty =
            DependencyProperty.Register("LinkTextUnderline", typeof(bool), typeof(TextView),
                new FrameworkPropertyMetadata(true));

        public static readonly RoutedEvent MouseHoverEvent =
            EventManager.RegisterRoutedEvent("MouseHover", RoutingStrategy.Bubble,
                typeof(MouseEventHandler), typeof(TextView));

        public static readonly RoutedEvent MouseHoverStoppedEvent =
            EventManager.RegisterRoutedEvent("MouseHoverStopped", RoutingStrategy.Bubble,
                typeof(MouseEventHandler), typeof(TextView));

        public static readonly DependencyProperty NonPrintableCharacterBrushProperty =
            DependencyProperty.Register("NonPrintableCharacterBrush", typeof(Brush), typeof(TextView),
                new FrameworkPropertyMetadata(Brushes.LightGray));

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(TextEditorOptions), typeof(TextView),
                new FrameworkPropertyMetadata(OnOptionsChanged));

        public static readonly RoutedEvent PreviewMouseHoverEvent =
            EventManager.RegisterRoutedEvent("PreviewMouseHover", RoutingStrategy.Tunnel,
                typeof(MouseEventHandler), typeof(TextView));

        public static readonly RoutedEvent PreviewMouseHoverStoppedEvent =
            EventManager.RegisterRoutedEvent("PreviewMouseHoverStopped", RoutingStrategy.Tunnel,
                typeof(MouseEventHandler), typeof(TextView));

        [ThreadStatic]
        private static bool invalidCursor;

        private readonly ObserveAddRemoveCollection<IBackgroundRenderer> backgroundRenderers;

        private readonly ColumnRulerRenderer columnRulerRenderer;

        private readonly CurrentLineHighlightRenderer currentLineHighlighRenderer;

        private readonly ObserveAddRemoveCollection<VisualLineElementGenerator> elementGenerators;

        private readonly List<InlineObjectRun> inlineObjects = new List<InlineObjectRun>();

        private readonly LayerCollection layers;

        private readonly ObserveAddRemoveCollection<IVisualLineTransformer> lineTransformers;

        private readonly ServiceContainer services = new ServiceContainer();

        internal readonly TextLayer textLayer;

        private readonly List<VisualLine> visualLinesWithOutstandingInlineObjects = new List<VisualLine>();

        private List<VisualLine> allVisualLines = new List<VisualLine>();

        internal TextViewCachedElements cachedElements;

        private bool canHorizontallyScroll;

        private bool canVerticallyScroll;

        private double clippedPixelsOnTop;

        private double defaultBaseline;

        private double defaultLineHeight;

        private bool defaultTextMetricsValid;

        private TextDocument document;

        private TextFormatter formatter;

        private HeightTree heightTree;

        private MouseHoverLogic hoverLogic;

        private bool inMeasure;

        private DispatcherOperation invalidateMeasureOperation;

        private Size lastAvailableSize;

        private LinkElementGenerator linkElementGenerator;

        private MailLinkElementGenerator mailLinkElementGenerator;

        private List<VisualLine> newVisualLines;

        private Size scrollExtent;

        private Vector scrollOffset;

        private Size scrollViewport;

        private SingleCharacterElementGenerator singleCharacterElementGenerator;

        private ReadOnlyCollection<VisualLine> visibleVisualLines;

        private double wideSpaceWidth;

        #endregion

        #region Свойства

        public TextDocument Document
        {
            get { return (TextDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        internal double FontSize
        {
            get { return (double)GetValue(TextBlock.FontSizeProperty); }
        }

        public TextEditorOptions Options
        {
            get { return (TextEditorOptions)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public IList<VisualLineElementGenerator> ElementGenerators
        {
            get { return elementGenerators; }
        }

        public IList<IVisualLineTransformer> LineTransformers
        {
            get { return lineTransformers; }
        }

        public UIElementCollection Layers
        {
            get { return layers; }
        }

        protected override int VisualChildrenCount
        {
            get { return layers.Count + inlineObjects.Count; }
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get { return inlineObjects.Select(io => io.Element).Concat(layers.Cast<UIElement>()).GetEnumerator(); }
        }

        public Brush NonPrintableCharacterBrush
        {
            get { return (Brush)GetValue(NonPrintableCharacterBrushProperty); }
            set { SetValue(NonPrintableCharacterBrushProperty, value); }
        }

        public Brush LinkTextForegroundBrush
        {
            get { return (Brush)GetValue(LinkTextForegroundBrushProperty); }
            set { SetValue(LinkTextForegroundBrushProperty, value); }
        }

        public Brush LinkTextBackgroundBrush
        {
            get { return (Brush)GetValue(LinkTextBackgroundBrushProperty); }
            set { SetValue(LinkTextBackgroundBrushProperty, value); }
        }

        public bool LinkTextUnderline
        {
            get { return (bool)GetValue(LinkTextUnderlineProperty); }
            set { SetValue(LinkTextUnderlineProperty, value); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public ReadOnlyCollection<VisualLine> VisualLines
        {
            get
            {
                if (visibleVisualLines == null)
                {
                    throw new VisualLinesInvalidException();
                }
                return visibleVisualLines;
            }
        }

        public bool VisualLinesValid
        {
            get { return visibleVisualLines != null; }
        }

        public IList<IBackgroundRenderer> BackgroundRenderers
        {
            get { return backgroundRenderers; }
        }

        bool IScrollInfo.CanVerticallyScroll
        {
            get { return canVerticallyScroll; }
            set
            {
                if (canVerticallyScroll != value)
                {
                    canVerticallyScroll = value;
                    InvalidateMeasure(DispatcherPriority.Normal);
                }
            }
        }

        bool IScrollInfo.CanHorizontallyScroll
        {
            get { return canHorizontallyScroll; }
            set
            {
                if (canHorizontallyScroll != value)
                {
                    canHorizontallyScroll = value;
                    ClearVisualLines();
                    InvalidateMeasure(DispatcherPriority.Normal);
                }
            }
        }

        double IScrollInfo.ExtentWidth
        {
            get { return scrollExtent.Width; }
        }

        double IScrollInfo.ExtentHeight
        {
            get { return scrollExtent.Height; }
        }

        double IScrollInfo.ViewportWidth
        {
            get { return scrollViewport.Width; }
        }

        double IScrollInfo.ViewportHeight
        {
            get { return scrollViewport.Height; }
        }

        public double HorizontalOffset
        {
            get { return scrollOffset.X; }
        }

        public double VerticalOffset
        {
            get { return scrollOffset.Y; }
        }

        public Vector ScrollOffset
        {
            get { return scrollOffset; }
        }

        ScrollViewer IScrollInfo.ScrollOwner { get; set; }

        public double WideSpaceWidth
        {
            get
            {
                CalculateDefaultTextMetrics();
                return wideSpaceWidth;
            }
        }

        public double DefaultLineHeight
        {
            get
            {
                CalculateDefaultTextMetrics();
                return defaultLineHeight;
            }
        }

        public double DefaultBaseline
        {
            get
            {
                CalculateDefaultTextMetrics();
                return defaultBaseline;
            }
        }

        public ServiceContainer Services
        {
            get { return services; }
        }

        public double DocumentHeight
        {
            get { return heightTree != null ? heightTree.TotalHeight : 0; }
        }

        public Pen ColumnRulerPen
        {
            get { return (Pen)GetValue(ColumnRulerPenProperty); }
            set { SetValue(ColumnRulerPenProperty, value); }
        }

        public Brush CurrentLineBackground
        {
            get { return (Brush)GetValue(CurrentLineBackgroundProperty); }
            set { SetValue(CurrentLineBackgroundProperty, value); }
        }

        public Pen CurrentLineBorder
        {
            get { return (Pen)GetValue(CurrentLineBorderProperty); }
            set { SetValue(CurrentLineBorderProperty, value); }
        }

        public int HighlightedLine
        {
            get { return currentLineHighlighRenderer.Line; }
            set { currentLineHighlighRenderer.Line = value; }
        }

        #endregion

        #region События

        public event EventHandler DocumentChanged;

        public event MouseEventHandler MouseHover
        {
            add { AddHandler(MouseHoverEvent, value); }
            remove { RemoveHandler(MouseHoverEvent, value); }
        }

        public event MouseEventHandler MouseHoverStopped
        {
            add { AddHandler(MouseHoverStoppedEvent, value); }
            remove { RemoveHandler(MouseHoverStoppedEvent, value); }
        }

        public event PropertyChangedEventHandler OptionChanged;

        public event MouseEventHandler PreviewMouseHover
        {
            add { AddHandler(PreviewMouseHoverEvent, value); }
            remove { RemoveHandler(PreviewMouseHoverEvent, value); }
        }

        public event MouseEventHandler PreviewMouseHoverStopped
        {
            add { AddHandler(PreviewMouseHoverStoppedEvent, value); }
            remove { RemoveHandler(PreviewMouseHoverStoppedEvent, value); }
        }

        public event EventHandler ScrollOffsetChanged;

        public event EventHandler<VisualLineConstructionStartEventArgs> VisualLineConstructionStarting;

        public event EventHandler VisualLinesChanged;

        #endregion

        #region Конструктор

        static TextView()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(TextView), new FrameworkPropertyMetadata(Boxes.True));
            FocusableProperty.OverrideMetadata(typeof(TextView), new FrameworkPropertyMetadata(Boxes.False));
        }

        public TextView()
        {
            services.AddService(typeof(TextView), this);
            textLayer = new TextLayer(this);
            elementGenerators = new ObserveAddRemoveCollection<VisualLineElementGenerator>(ElementGenerator_Added, ElementGenerator_Removed);
            lineTransformers = new ObserveAddRemoveCollection<IVisualLineTransformer>(LineTransformer_Added, LineTransformer_Removed);
            backgroundRenderers = new ObserveAddRemoveCollection<IBackgroundRenderer>(BackgroundRenderer_Added, BackgroundRenderer_Removed);
            columnRulerRenderer = new ColumnRulerRenderer(this);
            currentLineHighlighRenderer = new CurrentLineHighlightRenderer(this);
            Options = new TextEditorOptions();

            Debug.Assert(singleCharacterElementGenerator != null);

            layers = new LayerCollection(this);
            InsertLayer(textLayer, KnownLayer.Text, LayerInsertionPosition.Replace);

            hoverLogic = new MouseHoverLogic(this);
            hoverLogic.MouseHover += (sender, e) => RaiseHoverEventPair(e, PreviewMouseHoverEvent, MouseHoverEvent);
            hoverLogic.MouseHoverStopped += (sender, e) => RaiseHoverEventPair(e, PreviewMouseHoverStoppedEvent, MouseHoverStoppedEvent);
        }

        #endregion

        #region Методы

        private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextView)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
        }

        private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
        {
            if (oldValue != null)
            {
                heightTree.Dispose();
                heightTree = null;
                formatter.Dispose();
                formatter = null;
                cachedElements.Dispose();
                cachedElements = null;
                TextDocumentWeakEventManager.Changing.RemoveListener(oldValue, this);
            }
            document = newValue;
            ClearScrollData();
            ClearVisualLines();
            if (newValue != null)
            {
                TextDocumentWeakEventManager.Changing.AddListener(newValue, this);
                formatter = TextFormatterFactory.Create(this);
                InvalidateDefaultTextMetrics();
                heightTree = new HeightTree(newValue, DefaultLineHeight);
                cachedElements = new TextViewCachedElements();
            }
            InvalidateMeasure(DispatcherPriority.Normal);
            if (DocumentChanged != null)
            {
                DocumentChanged(this, EventArgs.Empty);
            }
        }

        private void RecreateTextFormatter()
        {
            if (formatter != null)
            {
                formatter.Dispose();
                formatter = TextFormatterFactory.Create(this);
                Redraw();
            }
        }

        private void RecreateCachedElements()
        {
            if (cachedElements != null)
            {
                cachedElements.Dispose();
                cachedElements = new TextViewCachedElements();
            }
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextDocumentWeakEventManager.Changing))
            {
                var change = (DocumentChangeEventArgs)e;
                Redraw(change.Offset, change.RemovalLength, DispatcherPriority.Normal);
                return true;
            }
            if (managerType == typeof(PropertyChangedWeakEventManager))
            {
                OnOptionChanged((PropertyChangedEventArgs)e);
                return true;
            }
            return false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ReceiveWeakEvent(managerType, sender, e);
        }

        protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
        {
            if (OptionChanged != null)
            {
                OptionChanged(this, e);
            }

            if (Options.ShowColumnRuler)
            {
                columnRulerRenderer.SetRuler(Options.ColumnRulerPosition, ColumnRulerPen);
            }
            else
            {
                columnRulerRenderer.SetRuler(-1, ColumnRulerPen);
            }

            UpdateBuiltinElementGeneratorsFromOptions();
            Redraw();
        }

        private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextView)dp).OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
        }

        private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
        {
            if (oldValue != null)
            {
                PropertyChangedWeakEventManager.RemoveListener(oldValue, this);
            }
            if (newValue != null)
            {
                PropertyChangedWeakEventManager.AddListener(newValue, this);
            }
            OnOptionChanged(new PropertyChangedEventArgs(null));
        }

        private void ElementGenerator_Added(VisualLineElementGenerator generator)
        {
            ConnectToTextView(generator);
            Redraw();
        }

        private void ElementGenerator_Removed(VisualLineElementGenerator generator)
        {
            DisconnectFromTextView(generator);
            Redraw();
        }

        private void LineTransformer_Added(IVisualLineTransformer lineTransformer)
        {
            ConnectToTextView(lineTransformer);
            Redraw();
        }

        private void LineTransformer_Removed(IVisualLineTransformer lineTransformer)
        {
            DisconnectFromTextView(lineTransformer);
            Redraw();
        }

        private void UpdateBuiltinElementGeneratorsFromOptions()
        {
            var options = Options;

            AddRemoveDefaultElementGeneratorOnDemand(ref singleCharacterElementGenerator, options.ShowBoxForControlCharacters || options.ShowSpaces || options.ShowTabs);
            AddRemoveDefaultElementGeneratorOnDemand(ref linkElementGenerator, options.EnableHyperlinks);
            AddRemoveDefaultElementGeneratorOnDemand(ref mailLinkElementGenerator, options.EnableEmailHyperlinks);
        }

        private void AddRemoveDefaultElementGeneratorOnDemand<T>(ref T generator, bool demand)
            where T : VisualLineElementGenerator, IBuiltinElementGenerator, new()
        {
            bool hasGenerator = generator != null;
            if (hasGenerator != demand)
            {
                if (demand)
                {
                    generator = new T();
                    ElementGenerators.Add(generator);
                }
                else
                {
                    ElementGenerators.Remove(generator);
                    generator = null;
                }
            }
            if (generator != null)
            {
                generator.FetchOptions(Options);
            }
        }

        private void LayersChanged()
        {
            textLayer.index = layers.IndexOf(textLayer);
        }

        public void InsertLayer(UIElement layer, KnownLayer referencedLayer, LayerInsertionPosition position)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }
            if (!Enum.IsDefined(typeof(KnownLayer), referencedLayer))
            {
                throw new InvalidEnumArgumentException("referencedLayer", (int)referencedLayer, typeof(KnownLayer));
            }
            if (!Enum.IsDefined(typeof(LayerInsertionPosition), position))
            {
                throw new InvalidEnumArgumentException("position", (int)position, typeof(LayerInsertionPosition));
            }
            if (referencedLayer == KnownLayer.Background && position != LayerInsertionPosition.Above)
            {
                throw new InvalidOperationException("Cannot replace or insert below the background layer.");
            }

            var newPosition = new LayerPosition(referencedLayer, position);
            LayerPosition.SetLayerPosition(layer, newPosition);
            for (int i = 0; i < layers.Count; i++)
            {
                var p = LayerPosition.GetLayerPosition(layers[i]);
                if (p != null)
                {
                    if (p.KnownLayer == referencedLayer && p.Position == LayerInsertionPosition.Replace)
                    {
                        switch (position)
                        {
                            case LayerInsertionPosition.Below:
                                layers.Insert(i, layer);
                                return;
                            case LayerInsertionPosition.Above:
                                layers.Insert(i + 1, layer);
                                return;
                            case LayerInsertionPosition.Replace:
                                layers[i] = layer;
                                return;
                        }
                    }
                    else if (p.KnownLayer == referencedLayer && p.Position == LayerInsertionPosition.Above
                             || p.KnownLayer > referencedLayer)
                    {
                        layers.Insert(i, layer);
                        return;
                    }
                }
            }

            layers.Add(layer);
        }

        protected override Visual GetVisualChild(int index)
        {
            int cut = textLayer.index + 1;
            if (index < cut)
            {
                return layers[index];
            }
            if (index < cut + inlineObjects.Count)
            {
                return inlineObjects[index - cut].Element;
            }
            return layers[index - inlineObjects.Count];
        }

        internal void AddInlineObject(InlineObjectRun inlineObject)
        {
            Debug.Assert(inlineObject.VisualLine != null);

            bool alreadyAdded = false;
            for (int i = 0; i < inlineObjects.Count; i++)
            {
                if (inlineObjects[i].Element == inlineObject.Element)
                {
                    RemoveInlineObjectRun(inlineObjects[i], true);
                    inlineObjects.RemoveAt(i);
                    alreadyAdded = true;
                    break;
                }
            }

            inlineObjects.Add(inlineObject);
            if (!alreadyAdded)
            {
                AddVisualChild(inlineObject.Element);
            }
            inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            inlineObject.desiredSize = inlineObject.Element.DesiredSize;
        }

        private void MeasureInlineObjects()
        {
            foreach (InlineObjectRun inlineObject in inlineObjects)
            {
                if (inlineObject.VisualLine.IsDisposed)
                {
                    continue;
                }
                inlineObject.Element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (!inlineObject.Element.DesiredSize.IsClose(inlineObject.desiredSize))
                {
                    inlineObject.desiredSize = inlineObject.Element.DesiredSize;
                    if (allVisualLines.Remove(inlineObject.VisualLine))
                    {
                        DisposeVisualLine(inlineObject.VisualLine);
                    }
                }
            }
        }

        private void RemoveInlineObjects(VisualLine visualLine)
        {
            if (visualLine.hasInlineObjects)
            {
                visualLinesWithOutstandingInlineObjects.Add(visualLine);
            }
        }

        private void RemoveInlineObjectsNow()
        {
            if (visualLinesWithOutstandingInlineObjects.Count == 0)
            {
                return;
            }
            inlineObjects.RemoveAll(
                ior =>
                {
                    if (visualLinesWithOutstandingInlineObjects.Contains(ior.VisualLine))
                    {
                        RemoveInlineObjectRun(ior, false);
                        return true;
                    }
                    return false;
                });
            visualLinesWithOutstandingInlineObjects.Clear();
        }

        private void RemoveInlineObjectRun(InlineObjectRun ior, bool keepElement)
        {
            if (!keepElement && ior.Element.IsKeyboardFocusWithin)
            {
                UIElement element = this;
                while (element != null && !element.Focusable)
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
                if (element != null)
                {
                    Keyboard.Focus(element);
                }
            }
            ior.VisualLine = null;
            if (!keepElement)
            {
                RemoveVisualChild(ior.Element);
            }
        }

        public void Redraw()
        {
            Redraw(DispatcherPriority.Normal);
        }

        public void Redraw(DispatcherPriority redrawPriority)
        {
            VerifyAccess();
            ClearVisualLines();
            InvalidateMeasure(redrawPriority);
        }

        public void Redraw(VisualLine visualLine, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
        {
            VerifyAccess();
            if (allVisualLines.Remove(visualLine))
            {
                DisposeVisualLine(visualLine);
                InvalidateMeasure(redrawPriority);
            }
        }

        public void Redraw(int offset, int length, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
        {
            VerifyAccess();
            bool changedSomethingBeforeOrInLine = false;
            for (int i = 0; i < allVisualLines.Count; i++)
            {
                var visualLine = allVisualLines[i];
                int lineStart = visualLine.FirstDocumentLine.Offset;
                int lineEnd = visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.TotalLength;
                if (offset <= lineEnd)
                {
                    changedSomethingBeforeOrInLine = true;
                    if (offset + length >= lineStart)
                    {
                        allVisualLines.RemoveAt(i--);
                        DisposeVisualLine(visualLine);
                    }
                }
            }
            if (changedSomethingBeforeOrInLine)
            {
                InvalidateMeasure(redrawPriority);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "knownLayer",
            Justification = "This method is meant to invalidate only a specific layer - I just haven't figured out how to do that, yet.")]
        public void InvalidateLayer(KnownLayer knownLayer)
        {
            InvalidateMeasure(DispatcherPriority.Normal);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "knownLayer",
            Justification = "This method is meant to invalidate only a specific layer - I just haven't figured out how to do that, yet.")]
        public void InvalidateLayer(KnownLayer knownLayer, DispatcherPriority priority)
        {
            InvalidateMeasure(priority);
        }

        public void Redraw(ISegment segment, DispatcherPriority redrawPriority = DispatcherPriority.Normal)
        {
            if (segment != null)
            {
                Redraw(segment.Offset, segment.Length, redrawPriority);
            }
        }

        private void ClearVisualLines()
        {
            visibleVisualLines = null;
            if (allVisualLines.Count != 0)
            {
                foreach (VisualLine visualLine in allVisualLines)
                {
                    DisposeVisualLine(visualLine);
                }
                allVisualLines.Clear();
            }
        }

        private void DisposeVisualLine(VisualLine visualLine)
        {
            if (newVisualLines != null && newVisualLines.Contains(visualLine))
            {
                throw new ArgumentException("Cannot dispose visual line because it is in construction!");
            }
            visibleVisualLines = null;
            visualLine.Dispose();
            RemoveInlineObjects(visualLine);
        }

        private void InvalidateMeasure(DispatcherPriority priority)
        {
            if (priority >= DispatcherPriority.Render)
            {
                if (invalidateMeasureOperation != null)
                {
                    invalidateMeasureOperation.Abort();
                    invalidateMeasureOperation = null;
                }
                base.InvalidateMeasure();
            }
            else
            {
                if (invalidateMeasureOperation != null)
                {
                    invalidateMeasureOperation.Priority = priority;
                }
                else
                {
                    invalidateMeasureOperation = Dispatcher.BeginInvoke(
                        priority,
                        new Action(
                            delegate
                            {
                                invalidateMeasureOperation = null;
                                base.InvalidateMeasure();
                            }
                            )
                        );
                }
            }
        }

        public VisualLine GetVisualLine(int documentLineNumber)
        {
            foreach (VisualLine visualLine in allVisualLines)
            {
                Debug.Assert(visualLine.IsDisposed == false);
                int start = visualLine.FirstDocumentLine.LineNumber;
                int end = visualLine.LastDocumentLine.LineNumber;
                if (documentLineNumber >= start && documentLineNumber <= end)
                {
                    return visualLine;
                }
            }
            return null;
        }

        public VisualLine GetOrConstructVisualLine(DocumentLine documentLine)
        {
            if (documentLine == null)
            {
                throw new ArgumentNullException("documentLine");
            }
            if (!Document.Lines.Contains(documentLine))
            {
                throw new InvalidOperationException("Line belongs to wrong document");
            }
            VerifyAccess();

            var l = GetVisualLine(documentLine.LineNumber);
            if (l == null)
            {
                var globalTextRunProperties = CreateGlobalTextRunProperties();
                var paragraphProperties = CreateParagraphProperties(globalTextRunProperties);

                while (heightTree.GetIsCollapsed(documentLine.LineNumber))
                {
                    documentLine = documentLine.PreviousLine;
                }

                l = BuildVisualLine(documentLine,
                    globalTextRunProperties, paragraphProperties,
                    elementGenerators.ToArray(), lineTransformers.ToArray(),
                    lastAvailableSize);
                allVisualLines.Add(l);

                foreach (var line in allVisualLines)
                {
                    line.VisualTop = heightTree.GetVisualPosition(line.FirstDocumentLine);
                }
            }
            return l;
        }

        public void EnsureVisualLines()
        {
            Dispatcher.VerifyAccess();
            if (inMeasure)
            {
                throw new InvalidOperationException("The visual line build process is already running! Cannot EnsureVisualLines() during Measure!");
            }
            if (!VisualLinesValid)
            {
                InvalidateMeasure(DispatcherPriority.Normal);

                UpdateLayout();
            }

            if (!VisualLinesValid)
            {
                Debug.WriteLine("UpdateLayout() failed in EnsureVisualLines");
                MeasureOverride(lastAvailableSize);
            }
            if (!VisualLinesValid)
            {
                throw new VisualLinesInvalidException("Internal error: visual lines invalid after EnsureVisualLines call");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width > 32000)
            {
                availableSize.Width = 32000;
            }

            if (!canHorizontallyScroll && !availableSize.Width.IsClose(lastAvailableSize.Width))
            {
                ClearVisualLines();
            }
            lastAvailableSize = availableSize;

            foreach (UIElement layer in layers)
            {
                layer.Measure(availableSize);
            }
            MeasureInlineObjects();

            InvalidateVisual();

            double maxWidth;
            if (document == null)
            {
                allVisualLines = new List<VisualLine>();
                visibleVisualLines = allVisualLines.AsReadOnly();
                maxWidth = 0;
            }
            else
            {
                inMeasure = true;
                try
                {
                    maxWidth = CreateAndMeasureVisualLines(availableSize);
                }
                finally
                {
                    inMeasure = false;
                }
            }

            RemoveInlineObjectsNow();

            maxWidth += AdditionalHorizontalScrollAmount;
            double heightTreeHeight = DocumentHeight;
            var options = Options;
            if (options.AllowScrollBelowDocument)
            {
                if (!double.IsInfinity(scrollViewport.Height))
                {
                    double minVisibleDocumentHeight = Math.Max(DefaultLineHeight, Editing.Caret.MinimumDistanceToViewBorder);

                    double scrollViewportBottom = Math.Min(heightTreeHeight - minVisibleDocumentHeight, scrollOffset.Y) + scrollViewport.Height;

                    heightTreeHeight = Math.Max(heightTreeHeight, scrollViewportBottom);
                }
            }

            textLayer.SetVisualLines(visibleVisualLines);

            SetScrollData(availableSize,
                new Size(maxWidth, heightTreeHeight),
                scrollOffset);
            if (VisualLinesChanged != null)
            {
                VisualLinesChanged(this, EventArgs.Empty);
            }

            return new Size(Math.Min(availableSize.Width, maxWidth), Math.Min(availableSize.Height, heightTreeHeight));
        }

        private double CreateAndMeasureVisualLines(Size availableSize)
        {
            var globalTextRunProperties = CreateGlobalTextRunProperties();
            var paragraphProperties = CreateParagraphProperties(globalTextRunProperties);

            Debug.WriteLine("Measure availableSize=" + availableSize + ", scrollOffset=" + scrollOffset);
            var firstLineInView = heightTree.GetLineByVisualPosition(scrollOffset.Y);

            clippedPixelsOnTop = scrollOffset.Y - heightTree.GetVisualPosition(firstLineInView);

            Debug.Assert(clippedPixelsOnTop >= -ExtensionMethods.EPSILON);

            newVisualLines = new List<VisualLine>();

            if (VisualLineConstructionStarting != null)
            {
                VisualLineConstructionStarting(this, new VisualLineConstructionStartEventArgs(firstLineInView));
            }

            var elementGeneratorsArray = elementGenerators.ToArray();
            var lineTransformersArray = lineTransformers.ToArray();
            var nextLine = firstLineInView;
            double maxWidth = 0;
            double yPos = -clippedPixelsOnTop;
            while (yPos < availableSize.Height && nextLine != null)
            {
                var visualLine = GetVisualLine(nextLine.LineNumber);
                if (visualLine == null)
                {
                    visualLine = BuildVisualLine(nextLine,
                        globalTextRunProperties, paragraphProperties,
                        elementGeneratorsArray, lineTransformersArray,
                        availableSize);
                }

                visualLine.VisualTop = scrollOffset.Y + yPos;

                nextLine = visualLine.LastDocumentLine.NextLine;

                yPos += visualLine.Height;

                foreach (TextLine textLine in visualLine.TextLines)
                {
                    if (textLine.WidthIncludingTrailingWhitespace > maxWidth)
                    {
                        maxWidth = textLine.WidthIncludingTrailingWhitespace;
                    }
                }

                newVisualLines.Add(visualLine);
            }

            foreach (VisualLine line in allVisualLines)
            {
                Debug.Assert(line.IsDisposed == false);
                if (!newVisualLines.Contains(line))
                {
                    DisposeVisualLine(line);
                }
            }

            allVisualLines = newVisualLines;

            visibleVisualLines = new ReadOnlyCollection<VisualLine>(newVisualLines.ToArray());
            newVisualLines = null;

            if (allVisualLines.Any(line => line.IsDisposed))
            {
                throw new InvalidOperationException("A visual line was disposed even though it is still in use.\n" +
                                                    "This can happen when Redraw() is called during measure for lines " +
                                                    "that are already constructed.");
            }
            return maxWidth;
        }

        private TextRunProperties CreateGlobalTextRunProperties()
        {
            var p = new GlobalTextRunProperties();
            p.typeface = this.CreateTypeface();
            p.fontRenderingEmSize = FontSize;
            p.foregroundBrush = (Brush)GetValue(Control.ForegroundProperty);
            ExtensionMethods.CheckIsFrozen(p.foregroundBrush);
            p.cultureInfo = CultureInfo.CurrentCulture;
            return p;
        }

        private VisualLineTextParagraphProperties CreateParagraphProperties(TextRunProperties defaultTextRunProperties)
        {
            return new VisualLineTextParagraphProperties
            {
                defaultTextRunProperties = defaultTextRunProperties,
                textWrapping = canHorizontallyScroll ? TextWrapping.NoWrap : TextWrapping.Wrap,
                tabSize = Options.IndentationSize * WideSpaceWidth
            };
        }

        private VisualLine BuildVisualLine(DocumentLine documentLine,
            TextRunProperties globalTextRunProperties,
            VisualLineTextParagraphProperties paragraphProperties,
            VisualLineElementGenerator[] elementGeneratorsArray,
            IVisualLineTransformer[] lineTransformersArray,
            Size availableSize)
        {
            if (heightTree.GetIsCollapsed(documentLine.LineNumber))
            {
                throw new InvalidOperationException("Trying to build visual line from collapsed line");
            }

            var visualLine = new VisualLine(this, documentLine);
            var textSource = new VisualLineTextSource(visualLine)
            {
                Document = document,
                GlobalTextRunProperties = globalTextRunProperties,
                TextView = this
            };

            visualLine.ConstructVisualElements(textSource, elementGeneratorsArray);

            if (visualLine.FirstDocumentLine != visualLine.LastDocumentLine)
            {
                double firstLinePos = heightTree.GetVisualPosition(visualLine.FirstDocumentLine.NextLine);
                double lastLinePos = heightTree.GetVisualPosition(visualLine.LastDocumentLine.NextLine ?? visualLine.LastDocumentLine);
                if (!firstLinePos.IsClose(lastLinePos))
                {
                    for (int i = visualLine.FirstDocumentLine.LineNumber + 1; i <= visualLine.LastDocumentLine.LineNumber; i++)
                    {
                        if (!heightTree.GetIsCollapsed(i))
                        {
                            throw new InvalidOperationException("Line " + i + " was skipped by a VisualLineElementGenerator, but it is not collapsed.");
                        }
                    }
                    throw new InvalidOperationException("All lines collapsed but visual pos different - height tree inconsistency?");
                }
            }

            visualLine.RunTransformers(textSource, lineTransformersArray);

            int textOffset = 0;
            TextLineBreak lastLineBreak = null;
            var textLines = new List<TextLine>();
            paragraphProperties.indent = 0;
            paragraphProperties.firstLineInParagraph = true;
            while (textOffset <= visualLine.VisualLengthWithEndOfLineMarker)
            {
                var textLine = formatter.FormatLine(
                    textSource,
                    textOffset,
                    availableSize.Width,
                    paragraphProperties,
                    lastLineBreak
                    );
                textLines.Add(textLine);
                textOffset += textLine.Length;

                if (textOffset >= visualLine.VisualLengthWithEndOfLineMarker)
                {
                    break;
                }

                if (paragraphProperties.firstLineInParagraph)
                {
                    paragraphProperties.firstLineInParagraph = false;

                    var options = Options;
                    double indentation = 0;
                    if (options.InheritWordWrapIndentation)
                    {
                        int indentVisualColumn = GetIndentationVisualColumn(visualLine);
                        if (indentVisualColumn > 0 && indentVisualColumn < textOffset)
                        {
                            indentation = textLine.GetDistanceFromCharacterHit(new CharacterHit(indentVisualColumn, 0));
                        }
                    }
                    indentation += options.WordWrapIndentation;

                    if (indentation > 0 && indentation * 2 < availableSize.Width)
                    {
                        paragraphProperties.indent = indentation;
                    }
                }
                lastLineBreak = textLine.GetTextLineBreak();
            }
            visualLine.SetTextLines(textLines);
            heightTree.SetHeight(visualLine.FirstDocumentLine, visualLine.Height);
            return visualLine;
        }

        private static int GetIndentationVisualColumn(VisualLine visualLine)
        {
            if (visualLine.Elements.Count == 0)
            {
                return 0;
            }
            int column = 0;
            int elementIndex = 0;
            var element = visualLine.Elements[elementIndex];
            while (element.IsWhitespace(column))
            {
                column++;
                if (column == element.VisualColumn + element.VisualLength)
                {
                    elementIndex++;
                    if (elementIndex == visualLine.Elements.Count)
                    {
                        break;
                    }
                    element = visualLine.Elements[elementIndex];
                }
            }
            return column;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            EnsureVisualLines();

            foreach (UIElement layer in layers)
            {
                layer.Arrange(new Rect(new Point(0, 0), finalSize));
            }

            if (document == null || allVisualLines.Count == 0)
            {
                return finalSize;
            }

            var newScrollOffset = scrollOffset;
            if (scrollOffset.X + finalSize.Width > scrollExtent.Width)
            {
                newScrollOffset.X = Math.Max(0, scrollExtent.Width - finalSize.Width);
            }
            if (scrollOffset.Y + finalSize.Height > scrollExtent.Height)
            {
                newScrollOffset.Y = Math.Max(0, scrollExtent.Height - finalSize.Height);
            }
            if (SetScrollData(scrollViewport, scrollExtent, newScrollOffset))
            {
                InvalidateMeasure(DispatcherPriority.Normal);
            }

            if (visibleVisualLines != null)
            {
                var pos = new Point(-scrollOffset.X, -clippedPixelsOnTop);
                foreach (VisualLine visualLine in visibleVisualLines)
                {
                    int offset = 0;
                    foreach (TextLine textLine in visualLine.TextLines)
                    {
                        foreach (var span in textLine.GetTextRunSpans())
                        {
                            var inline = span.Value as InlineObjectRun;
                            if (inline != null && inline.VisualLine != null)
                            {
                                Debug.Assert(inlineObjects.Contains(inline));
                                double distance = textLine.GetDistanceFromCharacterHit(new CharacterHit(offset, 0));
                                inline.Element.Arrange(new Rect(new Point(pos.X + distance, pos.Y), inline.Element.DesiredSize));
                            }
                            offset += span.Length;
                        }
                        pos.Y += textLine.Height;
                    }
                }
            }
            InvalidateCursorIfMouseWithinTextView();

            return finalSize;
        }

        private void BackgroundRenderer_Added(IBackgroundRenderer renderer)
        {
            ConnectToTextView(renderer);
            InvalidateLayer(renderer.Layer);
        }

        private void BackgroundRenderer_Removed(IBackgroundRenderer renderer)
        {
            DisconnectFromTextView(renderer);
            InvalidateLayer(renderer.Layer);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            RenderBackground(drawingContext, KnownLayer.Background);
            foreach (var line in visibleVisualLines)
            {
                Brush currentBrush = null;
                int startVC = 0;
                int length = 0;
                foreach (var element in line.Elements)
                {
                    if (currentBrush == null || !currentBrush.Equals(element.BackgroundBrush))
                    {
                        if (currentBrush != null)
                        {
                            var builder = new BackgroundGeometryBuilder();
                            builder.AlignToWholePixels = true;
                            builder.CornerRadius = 3;
                            foreach (var rect in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, line, startVC, startVC + length))
                            {
                                builder.AddRectangle(this, rect);
                            }
                            var geometry = builder.CreateGeometry();
                            if (geometry != null)
                            {
                                drawingContext.DrawGeometry(currentBrush, null, geometry);
                            }
                        }
                        startVC = element.VisualColumn;
                        length = element.DocumentLength;
                        currentBrush = element.BackgroundBrush;
                    }
                    else
                    {
                        length += element.VisualLength;
                    }
                }
                if (currentBrush != null)
                {
                    var builder = new BackgroundGeometryBuilder();
                    builder.AlignToWholePixels = true;
                    builder.CornerRadius = 3;
                    foreach (var rect in BackgroundGeometryBuilder.GetRectsFromVisualSegment(this, line, startVC, startVC + length))
                    {
                        builder.AddRectangle(this, rect);
                    }
                    var geometry = builder.CreateGeometry();
                    if (geometry != null)
                    {
                        drawingContext.DrawGeometry(currentBrush, null, geometry);
                    }
                }
            }
        }

        internal void RenderBackground(DrawingContext drawingContext, KnownLayer layer)
        {
            foreach (IBackgroundRenderer bg in backgroundRenderers)
            {
                if (bg.Layer == layer)
                {
                    bg.Draw(this, drawingContext);
                }
            }
        }

        internal void ArrangeTextLayer(IList<VisualLineDrawingVisual> visuals)
        {
            var pos = new Point(-scrollOffset.X, -clippedPixelsOnTop);
            foreach (VisualLineDrawingVisual visual in visuals)
            {
                var t = visual.Transform as TranslateTransform;
                if (t == null || t.X != pos.X || t.Y != pos.Y)
                {
                    visual.Transform = new TranslateTransform(pos.X, pos.Y);
                    visual.Transform.Freeze();
                }
                pos.Y += visual.Height;
            }
        }

        private void ClearScrollData()
        {
            SetScrollData(new Size(), new Size(), new Vector());
        }

        private bool SetScrollData(Size viewport, Size extent, Vector offset)
        {
            if (!(viewport.IsClose(scrollViewport)
                  && extent.IsClose(scrollExtent)
                  && offset.IsClose(scrollOffset)))
            {
                scrollViewport = viewport;
                scrollExtent = extent;
                SetScrollOffset(offset);
                OnScrollChange();
                return true;
            }
            return false;
        }

        private void OnScrollChange()
        {
            var scrollOwner = ((IScrollInfo)this).ScrollOwner;
            if (scrollOwner != null)
            {
                scrollOwner.InvalidateScrollInfo();
            }
        }

        private void SetScrollOffset(Vector vector)
        {
            if (!canHorizontallyScroll)
            {
                vector.X = 0;
            }
            if (!canVerticallyScroll)
            {
                vector.Y = 0;
            }

            if (!scrollOffset.IsClose(vector))
            {
                scrollOffset = vector;
                if (ScrollOffsetChanged != null)
                {
                    ScrollOffsetChanged(this, EventArgs.Empty);
                }
            }
        }

        void IScrollInfo.LineUp()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - DefaultLineHeight);
        }

        void IScrollInfo.LineDown()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + DefaultLineHeight);
        }

        void IScrollInfo.LineLeft()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - WideSpaceWidth);
        }

        void IScrollInfo.LineRight()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + WideSpaceWidth);
        }

        void IScrollInfo.PageUp()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y - scrollViewport.Height);
        }

        void IScrollInfo.PageDown()
        {
            ((IScrollInfo)this).SetVerticalOffset(scrollOffset.Y + scrollViewport.Height);
        }

        void IScrollInfo.PageLeft()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X - scrollViewport.Width);
        }

        void IScrollInfo.PageRight()
        {
            ((IScrollInfo)this).SetHorizontalOffset(scrollOffset.X + scrollViewport.Width);
        }

        void IScrollInfo.MouseWheelUp()
        {
            ((IScrollInfo)this).SetVerticalOffset(
                scrollOffset.Y - (SystemParameters.WheelScrollLines * DefaultLineHeight));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelDown()
        {
            ((IScrollInfo)this).SetVerticalOffset(
                scrollOffset.Y + (SystemParameters.WheelScrollLines * DefaultLineHeight));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelLeft()
        {
            ((IScrollInfo)this).SetHorizontalOffset(
                scrollOffset.X - (SystemParameters.WheelScrollLines * WideSpaceWidth));
            OnScrollChange();
        }

        void IScrollInfo.MouseWheelRight()
        {
            ((IScrollInfo)this).SetHorizontalOffset(
                scrollOffset.X + (SystemParameters.WheelScrollLines * WideSpaceWidth));
            OnScrollChange();
        }

        private void InvalidateDefaultTextMetrics()
        {
            defaultTextMetricsValid = false;
            if (heightTree != null)
            {
                CalculateDefaultTextMetrics();
            }
        }

        private void CalculateDefaultTextMetrics()
        {
            if (defaultTextMetricsValid)
            {
                return;
            }
            defaultTextMetricsValid = true;
            if (formatter != null)
            {
                var textRunProperties = CreateGlobalTextRunProperties();
                using (var line = formatter.FormatLine(
                    new SimpleTextSource("x", textRunProperties),
                    0, 32000,
                    new VisualLineTextParagraphProperties
                    {
                        defaultTextRunProperties = textRunProperties
                    },
                    null))
                {
                    wideSpaceWidth = Math.Max(1, line.WidthIncludingTrailingWhitespace);
                    defaultBaseline = Math.Max(1, line.Baseline);
                    defaultLineHeight = Math.Max(1, line.Height);
                }
            }
            else
            {
                wideSpaceWidth = FontSize / 2;
                defaultBaseline = FontSize;
                defaultLineHeight = FontSize + 3;
            }

            if (heightTree != null)
            {
                heightTree.DefaultLineHeight = defaultLineHeight;
            }
        }

        private static double ValidateVisualOffset(double offset)
        {
            if (double.IsNaN(offset))
            {
                throw new ArgumentException("offset must not be NaN");
            }
            if (offset < 0)
            {
                return 0;
            }
            return offset;
        }

        void IScrollInfo.SetHorizontalOffset(double offset)
        {
            offset = ValidateVisualOffset(offset);
            if (!scrollOffset.X.IsClose(offset))
            {
                SetScrollOffset(new Vector(offset, scrollOffset.Y));
                InvalidateVisual();
                textLayer.InvalidateVisual();
            }
        }

        void IScrollInfo.SetVerticalOffset(double offset)
        {
            offset = ValidateVisualOffset(offset);
            if (!scrollOffset.Y.IsClose(offset))
            {
                SetScrollOffset(new Vector(scrollOffset.X, offset));
                InvalidateMeasure(DispatcherPriority.Normal);
            }
        }

        Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null || visual == this || !IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            var childTransform = visual.TransformToAncestor(this);
            rectangle = childTransform.TransformBounds(rectangle);

            MakeVisible(Rect.Offset(rectangle, scrollOffset));

            return rectangle;
        }

        public void MakeVisible(Rect rectangle)
        {
            var visibleRectangle = new Rect(scrollOffset.X, scrollOffset.Y,
                scrollViewport.Width, scrollViewport.Height);
            var newScrollOffset = scrollOffset;
            if (rectangle.Left < visibleRectangle.Left)
            {
                if (rectangle.Right > visibleRectangle.Right)
                {
                    newScrollOffset.X = rectangle.Left + rectangle.Width / 2;
                }
                else
                {
                    newScrollOffset.X = rectangle.Left;
                }
            }
            else if (rectangle.Right > visibleRectangle.Right)
            {
                newScrollOffset.X = rectangle.Right - scrollViewport.Width;
            }
            if (rectangle.Top < visibleRectangle.Top)
            {
                if (rectangle.Bottom > visibleRectangle.Bottom)
                {
                    newScrollOffset.Y = rectangle.Top + rectangle.Height / 2;
                }
                else
                {
                    newScrollOffset.Y = rectangle.Top;
                }
            }
            else if (rectangle.Bottom > visibleRectangle.Bottom)
            {
                newScrollOffset.Y = rectangle.Bottom - scrollViewport.Height;
            }
            newScrollOffset.X = ValidateVisualOffset(newScrollOffset.X);
            newScrollOffset.Y = ValidateVisualOffset(newScrollOffset.Y);
            if (!scrollOffset.IsClose(newScrollOffset))
            {
                SetScrollOffset(newScrollOffset);
                OnScrollChange();
                InvalidateMeasure(DispatcherPriority.Normal);
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        public static void InvalidateCursor()
        {
            if (!invalidCursor)
            {
                invalidCursor = true;
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(
                        delegate
                        {
                            invalidCursor = false;
                            Mouse.UpdateCursor();
                        }));
            }
        }

        internal void InvalidateCursorIfMouseWithinTextView()
        {
            if (IsMouseOver)
            {
                InvalidateCursor();
            }
        }

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            var element = GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset);
            if (element != null)
            {
                element.OnQueryCursor(e);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled)
            {
                EnsureVisualLines();
                var element = GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset);
                if (element != null)
                {
                    element.OnMouseDown(e);
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (!e.Handled)
            {
                EnsureVisualLines();
                var element = GetVisualLineElementFromPosition(e.GetPosition(this) + scrollOffset);
                if (element != null)
                {
                    element.OnMouseUp(e);
                }
            }
        }

        public VisualLine GetVisualLineFromVisualTop(double visualTop)
        {
            EnsureVisualLines();
            foreach (VisualLine vl in VisualLines)
            {
                if (visualTop < vl.VisualTop)
                {
                    continue;
                }
                if (visualTop < vl.VisualTop + vl.Height)
                {
                    return vl;
                }
            }
            return null;
        }

        public double GetVisualTopByDocumentLine(int line)
        {
            VerifyAccess();
            if (heightTree == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            return heightTree.GetVisualPosition(heightTree.GetLineByNumber(line));
        }

        private VisualLineElement GetVisualLineElementFromPosition(Point visualPosition)
        {
            var vl = GetVisualLineFromVisualTop(visualPosition.Y);
            if (vl != null)
            {
                int column = vl.GetVisualColumnFloor(visualPosition);

                foreach (VisualLineElement element in vl.Elements)
                {
                    if (element.VisualColumn + element.VisualLength <= column)
                    {
                        continue;
                    }
                    return element;
                }
            }
            return null;
        }

        public Point GetVisualPosition(TextViewPosition position, VisualYPosition yPositionMode)
        {
            VerifyAccess();
            if (Document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            var documentLine = Document.GetLineByNumber(position.Line);
            var visualLine = GetOrConstructVisualLine(documentLine);
            int visualColumn = position.VisualColumn;
            if (visualColumn < 0)
            {
                int offset = documentLine.Offset + position.Column - 1;
                visualColumn = visualLine.GetVisualColumn(offset - visualLine.FirstDocumentLine.Offset);
            }
            return visualLine.GetVisualPosition(visualColumn, position.IsAtEndOfLine, yPositionMode);
        }

        public TextViewPosition? GetPosition(Point visualPosition)
        {
            VerifyAccess();
            if (Document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            var line = GetVisualLineFromVisualTop(visualPosition.Y);
            if (line == null)
            {
                return null;
            }
            return line.GetTextViewPosition(visualPosition, Options.EnableVirtualSpace);
        }

        public TextViewPosition? GetPositionFloor(Point visualPosition)
        {
            VerifyAccess();
            if (Document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            var line = GetVisualLineFromVisualTop(visualPosition.Y);
            if (line == null)
            {
                return null;
            }
            return line.GetTextViewPositionFloor(visualPosition, Options.EnableVirtualSpace);
        }

        public virtual object GetService(Type serviceType)
        {
            var instance = services.GetService(serviceType);
            if (instance == null && document != null)
            {
                instance = document.ServiceProvider.GetService(serviceType);
            }
            return instance;
        }

        private void ConnectToTextView(object obj)
        {
            var c = obj as ITextViewConnect;
            if (c != null)
            {
                c.AddToTextView(this);
            }
        }

        private void DisconnectFromTextView(object obj)
        {
            var c = obj as ITextViewConnect;
            if (c != null)
            {
                c.RemoveFromTextView(this);
            }
        }

        private void RaiseHoverEventPair(MouseEventArgs e, RoutedEvent tunnelingEvent, RoutedEvent bubblingEvent)
        {
            var mouseDevice = e.MouseDevice;
            var stylusDevice = e.StylusDevice;
            int inputTime = Environment.TickCount;
            var args1 = new MouseEventArgs(mouseDevice, inputTime, stylusDevice)
            {
                RoutedEvent = tunnelingEvent,
                Source = this
            };
            RaiseEvent(args1);
            var args2 = new MouseEventArgs(mouseDevice, inputTime, stylusDevice)
            {
                RoutedEvent = bubblingEvent,
                Source = this,
                Handled = args1.Handled
            };
            RaiseEvent(args2);
        }

        public CollapsedLineSection CollapseLines(DocumentLine start, DocumentLine end)
        {
            VerifyAccess();
            if (heightTree == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            return heightTree.CollapseText(start, end);
        }

        public DocumentLine GetDocumentLineByVisualTop(double visualTop)
        {
            VerifyAccess();
            if (heightTree == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            return heightTree.GetLineByVisualPosition(visualTop);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (TextFormatterFactory.PropertyChangeAffectsTextFormatter(e.Property))
            {
                RecreateTextFormatter();

                RecreateCachedElements();

                InvalidateDefaultTextMetrics();
            }
            else if (e.Property == Control.ForegroundProperty
                     || e.Property == TextView.NonPrintableCharacterBrushProperty
                     || e.Property == TextView.LinkTextBackgroundBrushProperty
                     || e.Property == TextView.LinkTextForegroundBrushProperty
                     || e.Property == TextView.LinkTextUnderlineProperty)
            {
                RecreateCachedElements();
                Redraw();
            }
            if (e.Property == Control.FontFamilyProperty
                || e.Property == Control.FontSizeProperty
                || e.Property == Control.FontStretchProperty
                || e.Property == Control.FontStyleProperty
                || e.Property == Control.FontWeightProperty)
            {
                RecreateCachedElements();

                InvalidateDefaultTextMetrics();
                Redraw();
            }
            if (e.Property == ColumnRulerPenProperty)
            {
                columnRulerRenderer.SetRuler(Options.ColumnRulerPosition, ColumnRulerPen);
            }
            if (e.Property == CurrentLineBorderProperty)
            {
                currentLineHighlighRenderer.BorderPen = CurrentLineBorder;
            }
            if (e.Property == CurrentLineBackgroundProperty)
            {
                currentLineHighlighRenderer.BackgroundBrush = CurrentLineBackground;
            }
        }

        private static Pen CreateFrozenPen(SolidColorBrush brush)
        {
            var pen = new Pen(brush, 1);
            pen.Freeze();
            return pen;
        }

        #endregion

        #region Вложенный класс: LayerCollection

        private sealed class LayerCollection : UIElementCollection
        {
            #region Поля

            private readonly TextView textView;

            #endregion

            #region Конструктор

            public LayerCollection(TextView textView)
                : base(textView, textView)
            {
                this.textView = textView;
            }

            #endregion

            #region Методы

            public override void Clear()
            {
                base.Clear();
                textView.LayersChanged();
            }

            public override int Add(UIElement element)
            {
                int r = base.Add(element);
                textView.LayersChanged();
                return r;
            }

            public override void RemoveAt(int index)
            {
                base.RemoveAt(index);
                textView.LayersChanged();
            }

            public override void RemoveRange(int index, int count)
            {
                base.RemoveRange(index, count);
                textView.LayersChanged();
            }

            #endregion
        }

        #endregion
    }
}