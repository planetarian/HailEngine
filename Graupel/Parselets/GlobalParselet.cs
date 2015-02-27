using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;

namespace Graupel.Parselets
{
    public class GlobalParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            if (parser.Match(TokenType.SemiColon))
                return new GlobalExpression(new List<TemplateExpression>());
            if (parser.Match(TokenType.LeftBrace))
            {
                var templates = new List<TemplateExpression>();
                while (!parser.Match(TokenType.RightBrace))
                {
                    IExpression expression = parser.ParseExpression<GlobalExpression>();
                    var template = expression as TemplateExpression;
                    if (template == null)
                        throw new ParseException(
                            token.Position,
                            "Global: Body may only contain templates. Unexpected " +
                            expression);
                    templates.Add(template);
                }
                return new GlobalExpression(templates);
            }
            throw new ParseException(
                token.Position, "Global: Syntax error. Unexpected " + parser.Consume());
        }
    }
}
