using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class TemplateParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            var templates = new List<string>();

            // name
            //var idExpression = parser.ParseExpression() as IdentifierExpression;
            //if (idExpression == null)
            Token name = parser.Consume();
            if (name.Type != TokenType.Identifier)
                throw new ParseException(token.Position, "Template object must have a name.");

            // template includes
            if (parser.Match(TokenType.Colon))
            {
                do
                {
                    IExpression expression = parser.ParseExpression<TemplateExpression>();
                    var include = expression as IdentifierExpression;
                    if (include == null)
                        throw new ParseException(
                            token.Position,
                            "Template " + name.Text +
                            ": Include list contains " + expression.GetType().Name);
                    templates.Add(include.Name);
                } while (parser.Match(TokenType.Comma));
            }

            // empty template
            if (parser.Match(TokenType.SemiColon))
                return new TemplateExpression(name.Text, templates,
                    new EntityBodyExpression(null, new List<ComponentExpression>()));

            // body
            IExpression expr = parser.ParseExpression<TemplateExpression>();
            var bodyExpression = expr as EntityBodyExpression;
            if (bodyExpression == null)
                throw new ParseException(
                    token.Position, "Template " + name.Text +
                                    ": Body syntax error. Unexpected " + expr);
            return new TemplateExpression(
                name.Text, templates, bodyExpression);
            
        }
    }
}
