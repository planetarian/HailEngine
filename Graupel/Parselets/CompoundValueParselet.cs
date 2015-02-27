using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class CompoundValueParselet : INonPrefixParselet
    {
        public bool ConsumeToken { get { return false; } }
        public List<IExpression> Values { get; private set; }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            Values = new List<IExpression> {left};
            while (true)
            {
                switch (parser.LookAhead().Type)
                {
                    case TokenType.Identifier:
                    case TokenType.String:
                    case TokenType.Integer:
                    case TokenType.Float:
                    case TokenType.Boolean:
                    case TokenType.LeftParen:
                        Values.Add(parser.ParseExpression<CompoundValueExpression>(Precedence - 1));
                        break;
                    default:
                        return new CompoundValueExpression(Values);
                }
            }
        }

        public int Precedence
        {
            get { return PrecedenceValues.Compound; }
        }
    }
}
