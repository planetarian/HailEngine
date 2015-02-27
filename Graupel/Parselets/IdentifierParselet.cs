using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class IdentifierParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            return new IdentifierExpression(token.Text);
        }
    }
}
