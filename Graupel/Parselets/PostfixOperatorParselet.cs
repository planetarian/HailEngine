using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class PostfixOperatorParselet : INonPrefixParselet
    {
        public bool ConsumeToken { get { return true; } }
        public int Precedence { get; private set; }

        public PostfixOperatorParselet(int precedence)
        {
            Precedence = precedence;
        }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            return new PostfixExpression(left, token.Type);
        }
    }
}
