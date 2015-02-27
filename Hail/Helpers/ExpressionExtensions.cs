using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Expressions;
using Hail.GraupelSemantics;
using Microsoft.Xna.Framework;

namespace Hail.Helpers
{
    public static class ExpressionExtensions
    {
        public static List<T> ToList<T>(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return ((List<object>) expression.Accept(visitor)).Select(o => (T) o).ToList();
        }

        public static string ToString(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return (string) expression.Accept(visitor);
        }

        public static bool ToBool(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return (bool)expression.Accept(visitor);
        }

        public static float ToFloat(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return HandyMath.ToFloat(expression.Accept(visitor));
        }

        public static int ToInt(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return HandyMath.ToInt(expression.Accept(visitor));
        }

        public static Vector2 ToVector2(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return HandyMath.ToVector2(expression.Accept(visitor));
        }

        public static Vector3 ToVector3(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return HandyMath.ToVector3(expression.Accept(visitor));
        }

        public static Quaternion ToQuaternion(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return HandyMath.ToQuaternion(expression.Accept(visitor));
        }

        public static Rectangle ToRectangle(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return HandyMath.ToRectangle(expression.Accept(visitor));
        }

        public static RectangleF ToRectangleF(this IExpression expression, IExpressionVisitor<object> visitor)
        {
            return HandyMath.ToRectangleF(expression.Accept(visitor));
        }

        public static object Translate(this IExpression expression, IExpressionVisitor<object> visitor, Type type)
        {
            if (type.Name == typeof(List<>).Name)
            {
                Type genericType = type
#if WINRT
                    .GenericTypeArguments[0];
#else
                    .GetGenericArguments()[0];
#endif
                if (genericType == typeof(int))
                    return expression.ToList<int>(visitor);
                if (genericType == typeof(float))
                    return expression.ToList<float>(visitor);
                if (genericType == typeof(bool))
                    return expression.ToList<bool>(visitor);
                if (genericType == typeof(string))
                    return expression.ToList<string>(visitor);
                if (genericType == typeof(Vector2))
                    return expression.ToList<Vector2>(visitor);
                if (genericType == typeof(Vector3))
                    return expression.ToList<Vector3>(visitor);
                if (genericType == typeof(Quaternion))
                    return expression.ToList<Quaternion>(visitor);
                if (genericType == typeof(Rectangle))
                    return expression.ToList<Rectangle>(visitor);
                if (genericType == typeof(RectangleF))
                    return expression.ToList<RectangleF>(visitor);
            }
            else
                return HandyMath.Translate(type, expression.Accept(visitor));
            throw new ArgumentException("Can't translate to the given type.");
        }
    }
}
