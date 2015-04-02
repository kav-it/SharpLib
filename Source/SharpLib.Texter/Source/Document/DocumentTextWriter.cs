using System;
using System.IO;
using System.Text;

namespace SharpLib.Texter.Document
{
    public class DocumentTextWriter : TextWriter
    {
        #region Поля

        private readonly IDocument document;

        private int insertionOffset;

        #endregion

        #region Свойства

        public int InsertionOffset
        {
            get { return insertionOffset; }
            set { insertionOffset = value; }
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        #endregion

        #region Конструктор

        public DocumentTextWriter(IDocument document, int insertionOffset)
        {
            this.insertionOffset = insertionOffset;
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            this.document = document;
            var line = document.GetLineByOffset(insertionOffset);
            if (line.DelimiterLength == 0)
            {
                line = line.PreviousLine;
            }
            if (line != null)
            {
                NewLine = document.GetText(line.EndOffset, line.DelimiterLength);
            }
        }

        #endregion

        #region Методы

        public override void Write(char value)
        {
            document.Insert(insertionOffset, value.ToString());
            insertionOffset++;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            document.Insert(insertionOffset, new string(buffer, index, count));
            insertionOffset += count;
        }

        public override void Write(string value)
        {
            document.Insert(insertionOffset, value);
            insertionOffset += value.Length;
        }

        #endregion
    }
}