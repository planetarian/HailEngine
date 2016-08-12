using System;
using System.Text.RegularExpressions;
using Graupel.Util;

namespace Graupel.Lexer
{
    public class Token
    {
        public Position Position { get; private set; }
        public TokenType Type { get; private set; }
        public string Text { get; private set; }
        public object Value { get; private set; }

        public Token(Position position, TokenType type, string text, Object value)
        {
            Expect.NotNull(position, type, text);

            Position = position;
            Type = type;
            Text = text;
            Value = value;
        }

        public bool IsType(TokenType type)
        {
            return Type == type;
        }

        public override string ToString() => $"({Type}) {Regex.Escape(Text)}";
    }
}
