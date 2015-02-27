using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class ListParselet : INonPrefixParselet
    {
        public bool ConsumeToken { get { return true; } }
        public List<IExpression> Values { get; private set; }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            Values = new List<IExpression> {left};
            do
            {
                Values.Add(parser.ParseExpression<ListExpression>(PrecedenceValues.List));
            } while (parser.Match(TokenType.Comma));
            return new ListExpression(Values);
        }

        public int Precedence
        {
            get { return PrecedenceValues.List; }
        }
    }
}
