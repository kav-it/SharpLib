using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding
{
    public sealed class FoldingElementGenerator : VisualLineElementGenerator, ITextViewConnect
    {
        #region Поля

        public static readonly Brush DefaultTextBrush = Brushes.Gray;

        private static Brush textBrush = DefaultTextBrush;

        private readonly List<TextView> textViews = new List<TextView>();

        private FoldingManager foldingManager;

        #endregion

        #region Свойства

        public FoldingManager FoldingManager
        {
            get { return foldingManager; }
            set
            {
                if (foldingManager != value)
                {
                    if (foldingManager != null)
                    {
                        foreach (TextView v in textViews)
                        {
                            foldingManager.RemoveFromTextView(v);
                        }
                    }
                    foldingManager = value;
                    if (foldingManager != null)
                    {
                        foreach (TextView v in textViews)
                        {
                            foldingManager.AddToTextView(v);
                        }
                    }
                }
            }
        }

        public static Brush TextBrush
        {
            get { return textBrush; }
            set { textBrush = value; }
        }

        #endregion

        #region Методы

        void ITextViewConnect.AddToTextView(TextView textView)
        {
            textViews.Add(textView);
            if (foldingManager != null)
            {
                foldingManager.AddToTextView(textView);
            }
        }

        void ITextViewConnect.RemoveFromTextView(TextView textView)
        {
            textViews.Remove(textView);
            if (foldingManager != null)
            {
                foldingManager.RemoveFromTextView(textView);
            }
        }

        public override void StartGeneration(ITextRunConstructionContext context)
        {
            base.StartGeneration(context);
            if (foldingManager != null)
            {
                if (!foldingManager.textViews.Contains(context.TextView))
                {
                    throw new ArgumentException("Invalid TextView");
                }
                if (context.Document != foldingManager.document)
                {
                    throw new ArgumentException("Invalid document");
                }
            }
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            if (foldingManager != null)
            {
                foreach (FoldingSection fs in foldingManager.GetFoldingsContaining(startOffset))
                {
                    if (fs.IsFolded && fs.EndOffset > startOffset)
                    {
                    }
                }
                return foldingManager.GetNextFoldedFoldingStart(startOffset);
            }
            return -1;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            if (foldingManager == null)
            {
                return null;
            }
            int foldedUntil = -1;
            FoldingSection foldingSection = null;
            foreach (FoldingSection fs in foldingManager.GetFoldingsContaining(offset))
            {
                if (fs.IsFolded)
                {
                    if (fs.EndOffset > foldedUntil)
                    {
                        foldedUntil = fs.EndOffset;
                        foldingSection = fs;
                    }
                }
            }
            if (foldedUntil > offset && foldingSection != null)
            {
                bool foundOverlappingFolding;
                do
                {
                    foundOverlappingFolding = false;
                    foreach (FoldingSection fs in FoldingManager.GetFoldingsContaining(foldedUntil))
                    {
                        if (fs.IsFolded && fs.EndOffset > foldedUntil)
                        {
                            foldedUntil = fs.EndOffset;
                            foundOverlappingFolding = true;
                        }
                    }
                } while (foundOverlappingFolding);

                string title = foldingSection.Title;
                if (string.IsNullOrEmpty(title))
                {
                    title = "...";
                }
                var p = new VisualLineElementTextRunProperties(CurrentContext.GlobalTextRunProperties);
                p.SetForegroundBrush(textBrush);
                var textFormatter = TextFormatterFactory.Create(CurrentContext.TextView);
                var text = FormattedTextElement.PrepareText(textFormatter, title, p);
                return new FoldingLineElement(foldingSection, text, foldedUntil - offset)
                {
                    textBrush = textBrush
                };
            }
            return null;
        }

        #endregion

        #region Вложенный класс: FoldingLineElement

        private sealed class FoldingLineElement : FormattedTextElement
        {
            #region Поля

            private readonly FoldingSection fs;

            internal Brush textBrush;

            #endregion

            #region Конструктор

            public FoldingLineElement(FoldingSection fs, TextLine text, int documentLength)
                : base(text, documentLength)
            {
                this.fs = fs;
            }

            #endregion

            #region Методы

            public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
            {
                return new FoldingLineTextRun(this, TextRunProperties)
                {
                    textBrush = textBrush
                };
            }

            protected internal override void OnMouseDown(MouseButtonEventArgs e)
            {
                if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
                {
                    fs.IsFolded = false;
                    e.Handled = true;
                }
                else
                {
                    base.OnMouseDown(e);
                }
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: FoldingLineTextRun

        private sealed class FoldingLineTextRun : FormattedTextRun
        {
            #region Поля

            internal Brush textBrush;

            #endregion

            #region Конструктор

            public FoldingLineTextRun(FormattedTextElement element, TextRunProperties properties)
                : base(element, properties)
            {
            }

            #endregion

            #region Методы

            public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
            {
                var metrics = Format(double.PositiveInfinity);
                var r = new Rect(origin.X, origin.Y - metrics.Baseline, metrics.Width, metrics.Height);
                drawingContext.DrawRectangle(null, new Pen(textBrush, 1), r);
                base.Draw(drawingContext, origin, rightToLeft, sideways);
            }

            #endregion
        }

        #endregion
    }
}