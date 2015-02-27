using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class BinaryOperatorParselet : INonPrefixParselet
    {
        public bool ConsumeToken { get { return true; } }
        public int Precedence { get; private set; }
        public bool IsRight { get; private set; }

        public BinaryOperatorParselet(int precedence, bool isRight)
        {
            Precedence = precedence;
            IsRight = isRight;
        }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            IExpression right = parser.ParseExpression<OperatorExpression>(Precedence - (IsRight ? 1 : 0));
            return new OperatorExpression(left, token, right);
        }

    }
}
