using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class AssignParselet : INonPrefixParselet
    {
        public int Precedence { get { return PrecedenceValues.Assignment; } }
        public bool ConsumeToken { get { return true; } }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            IExpression right = parser.ParseExpression<AssignExpression>(Precedence - 1);
            if (!(left is IdentifierExpression)) throw new ParseException(
                Position.None, "The left-hand side of an assignment must be an identifier.");
            string name = ((IdentifierExpression) left).Name;
            parser.Consume(TokenType.SemiColon);
            return new AssignExpression(name, right);
        }
    }
}
