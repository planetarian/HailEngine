using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class SceneParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            var templates = new List<TemplateExpression>();
            var entities = new List<EntityExpression>();
            var includes = new List<string>();

            // name
            //var idExpression = parser.ParseExpression() as IdentifierExpression;
            //if (idExpression == null)
            Token name = parser.Consume(TokenType.Identifier);

            // scene includes
            if (parser.Match(TokenType.Colon))
            {
                do
                {
                    IExpression expression = parser.ParseExpression<SceneExpression>();
                    var include = expression as IdentifierExpression;
                    if (include != null)
                        includes.Add(include.Name);
                    else
                        throw new ParseException(
                            token.Position,
                            "Scene " + name.Text +
                            ": Include list contains " + expression.GetType().Name);
                } while (parser.Match(TokenType.Comma));
            }

            // empty scene
            if (parser.Match(TokenType.SemiColon))
                return new SceneExpression(name.Text, includes, templates, entities);

            if (parser.Match(TokenType.LeftBrace))
            {
                while (!parser.Match(TokenType.RightBrace))
                {
                    IExpression expression = parser.ParseExpression<SceneExpression>();
                    var template = expression as TemplateExpression;
                    var entity = expression as EntityExpression;

                    if (template != null)
                        templates.Add(template);
                    else if (entity != null)
                        entities.Add(entity);
                    else
                        throw new ParseException(
                            token.Position,
                            "Scene " + name.Text +
                            ": Body may only contain templates and entities. Unexpected " +
                            expression);
                }
                return new SceneExpression(name.Text, includes, templates, entities);
            }

            throw new ParseException(
                token.Position, "Scene " + name.Text +
                                ": Syntax error. Unexpected " + parser.Consume());
        }
    }
}
