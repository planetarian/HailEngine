using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class EntityParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            var templates = new List<string>();

            // name
            //var idExpression = parser.ParseExpression() as IdentifierExpression;
            //if (idExpression == null)
            var name = new Token(Position.None, TokenType.Identifier, "", null);
            if (parser.LookAhead().Type == TokenType.Identifier)
                name = parser.Consume();
                // name is optional for entities
                //throw new ParseException(token.Position, "Entity object must have a name.");
            string printedName = (String.IsNullOrEmpty(name.Text) ? "" : " " + name.Text);

            // template includes
            if (parser.Match(TokenType.Colon))
            {
                do
                {
                    IExpression expression = parser.ParseExpression<EntityExpression>();
                    var include = expression as IdentifierExpression;
                    if (include == null)
                        throw new ParseException(
                            token.Position,
                            "Entity" + printedName +
                            ": Include list contains " + expression.GetType().Name);
                    templates.Add(include.Name);
                } while (parser.Match(TokenType.Comma));
            }

            // empty template
            if (parser.Match(TokenType.SemiColon))
                return new EntityExpression(name.Text, templates,
                    new EntityBodyExpression(null, new List<ComponentExpression>()));

            // body
            IExpression expr = parser.ParseExpression<EntityExpression>();
            var bodyExpression = expr as EntityBodyExpression;
            if (bodyExpression == null)
                throw new ParseException(
                    token.Position, "Entity" + printedName +
                                    ": Body syntax error. Unexpected " + expr);

            return new EntityExpression(
                name.Text, templates, bodyExpression);
        }
    }
}
