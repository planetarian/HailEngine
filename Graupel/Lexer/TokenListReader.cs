using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Lexer
{
    public class TokenListReader : ITokenReader
    {
        private readonly List<Token> tokens;
        private int index;

        public TokenListReader(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Token ReadToken()
        {
            return index >= tokens.Count
                ? new Token(Position.None, TokenType.EOF, "\0", null)
                : tokens[index++];
        }
    }
}
