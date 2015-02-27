using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Graupel.Lexer;
using Graupel.Parselets;

namespace Graupel
{
    public class GraupelParser : Parser
    {
        public DocumentExpression ParseDocument()
        {
            GlobalExpression global = null;
            var scenes = new List<SceneExpression>();

            while (true)
            {
                IExpression expression = ParseExpression<DocumentExpression>();
                var globalExpression = expression as GlobalExpression;
                var sceneExpression = expression as SceneExpression;
                var eofExpression = expression as EofExpression;

                if (sceneExpression != null)
                {
                    scenes.Add(sceneExpression);
                }
                else if (globalExpression != null)
                {
                    if (global != null)
                        throw new ParseException(
                            Position.None, "Document: Can only contain one Global object.");
                    global = globalExpression;
                }
                else if (eofExpression != null)
                {
                    return new DocumentExpression(global, scenes);
                }
                else
                {
                    throw new ParseException(Position.None, "Document: Unexpected " + expression);
                }
            }
        }

        public GraupelParser(ITokenReader reader) : base(reader)
        {
            Register(TokenType.Global, new GlobalParselet());
            Register(TokenType.Scene, new SceneParselet());
            Register(TokenType.Template, new TemplateParselet());
            Register(TokenType.Entity, new EntityParselet());
            Register(TokenType.Group, new GroupParselet());
            Register(TokenType.Identifier, new IdentifierParselet());
            Register(TokenType.Identifier, new ComponentParselet(),
                typeof(EntityBodyExpression));
            Register(TokenType.LeftBrace, new EntityBodyParselet());
            Register(TokenType.LeftParen, new ExpressionGroupParselet());
            Register(TokenType.LeftParen, new FunctionParselet());
            Register(TokenType.Question, new TernaryParselet());
            Register(TokenType.Colon, new AssignParselet());
            Register(TokenType.Eval, new EvalParselet());

            Register(TokenType.String, new StringParselet());
            Register(TokenType.Integer, new IntegerParselet());
            Register(TokenType.Float, new FloatParselet());
            Register(TokenType.Boolean, new BoolParselet());

            Register(TokenType.Identifier, new CompoundValueParselet(),
                typeof(AssignExpression), typeof(ListExpression));
            Register(TokenType.String, new CompoundValueParselet(),
                typeof(AssignExpression), typeof(ListExpression));
            Register(TokenType.Integer, new CompoundValueParselet(),
                typeof(AssignExpression), typeof(ListExpression));
            Register(TokenType.Float, new CompoundValueParselet(),
                typeof(AssignExpression), typeof(ListExpression));
            Register(TokenType.Boolean, new CompoundValueParselet(),
                typeof(AssignExpression), typeof(ListExpression));


            Register(TokenType.Comma, new ListParselet());

            InfixLeft(TokenType.Plus, PrecedenceValues.Sum);
            InfixLeft(TokenType.Minus, PrecedenceValues.Sum);
            InfixLeft(TokenType.Asterisk, PrecedenceValues.Product);
            InfixLeft(TokenType.Slash, PrecedenceValues.Product);
            InfixLeft(TokenType.Percent, PrecedenceValues.Product);
            InfixLeft(TokenType.Caret, PrecedenceValues.Exponent);

            Prefix(TokenType.Bang, PrecedenceValues.Prefix);
            Postfix(TokenType.SemiColon, PrecedenceValues.End);
        }

        public void Postfix(TokenType type, int precedence)
        {
            Register(type, new PostfixOperatorParselet(precedence));
        }

        public void Prefix(TokenType type, int precedence)
        {
            Register(type, new PrefixOperatorParselet(precedence));
        }

        public void InfixLeft(TokenType type, int precedence)
        {
            Register(type, new BinaryOperatorParselet(precedence, false));
        }

        public void InfixRight(TokenType type, int precedence)
        {
            Register(type, new BinaryOperatorParselet(precedence, true));
        }

    }
}
