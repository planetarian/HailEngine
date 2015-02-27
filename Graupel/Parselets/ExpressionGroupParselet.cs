using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class ExpressionGroupParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            IExpression expression = parser.ParseExpression<AssignExpression>();
            parser.Consume(TokenType.RightParen);
            return expression;
        }
    }
}
