using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class StringParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            string text = token.Text.Substring(1, token.Text.Length - 2); // trim ""
            return new StringExpression(text);
        }
    }
}
