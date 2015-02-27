using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class EvalParselet : IPrefixParselet
    {
        public int Precedence { get { return PrecedenceValues.Assignment; } }

        public IExpression Parse(Parser parser, Token token)
        {
            // block assignment
            IExpression expression = parser.ParseExpression<AssignExpression>(Precedence - 1);
            return new EvalExpression(expression);
        }
    }
}
