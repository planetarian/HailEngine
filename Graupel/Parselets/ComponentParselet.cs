using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class ComponentParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            string name = token.Text;
            var assignments = new List<AssignExpression>();
            
            // default
            if (parser.Match(TokenType.SemiColon))
                return new ComponentExpression(name, assignments);

            // block assignment
            if (parser.Match(TokenType.LeftBrace))
            {
                while (!parser.Match(TokenType.RightBrace))
                {
                    IExpression expression = parser.ParseExpression<ComponentExpression>();

                    var assign = expression as AssignExpression;
                    if (assign != null)
                        assignments.Add(assign);
                    else
                        throw new ParseException(
                            token.Position, "Component " + name +
                                            ": Body may only contain assignments. Unexpected " +
                                            expression);
                }
                return new ComponentExpression(name, assignments);
            }

            // shorthand assignment
            if (parser.LookAhead().Type==TokenType.Identifier)
            {
                IExpression expression = parser.ParseExpression<ComponentExpression>();
                var assign = expression as AssignExpression;
                if (assign != null)
                {
                    assignments.Add(assign);
                    return new ComponentExpression(name, assignments);
                }
                else
                    throw new ParseException(
                        token.Position, "Component " + name +
                                        ": Body may only contain assignments. Unexpected " +
                                        expression);
            }

            throw new ParseException(
                token.Position, "Component " + name +
                                ": Syntax error. Unexpected " + parser.Consume());
        }
    }
}
