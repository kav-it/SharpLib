
using System;

namespace SharpLib.Log
{
    internal enum ConditionTokenType
    {
        EndOfInput,

        BeginningOfInput,

        Number,

        String,

        Keyword,

        Whitespace,

        FirstPunct,

        LessThan,

        GreaterThan,

        LessThanOrEqualTo,

        GreaterThanOrEqualTo,

        EqualTo,

        NotEqual,

        LeftParen,

        RightParen,

        Dot,

        Comma,

        Not,

        And,

        Or,

        Minus,

        LastPunct,

        Invalid,

        ClosingCurlyBrace,

        Colon,

        Exclamation,

        Ampersand,

        Pipe,
    }
}
