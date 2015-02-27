using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class GroupParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            //if (!parser.Match(TokenType.Identifier))
                //throw new ParseException(
                    //token.Position,
                    //"Group: Invalid syntax. Unexpected " + parser.ParseExpression(typeof(GroupExpression)));

            IExpression expression = parser.ParseExpression<GroupExpression>(PrecedenceValues.Assignment);
            var stringExpression = expression as StringExpression;
            if (stringExpression == null) 
                throw new ParseException(
                    token.Position,
                    "Group: Value must be a string literal. Unexpected " + expression);
            parser.Consume(TokenType.SemiColon);

            return new GroupExpression(stringExpression.Value);
        }
    }
}
