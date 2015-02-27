using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public interface IPrefixParselet
    {
        IExpression Parse(Parser parser, Token token);
    }
}
