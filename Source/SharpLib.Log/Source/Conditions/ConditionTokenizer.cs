using System;
using System.Text;

using NLog.Internal;

namespace NLog.Conditions
{
    internal sealed class ConditionTokenizer
    {
        #region Поля

        private static readonly ConditionTokenType[] charIndexToTokenType = BuildCharIndexToTokenType();

        private readonly SimpleStringReader stringReader;

        #endregion

        #region Свойства

        public int TokenPosition { get; private set; }

        public ConditionTokenType TokenType { get; private set; }

        public string TokenValue { get; private set; }

        public string StringTokenValue
        {
            get
            {
                string s = TokenValue;

                return s.Substring(1, s.Length - 2).Replace("''", "'");
            }
        }

        #endregion

        #region Конструктор

        public ConditionTokenizer(SimpleStringReader stringReader)
        {
            this.stringReader = stringReader;
            TokenType = ConditionTokenType.BeginningOfInput;
            GetNextToken();
        }

        #endregion

        #region Методы

        public void Expect(ConditionTokenType tokenType)
        {
            if (TokenType != tokenType)
            {
                throw new ConditionParseException("Expected token of type: " + tokenType + ", got " + TokenType + " (" + TokenValue + ").");
            }

            GetNextToken();
        }

        public string EatKeyword()
        {
            if (TokenType != ConditionTokenType.Keyword)
            {
                throw new ConditionParseException("Identifier expected");
            }

            string s = TokenValue;
            GetNextToken();
            return s;
        }

