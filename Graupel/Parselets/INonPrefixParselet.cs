using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public interface INonPrefixParselet
    {
        IExpression Parse(Parser parser, IExpression left, Token token);
        int Precedence { get; }
        bool ConsumeToken { get; }
    }
}
