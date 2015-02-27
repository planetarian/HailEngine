using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class EntityBodyParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {

            var components = new List<ComponentExpression>();
            GroupExpression groupExpression = null;

            while (!parser.Match(TokenType.RightBrace))
            {
                IExpression expression = parser.ParseExpression<EntityBodyExpression>();
                var component = expression as ComponentExpression;
                var group = expression as GroupExpression;

                if (component != null)
                    components.Add(component);
                else if (group != null)
                {
                    if (groupExpression != null)
                        throw new ParseException(
                            token.Position,
                            "Template body: Group already set");
                    groupExpression = group;
                }
                else
                    throw new ParseException(
                        token.Position,
                        "Template body: Body may only contain group and components. Unexpected " +
                        expression);
            }
            return new EntityBodyExpression(groupExpression, components);
        }
    }
}
