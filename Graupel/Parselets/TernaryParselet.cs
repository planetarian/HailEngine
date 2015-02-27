using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class TernaryParselet : INonPrefixParselet
    {
        public bool ConsumeToken { get { return true; } }
        public int Precedence
        {
            get { return PrecedenceValues.Ternary; }
        }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            IExpression trueExpression = parser.ParseExpression<TernaryExpression>(PrecedenceValues.Ternary);
            parser.Consume(TokenType.Colon);
            IExpression falseExpression = parser.ParseExpression<TernaryExpression>(PrecedenceValues.Ternary - 1);
            return new TernaryExpression(left, trueExpression, falseExpression);
        }
    }
}
