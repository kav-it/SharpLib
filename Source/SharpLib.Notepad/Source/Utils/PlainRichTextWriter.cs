using System;
using System.IO;
using System.Text;

namespace SharpLib.Notepad.Utils
{
    internal class PlainRichTextWriter : RichTextWriter
    {
        #region Поля

        protected readonly TextWriter textWriter;

        private int indentationLevel;

        private string indentationString = "\t";

        private char prevChar;

        #endregion

        #region Свойства

        public string IndentationString
        {
            get { return indentationString; }
            set { indentationString = value; }
        }

        public override Encoding Encoding
        {
            get { return textWriter.Encoding; }
        }

        public override IFormatProvider FormatProvider
        {
            get { return textWriter.FormatProvider; }
        }

        public override string NewLine
        {
            get { return textWriter.NewLine; }
            set { textWriter.NewLine = value; }
        }

        #endregion

        #region Конструктор

        public PlainRichTextWriter(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter");
            }
            this.textWriter = textWriter;
        }

        #endregion

        #region Методы

        protected override void BeginUnhandledSpan()
        {
        }

        public override void EndSpan()
        {
        }

        private void WriteIndentation()
        {
            for (int i = 0; i < indentationLevel; i++)
            {
                textWriter.Write(indentationString);
            }
        }

        protected void WriteIndentationIfNecessary()
        {
            if (prevChar == '\n')
            {
                WriteIndentation();
                prevChar = '\0';
            }
        }

        protected virtual void AfterWrite()
        {
        }

        public override void Write(char value)
        {
            if (prevChar == '\n')
            {
                WriteIndentation();
            }
            textWriter.Write(value);
            prevChar = value;
            AfterWrite();
        }

        public override void Indent()
        {
            indentationLevel++;
        }

        public override void Unindent()
        {
            if (indentationLevel == 0)
            {
                throw new NotSupportedException();
            }
            indentationLevel--;
        }

        #endregion
    }
}