using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class PrefixOperatorParselet : IPrefixParselet
    {
        public int Precedence { get; private set; }

        public PrefixOperatorParselet(int precedence)
        {
            Precedence = precedence;
        }

        public IExpression Parse(Parser parser, Token token)
        {
            IExpression right = parser.ParseExpression(Precedence);

            return new PrefixExpression(token.Type, right);
        }
    }
}
