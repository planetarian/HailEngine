using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.System;
using Graupel;
using Graupel.Lexer;
using Graupel.Expressions;
using Hail.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.GraupelSemantics
{
    public class GraupelExpressionVisitor : IExpressionVisitor<object>
    {
        public static readonly GraupelExpressionVisitor Visitor
            = new GraupelExpressionVisitor();

        private GraupelExpressionVisitor()
        {}

        private readonly Random random = new Random();

        public object Visit(FunctionExpression expression, string context)
        {
            if (expression.Function.Name == "rand")
            {
                if (expression.ValueType == typeof(float))
                {
                    if (expression.Args.Count == 0)
                        return (float)random.NextDouble();
                    if (expression.Args.Count == 2)
                    {
                        var min = HandyMath.ToFloat(expression.Args[0].Accept(this, context));
                        var max = HandyMath.ToFloat(expression.Args[1].Accept(this, context));
                        return (float) random.NextDouble()*(max - min) + min;
                    }
                    throw new InvalidOperationException("rand: Invalid argument count.");
                }
                if (expression.ValueType == typeof(int))
                {
                    if (expression.Args.Count == 1)
                    {
                        var max = (int)expression.Args[0].Accept(this, context);
                        // NOTE: Max is INCLUSIVE. Note that Random.Next(int,int) has Max as EXCLUSIVE.
                        return random.Next(0, max + 1);
                    }
                    if (expression.Args.Count == 2)
                    {
                        var min = (int) expression.Args[0].Accept(this, context);
                        var max = (int) expression.Args[1].Accept(this, context);
                        // NOTE: Max is INCLUSIVE. Note that Random.Next(int,int) has Max as EXCLUSIVE.
                        return random.Next(min, max + 1);
                    }
                    throw new InvalidOperationException("rand: Invalid argument count.");
                }
            }
            throw new InvalidOperationException("Unknown function or incorrect parameters: " + expression.Function.Name);
        }

        public object Visit(IdentifierExpression expression, string context)
        {
            if (expression.Name == "screenwidth")
            {
                var graphics = EntitySystem.BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
                return Math.Max(graphics.PresentationParameters.Bounds.Width, graphics.PresentationParameters.Bounds.Height);
            }
            if (expression.Name == "screenheight")
            {
                var graphics = EntitySystem.BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
                return Math.Min(graphics.PresentationParameters.Bounds.Width, graphics.PresentationParameters.Bounds.Height);
            }
            if (expression.Name == "isphone")
            {
#if WINDOWS_PHONE
                return true;
#else
                return false;
#endif
            }
            return expression.Name;
        }

        public object Visit(BoolExpression expression, string context)
        {
            return expression.Value;
        }

        public object Visit(CompoundValueExpression expression, string context)
        {
            // List<object>
            return expression.Expressions.Select(expr => expr.Accept(this, context)).ToList();
        }

        public object Visit(FloatExpression expression, string context)
        {
            return expression.Value;
        }

        public object Visit(IntegerExpression expression, string context)
        {
            return expression.Value;
        }
        
        public object Visit(ListExpression expression, string context)
        {
            // Get list of expressions
            var list = expression.Expressions.Select(expr => expr.Accept(this, context)).ToList();
            return list;
        }

        public object Visit(StringExpression expression, string context)
        {
            return expression.Value;
        }

        public object Visit(OperatorExpression expression, string context)
        {
            TokenType oper = expression.Operator.Type;
            object leftObj = expression.LeftExpression.Accept(this, context);
            object rightObj = expression.RightExpression.Accept(this, context);
            Type opType = HandyMath.OperationType(leftObj, rightObj);

            if (opType == typeof(int) || opType == typeof(float))
            {
                var left = HandyMath.ToFloat(leftObj);
                var right = HandyMath.ToFloat(rightObj);
                float result;

                if (oper == TokenType.Asterisk)
                    result = left*right;
                else if (oper == TokenType.Slash)
                    result = left/right;
                else if (oper == TokenType.Plus)
                    result = left + right;
                else if (oper == TokenType.Minus)
                    result = left - right;
                else if (oper == TokenType.Caret)
                    result = (float) Math.Pow(left, right);
                else if (oper == TokenType.Percent)
                    result = left%right;
                else
                    throw new InvalidOperationException(
                        "Invalid operator token " + oper);

                if (opType == typeof(int))
                    result = (int) result;

                return result;
            }

            if (opType == typeof(string))
            {
                string left = leftObj.ToString();
                string right = rightObj.ToString();

                if (oper == TokenType.Plus)
                    return left + right;

                throw new InvalidOperationException(
                    "Invalid operator token " + oper);
            }

            if (opType == typeof(Vector3))
            {
                Vector3 left = HandyMath.ToVector3(leftObj);
                Vector3 right = HandyMath.ToVector3(rightObj);

                if (oper == TokenType.Asterisk)
                    return left * right;
                if (oper == TokenType.Slash)
                    return left / right;
                if (oper == TokenType.Plus)
                    return left + right;
                if (oper == TokenType.Minus)
                    return left - right;

                throw new InvalidOperationException(
                    "Invalid operator token " + oper);
            }

            throw new InvalidOperationException(
                "Don't know how to operate on a " + expression);
        }

        public object Visit(TernaryExpression expression, string context)
        {
            if (expression.Condition.ValueType != typeof(bool))
                throw new InvalidOperationException("Ternary operation consition must evaluate to a boolean value.");

            var condition = (bool)expression.Condition.Accept(this, context);

            return condition
                ? expression.TrueExpression.Accept(this, context)
                : expression.FalseExpression.Accept(this, context);
        }

        public object Visit(PrefixExpression expression, string context)
        {
            if (expression.OperatorType == TokenType.Bang)
            {
                if (expression.RightExpression.ValueType != typeof(bool))
                    throw new InvalidOperationException(
                        "Negate operator can only be used with a boolean value.");
                return !(bool)expression.RightExpression.Accept(this, context);
            }
            throw new InvalidOperationException(
                "Unrecognized prefix operator.");
        }

        public object Visit(EvalExpression expression, string context)
        {
            return expression.Value.Accept(this, context);
        }

        #region not implemented

        public object Visit(DocumentExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(GlobalExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(SceneExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(EntityBaseExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(TemplateExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(EntityExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(EntityBodyExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(GroupExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(ComponentExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(AssignExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(EofExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        public object Visit(PostfixExpression expression, string context)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