        public bool IsKeyword(string keyword)
        {
            if (TokenType != ConditionTokenType.Keyword)
            {
                return false;
            }

            if (!TokenValue.Equals(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public bool IsEOF()
        {
            if (TokenType != ConditionTokenType.EndOfInput)
            {
                return false;
            }

            return true;
        }

        public bool IsNumber()
        {
            return TokenType == ConditionTokenType.Number;
        }

        public bool IsToken(ConditionTokenType tokenType)
        {
            return TokenType == tokenType;
        }

        public void GetNextToken()
        {
            if (TokenType == ConditionTokenType.EndOfInput)
            {
                throw new ConditionParseException("Cannot read past end of stream.");
            }

            SkipWhitespace();

            TokenPosition = TokenPosition;

            int i = PeekChar();
            if (i == -1)
            {
                TokenType = ConditionTokenType.EndOfInput;
                return;
            }

            char ch = (char)i;

            if (char.IsDigit(ch))
            {
                ParseNumber(ch);
                return;
            }

            if (ch == '\'')
            {
                ParseSingleQuotedString(ch);
                return;
            }

            if (ch == '_' || char.IsLetter(ch))
            {
                ParseKeyword(ch);
                return;
            }

            if (ch == '}' || ch == ':')
            {
                TokenType = ConditionTokenType.EndOfInput;
                return;
            }

            TokenValue = ch.ToString();

            if (ch == '<')
            {
                ReadChar();
                int nextChar = PeekChar();

                if (nextChar == '>')
                {
                    TokenType = ConditionTokenType.NotEqual;
                    TokenValue = "<>";
                    ReadChar();
                    return;
                }

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.LessThanOrEqualTo;
                    TokenValue = "<=";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.LessThan;
                TokenValue = "<";
                return;
            }

            if (ch == '>')
            {
                ReadChar();
                int nextChar = PeekChar();

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.GreaterThanOrEqualTo;
                    TokenValue = ">=";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.GreaterThan;
                TokenValue = ">";
                return;
            }

            if (ch == '!')
            {
                ReadChar();
                int nextChar = PeekChar();

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.NotEqual;
                    TokenValue = "!=";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.Not;
                TokenValue = "!";
                return;
            }

            if (ch == '&')
            {
                ReadChar();
                int nextChar = PeekChar();
                if (nextChar == '&')
                {
                    TokenType = ConditionTokenType.And;
                    TokenValue = "&&";
                    ReadChar();
                    return;
                }

                throw new ConditionParseException("Expected '&&' but got '&'");
            }

            if (ch == '|')
            {
                ReadChar();
                int nextChar = PeekChar();
                if (nextChar == '|')
                {
                    TokenType = ConditionTokenType.Or;
                    TokenValue = "||";
                    ReadChar();
                    return;
                }

                throw new ConditionParseException("Expected '||' but got '|'");
            }

            if (ch == '=')
            {
                ReadChar();
                int nextChar = PeekChar();

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.EqualTo;
                    TokenValue = "==";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.EqualTo;
                TokenValue = "=";
                return;
            }

            if (ch >= 32 && ch < 128)
            {
                ConditionTokenType tt = charIndexToTokenType[ch];

                if (tt != ConditionTokenType.Invalid)
                {
                    TokenType = tt;
                    TokenValue = new string(ch, 1);
                    ReadChar();
                    return;
                }

                throw new ConditionParseException("Invalid punctuation: " + ch);
            }

            throw new ConditionParseException("Invalid token: " + ch);
        }

        private static ConditionTokenType[] BuildCharIndexToTokenType()
        {
            CharToTokenType[] charToTokenType =
            {
                new CharToTokenType('(', ConditionTokenType.LeftParen),
                new CharToTokenType(')', ConditionTokenType.RightParen),
                new CharToTokenType('.', ConditionTokenType.Dot),
                new CharToTokenType(',', ConditionTokenType.Comma),
                new CharToTokenType('!', ConditionTokenType.Not),
                new CharToTokenType('-', ConditionTokenType.Minus)
            };

            var result = new ConditionTokenType[128];

            for (int i = 0; i < 128; ++i)
            {
                result[i] = ConditionTokenType.Invalid;
            }

            foreach (CharToTokenType cht in charToTokenType)
            {
                result[cht.Character] = cht.TokenType;
            }

            return result;
        }

        private void ParseSingleQuotedString(char ch)
        {
            int i;
            TokenType = ConditionTokenType.String;

            StringBuilder sb = new StringBuilder();

            sb.Append(ch);
            ReadChar();

            while ((i = PeekChar()) != -1)
            {
                ch = (char)i;

                sb.Append((char)ReadChar());

                if (ch == '\'')
                {
                    if (PeekChar() == '\'')
                    {
                        sb.Append('\'');
                        ReadChar();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (i == -1)
            {
                throw new ConditionParseException("String literal is missing a closing quote character.");
            }

            TokenValue = sb.ToString();
        }

        private void ParseKeyword(char ch)
        {
            int i;
            TokenType = ConditionTokenType.Keyword;

            StringBuilder sb = new StringBuilder();

            sb.Append(ch);

            ReadChar();

            while ((i = PeekChar()) != -1)
            {
                if ((char)i == '_' || (char)i == '-' || char.IsLetterOrDigit((char)i))
                {
                    sb.Append((char)ReadChar());
                }
                else
                {
                    break;
                }
            }

            TokenValue = sb.ToString();
        }

        private void ParseNumber(char ch)
        {
            int i;
            TokenType = ConditionTokenType.Number;
            StringBuilder sb = new StringBuilder();

            sb.Append(ch);
            ReadChar();

            while ((i = PeekChar()) != -1)
            {
                ch = (char)i;

                if (char.IsDigit(ch) || (ch == '.'))
                {
                    sb.Append((char)ReadChar());
                }
                else
                {
                    break;
                }
            }

            TokenValue = sb.ToString();
        }

        private void SkipWhitespace()
        {
            int ch;

            while ((ch = PeekChar()) != -1)
            {
                if (!char.IsWhiteSpace((char)ch))
                {
                    break;
                }

                ReadChar();
            }
        }

        private int PeekChar()
        {
            return stringReader.Peek();
        }

        private int ReadChar()
        {
            return stringReader.Read();
        }

        #endregion

        #region Вложенный класс: CharToTokenType

        private struct CharToTokenType
        {
            #region Поля

            public readonly char Character;

            public readonly ConditionTokenType TokenType;

            #endregion

            #region Конструктор

            public CharToTokenType(char character, ConditionTokenType tokenType)
            {
                Character = character;
                TokenType = tokenType;
            }

            #endregion
        }

        #endregion
    }
}