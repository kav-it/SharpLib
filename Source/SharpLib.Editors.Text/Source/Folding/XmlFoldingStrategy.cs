using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Folding
{
    public class XmlFoldingStrategy
    {
        #region Свойства

        public bool ShowAttributesWhenFolded { get; set; }

        #endregion

        #region Методы

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            var foldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(foldings, firstErrorOffset);
        }

        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            try
            {
                var reader = new XmlTextReader(document.CreateReader());
                reader.XmlResolver = null;
                return CreateNewFoldings(document, reader, out firstErrorOffset);
            }
            catch (XmlException)
            {
                firstErrorOffset = 0;
                return Enumerable.Empty<NewFolding>();
            }
        }

        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, XmlReader reader, out int firstErrorOffset)
        {
            var stack = new Stack<XmlFoldStart>();
            var foldMarkers = new List<NewFolding>();
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (!reader.IsEmptyElement)
                            {
                                var newFoldStart = CreateElementFoldStart(document, reader);
                                stack.Push(newFoldStart);
                            }
                            break;

                        case XmlNodeType.EndElement:
                            var foldStart = stack.Pop();
                            CreateElementFold(document, foldMarkers, reader, foldStart);
                            break;

                        case XmlNodeType.Comment:
                            CreateCommentFold(document, foldMarkers, reader);
                            break;
                    }
                }
                firstErrorOffset = -1;
            }
            catch (XmlException ex)
            {
                if (ex.LineNumber >= 1 && ex.LineNumber <= document.LineCount)
                {
                    firstErrorOffset = document.GetOffset(ex.LineNumber, ex.LinePosition);
                }
                else
                {
                    firstErrorOffset = 0;
                }
            }
            foldMarkers.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return foldMarkers;
        }

        private static int GetOffset(TextDocument document, XmlReader reader)
        {
            var info = reader as IXmlLineInfo;
            if (info != null && info.HasLineInfo())
            {
                return document.GetOffset(info.LineNumber, info.LinePosition);
            }
            throw new ArgumentException("XmlReader does not have positioning information.");
        }

        private static void CreateCommentFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader)
        {
            string comment = reader.Value;
            if (comment != null)
            {
                int firstNewLine = comment.IndexOf('\n');
                if (firstNewLine >= 0)
                {
                    int startOffset = GetOffset(document, reader) - 4;
                    int endOffset = startOffset + comment.Length + 7;

                    string foldText = String.Concat("<!--", comment.Substring(0, firstNewLine).TrimEnd('\r'), "-->");
                    foldMarkers.Add(new NewFolding(startOffset, endOffset)
                    {
                        Name = foldText
                    });
                }
            }
        }

        private XmlFoldStart CreateElementFoldStart(TextDocument document, XmlReader reader)
        {
            var newFoldStart = new XmlFoldStart();

            var lineInfo = (IXmlLineInfo)reader;
            newFoldStart.StartLine = lineInfo.LineNumber;
            newFoldStart.StartOffset = document.GetOffset(newFoldStart.StartLine, lineInfo.LinePosition - 1);

            if (ShowAttributesWhenFolded && reader.HasAttributes)
            {
                newFoldStart.Name = String.Concat("<", reader.Name, " ", GetAttributeFoldText(reader), ">");
            }
            else
            {
                newFoldStart.Name = String.Concat("<", reader.Name, ">");
            }

            return newFoldStart;
        }

        private static void CreateElementFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader, XmlFoldStart foldStart)
        {
            var lineInfo = (IXmlLineInfo)reader;
            int endLine = lineInfo.LineNumber;
            if (endLine > foldStart.StartLine)
            {
                int endCol = lineInfo.LinePosition + reader.Name.Length + 1;
                foldStart.EndOffset = document.GetOffset(endLine, endCol);
                foldMarkers.Add(foldStart);
            }
        }

        private static string GetAttributeFoldText(XmlReader reader)
        {
            var text = new StringBuilder();

            for (int i = 0; i < reader.AttributeCount; ++i)
            {
                reader.MoveToAttribute(i);

                text.Append(reader.Name);
                text.Append("=");
                text.Append(reader.QuoteChar.ToString());
                text.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
                text.Append(reader.QuoteChar.ToString());

                if (i < reader.AttributeCount - 1)
                {
                    text.Append(" ");
                }
            }

            return text.ToString();
        }

        private static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
        {
            var encodedValue = new StringBuilder(attributeValue);

            encodedValue.Replace("&", "&amp;");
            encodedValue.Replace("<", "&lt;");
            encodedValue.Replace(">", "&gt;");

            if (quoteChar == '"')
            {
                encodedValue.Replace("\"", "&quot;");
            }
            else
            {
                encodedValue.Replace("'", "&apos;");
            }

            return encodedValue.ToString();
        }

        #endregion
    }
}