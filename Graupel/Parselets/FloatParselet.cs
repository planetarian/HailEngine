using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class FloatParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            return new FloatExpression((float)token.Value);
        }
    }
}
