using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class FunctionParselet : INonPrefixParselet
    {
        public bool ConsumeToken { get { return true; } }
        public int Precedence { get { return PrecedenceValues.Function; } }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            var idExpression = left as IdentifierExpression;
            if (idExpression == null) 
                throw new ParseException(
                    token.Position, "Function: unexpected left-side " + left);

            var args = new List<IExpression>();
            if (!parser.Match(TokenType.RightParen))
            {
                do
                {
                    args.Add(parser.ParseExpression<FunctionExpression>(PrecedenceValues.List));
                } while (parser.Match(TokenType.Comma));
                parser.Consume(TokenType.RightParen);
            }
            return new FunctionExpression(idExpression, args);
        }

    }
}
