﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Rendering
{
    public class FormattedTextElement : VisualLineElement
    {
        #region Поля

        internal readonly FormattedText formattedText;

        internal string text;

        internal TextLine textLine;

        #endregion

        #region Свойства

        public LineBreakCondition BreakBefore { get; set; }

        public LineBreakCondition BreakAfter { get; set; }

        #endregion

        #region Конструктор

        public FormattedTextElement(string text, int documentLength)
            : base(1, documentLength)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            this.text = text;
            BreakBefore = LineBreakCondition.BreakPossible;
            BreakAfter = LineBreakCondition.BreakPossible;
        }

        public FormattedTextElement(TextLine text, int documentLength)
            : base(1, documentLength)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            textLine = text;
            BreakBefore = LineBreakCondition.BreakPossible;
            BreakAfter = LineBreakCondition.BreakPossible;
        }

        public FormattedTextElement(FormattedText text, int documentLength)
            : base(1, documentLength)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            formattedText = text;
            BreakBefore = LineBreakCondition.BreakPossible;
            BreakAfter = LineBreakCondition.BreakPossible;
        }

        #endregion

        #region Методы

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            if (textLine == null)
            {
                var formatter = TextFormatterFactory.Create(context.TextView);
                textLine = PrepareText(formatter, text, TextRunProperties);
                text = null;
            }
            return new FormattedTextRun(this, TextRunProperties);
        }

        public static TextLine PrepareText(TextFormatter formatter, string text, TextRunProperties properties)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            return formatter.FormatLine(
                new SimpleTextSource(text, properties),
                0,
                32000,
                new VisualLineTextParagraphProperties
                {
                    defaultTextRunProperties = properties,
                    textWrapping = TextWrapping.NoWrap,
                    tabSize = 40
                },
                null);
        }

        #endregion
    }
}