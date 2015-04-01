using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SharpLib.Notepad.Indentation.CSharp
{
    internal sealed class IndentationReformatter
    {
        #region Поля

        private Block block;

        private bool blockComment;

        private Stack<Block> blocks;

        private bool escape;

        private bool inChar;

        private bool inString;

        private char lastRealChar;

        private bool lineComment;

        private bool verbatim;

        private StringBuilder wordBuilder;

        #endregion

        #region Методы

        public void Reformat(IDocumentAccessor doc, IndentationSettings set)
        {
            Init();

            while (doc.MoveNext())
            {
                Step(doc, set);
            }
        }

        public void Init()
        {
            wordBuilder = new StringBuilder();
            blocks = new Stack<Block>();
            block = new Block();
            block.InnerIndent = "";
            block.OuterIndent = "";
            block.Bracket = '{';
            block.Continuation = false;
            block.LastWord = "";
            block.OneLineBlock = 0;
            block.PreviousOneLineBlock = 0;
            block.StartLine = 0;

            inString = false;
            inChar = false;
            verbatim = false;
            escape = false;

            lineComment = false;
            blockComment = false;

            lastRealChar = ' ';
        }

        public void Step(IDocumentAccessor doc, IndentationSettings set)
        {
            string line = doc.Text;
            if (set.LeaveEmptyLines && line.Length == 0)
            {
                return;
            }
            line = line.TrimStart();

            var indent = new StringBuilder();
            if (line.Length == 0)
            {
                if (blockComment || (inString && verbatim))
                {
                    return;
                }
                indent.Append(block.InnerIndent);
                indent.Append(Repeat(set.IndentString, block.OneLineBlock));
                if (block.Continuation)
                {
                    indent.Append(set.IndentString);
                }
                if (doc.Text != indent.ToString())
                {
                    doc.Text = indent.ToString();
                }
                return;
            }

            if (TrimEnd(doc))
            {
                line = doc.Text.TrimStart();
            }

            var oldBlock = block;
            bool startInComment = blockComment;
            bool startInString = (inString && verbatim);

            #region Parse char by char

            lineComment = false;
            inChar = false;
            escape = false;
            if (!verbatim)
            {
                inString = false;
            }

            lastRealChar = '\n';

            char lastchar = ' ';
            char c = ' ';
            char nextchar = line[0];
            for (int i = 0; i < line.Length; i++)
            {
                if (lineComment)
                {
                    break;
                }

                lastchar = c;
                c = nextchar;
                if (i + 1 < line.Length)
                {
                    nextchar = line[i + 1];
                }
                else
                {
                    nextchar = '\n';
                }

                if (escape)
                {
                    escape = false;
                    continue;
                }

                #region Check for comment/string chars

                switch (c)
                {
                    case '/':
                        if (blockComment && lastchar == '*')
                        {
                            blockComment = false;
                        }
                        if (!inString && !inChar)
                        {
                            if (!blockComment && nextchar == '/')
                            {
                                lineComment = true;
                            }
                            if (!lineComment && nextchar == '*')
                            {
                                blockComment = true;
                            }
                        }
                        break;
                    case '#':
                        if (!(inChar || blockComment || inString))
                        {
                            lineComment = true;
                        }
                        break;
                    case '"':
                        if (!(inChar || lineComment || blockComment))
                        {
                            inString = !inString;
                            if (!inString && verbatim)
                            {
                                if (nextchar == '"')
                                {
                                    escape = true;
                                    inString = true;
                                }
                                else
                                {
                                    verbatim = false;
                                }
                            }
                            else if (inString && lastchar == '@')
                            {
                                verbatim = true;
                            }
                        }
                        break;
                    case '\'':
                        if (!(inString || lineComment || blockComment))
                        {
                            inChar = !inChar;
                        }
                        break;
                    case '\\':
                        if ((inString && !verbatim) || inChar)
                        {
                            escape = true;
                        }
                        break;
                }

                #endregion

                if (lineComment || blockComment || inString || inChar)
                {
                    if (wordBuilder.Length > 0)
                    {
                        block.LastWord = wordBuilder.ToString();
                    }
                    wordBuilder.Length = 0;
                    continue;
                }

                if (!Char.IsWhiteSpace(c) && c != '[' && c != '/')
                {
                    if (block.Bracket == '{')
                    {
                        block.Continuation = true;
                    }
                }

                if (Char.IsLetterOrDigit(c))
                {
                    wordBuilder.Append(c);
                }
                else
                {
                    if (wordBuilder.Length > 0)
                    {
                        block.LastWord = wordBuilder.ToString();
                    }
                    wordBuilder.Length = 0;
                }

                #region Push/Pop the blocks

                switch (c)
                {
                    case '{':
                        block.ResetOneLineBlock();
                        blocks.Push(block);
                        block.StartLine = doc.LineNumber;
                        if (block.LastWord == "switch")
                        {
                            block.Indent(set.IndentString + set.IndentString);
                        }
                        else
                        {
                            block.Indent(set);
                        }
                        block.Bracket = '{';
                        break;
                    case '}':
                        while (block.Bracket != '{')
                        {
                            if (blocks.Count == 0)
                            {
                                break;
                            }
                            block = blocks.Pop();
                        }
                        if (blocks.Count == 0)
                        {
                            break;
                        }
                        block = blocks.Pop();
                        block.Continuation = false;
                        block.ResetOneLineBlock();
                        break;
                    case '(':
                    case '[':
                        blocks.Push(block);
                        if (block.StartLine == doc.LineNumber)
                        {
                            block.InnerIndent = block.OuterIndent;
                        }
                        else
                        {
                            block.StartLine = doc.LineNumber;
                        }
                        block.Indent(Repeat(set.IndentString, oldBlock.OneLineBlock) +
                                     (oldBlock.Continuation ? set.IndentString : "") +
                                     (i == line.Length - 1 ? set.IndentString : new String(' ', i + 1)));
                        block.Bracket = c;
                        break;
                    case ')':
                        if (blocks.Count == 0)
                        {
                            break;
                        }
                        if (block.Bracket == '(')
                        {
                            block = blocks.Pop();
                            if (IsSingleStatementKeyword(block.LastWord))
                            {
                                block.Continuation = false;
                            }
                        }
                        break;
                    case ']':
                        if (blocks.Count == 0)
                        {
                            break;
                        }
                        if (block.Bracket == '[')
                        {
                            block = blocks.Pop();
                        }
                        break;
                    case ';':
                    case ',':
                        block.Continuation = false;
                        block.ResetOneLineBlock();
                        break;
                    case ':':
                        if (block.LastWord == "case"
                            || line.StartsWith("case ", StringComparison.Ordinal)
                            || line.StartsWith(block.LastWord + ":", StringComparison.Ordinal))
                        {
                            block.Continuation = false;
                            block.ResetOneLineBlock();
                        }
                        break;
                }

                if (!Char.IsWhiteSpace(c))
                {
                    lastRealChar = c;
                }

                #endregion
            }

            #endregion

            if (wordBuilder.Length > 0)
            {
                block.LastWord = wordBuilder.ToString();
            }
            wordBuilder.Length = 0;

            if (startInString)
            {
                return;
            }
            if (startInComment && line[0] != '*')
            {
                return;
            }
            if (doc.Text.StartsWith("//\t", StringComparison.Ordinal) || doc.Text == "//")
            {
                return;
            }

            if (line[0] == '}')
            {
                indent.Append(oldBlock.OuterIndent);
                oldBlock.ResetOneLineBlock();
                oldBlock.Continuation = false;
            }
            else
            {
                indent.Append(oldBlock.InnerIndent);
            }

            if (indent.Length > 0 && oldBlock.Bracket == '(' && line[0] == ')')
            {
                indent.Remove(indent.Length - 1, 1);
            }
            else if (indent.Length > 0 && oldBlock.Bracket == '[' && line[0] == ']')
            {
                indent.Remove(indent.Length - 1, 1);
            }

            if (line[0] == ':')
            {
                oldBlock.Continuation = true;
            }
            else if (lastRealChar == ':' && indent.Length >= set.IndentString.Length)
            {
                if (block.LastWord == "case" || line.StartsWith("case ", StringComparison.Ordinal) || line.StartsWith(block.LastWord + ":", StringComparison.Ordinal))
                {
                    indent.Remove(indent.Length - set.IndentString.Length, set.IndentString.Length);
                }
            }
            else if (lastRealChar == ')')
            {
                if (IsSingleStatementKeyword(block.LastWord))
                {
                    block.OneLineBlock++;
                }
            }
            else if (lastRealChar == 'e' && block.LastWord == "else")
            {
                block.OneLineBlock = Math.Max(1, block.PreviousOneLineBlock);
                block.Continuation = false;
                oldBlock.OneLineBlock = block.OneLineBlock - 1;
            }

            if (doc.IsReadOnly)
            {
                if (!oldBlock.Continuation && oldBlock.OneLineBlock == 0 &&
                    oldBlock.StartLine == block.StartLine &&
                    block.StartLine < doc.LineNumber && lastRealChar != ':')
                {
                    indent.Length = 0;
                    line = doc.Text;
                    for (int i = 0; i < line.Length; ++i)
                    {
                        if (!Char.IsWhiteSpace(line[i]))
                        {
                            break;
                        }
                        indent.Append(line[i]);
                    }

                    if (startInComment && indent.Length > 0 && indent[indent.Length - 1] == ' ')
                    {
                        indent.Length -= 1;
                    }
                    block.InnerIndent = indent.ToString();
                }
                return;
            }

            if (line[0] != '{')
            {
                if (line[0] != ')' && oldBlock.Continuation && oldBlock.Bracket == '{')
                {
                    indent.Append(set.IndentString);
                }
                indent.Append(Repeat(set.IndentString, oldBlock.OneLineBlock));
            }

            if (startInComment)
            {
                indent.Append(' ');
            }

            if (indent.Length != (doc.Text.Length - line.Length) ||
                !doc.Text.StartsWith(indent.ToString(), StringComparison.Ordinal) ||
                Char.IsWhiteSpace(doc.Text[indent.Length]))
            {
                doc.Text = indent + line;
            }
        }

        private static string Repeat(string text, int count)
        {
            if (count == 0)
            {
                return string.Empty;
            }
            if (count == 1)
            {
                return text;
            }
            var b = new StringBuilder(text.Length * count);
            for (int i = 0; i < count; i++)
            {
                b.Append(text);
            }
            return b.ToString();
        }

        private static bool IsSingleStatementKeyword(string keyword)
        {
            switch (keyword)
            {
                case "if":
                case "for":
                case "while":
                case "do":
                case "foreach":
                case "using":
                case "lock":
                    return true;
                default:
                    return false;
            }
        }

        private static bool TrimEnd(IDocumentAccessor doc)
        {
            string line = doc.Text;
            if (!Char.IsWhiteSpace(line[line.Length - 1]))
            {
                return false;
            }

            if (line.EndsWith("// ", StringComparison.Ordinal) || line.EndsWith("* ", StringComparison.Ordinal))
            {
                return false;
            }

            doc.Text = line.TrimEnd();
            return true;
        }

        #endregion

        #region Вложенный класс: Block

        private struct Block
        {
            #region Поля

            public char Bracket;

            public bool Continuation;

            public string InnerIndent;

            public string LastWord;

            public int OneLineBlock;

            public string OuterIndent;

            public int PreviousOneLineBlock;

            public int StartLine;

            #endregion

            #region Методы

            public void ResetOneLineBlock()
            {
                PreviousOneLineBlock = OneLineBlock;
                OneLineBlock = 0;
            }

            public void Indent(IndentationSettings set)
            {
                Indent(set.IndentString);
            }

            public void Indent(string indentationString)
            {
                OuterIndent = InnerIndent;
                InnerIndent += indentationString;
                Continuation = false;
                ResetOneLineBlock();
                LastWord = "";
            }

            public override string ToString()
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "[Block StartLine={0}, LastWord='{1}', Continuation={2}, OneLineBlock={3}, PreviousOneLineBlock={4}]",
                    StartLine, LastWord, Continuation, OneLineBlock, PreviousOneLineBlock);
            }

            #endregion
        }

        #endregion
    }
}