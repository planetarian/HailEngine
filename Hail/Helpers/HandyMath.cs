using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.Helpers
{
    public static class HandyMath
    {
        private const float fixedRate = 60;


        public static int ToInt(this float f)
        {
            return (int) (f > 0 ? f + 0.5f : f - 0.5f);
        }

        public static float ScaleValue(float value, float valueScale, float targetScale)
        {
            if (valueScale == 0 || value == 0)
                return 0;
            return targetScale/valueScale*value;
        }

        public static float FastAsin(float x)
        {
            const float scaleFactor = .391f; //empirical
            float x5 = x*x; //x^2
            x5 *= x5; //x^4
            x5 *= x; //x^5
            return x + scaleFactor*x5;
        }

        public static float FastAcos(float x)
        {
            return MathHelper.PiOver2 - FastAsin(x);
        }

        public static bool IsNumeric(object o)
        {
            return o is int || o is float;
        }

        public static bool IsStringCompatible(object o)
        {
            return o is string || IsNumeric(o);
        }

        public static bool IsVector3Compatible(object o)
        {
            return o is Vector3 || IsNumeric(o);
        }

        public static Type OperationType(object o1, object o2)
        {
            if (o1 is int && o2 is int)
                return typeof (int);
            if (IsNumeric(o1) && IsNumeric(o2))
                return typeof (float);
            if (IsStringCompatible(o1) && IsStringCompatible(o2))
                return typeof (string);
            if (IsVector3Compatible(o1) && IsVector3Compatible(o2))
                return typeof (Vector3);
            return null;
        }

        public static float ToFloat(object value)
        {
            if (value is int)
                return (int) value;
            return (float) value;
        }

        public static int ToInt(object value)
        {
            if (value is float)
                return (int) (float) value;
            return (int) value;
        }

        public static Vector2 ToVector2(object o)
        {
            if (IsNumeric(o))
                return new Vector2(ToFloat(o));

            var items = o as IList<object>;
            if (items != null)
            {
                if (items.Count == 1)
                {
                    return new Vector2(ToFloat(items[0]));
                }
                if (items.Count == 2)
                {
                    List<float> v = items.Select(ToFloat).ToList();
                    return new Vector2(v[0], v[1]);
                }
                throw new InvalidOperationException(
                    "Incorrect number of parameters for vector2");
            }

            return (Vector2) o;
        }

        public static Vector3 ToVector3(object o)
        {
            if (IsNumeric(o))
                return new Vector3(ToFloat(o));

            var items = o as IList<object>;
            if (items != null)
            {
                if (items.Count == 1)
                {
                    return new Vector3(ToFloat(items[0]));
                }
                if (items.Count == 3)
                {
                    List<float> v = items.Select(ToFloat).ToList();
                    return new Vector3(v[0], v[1], v[2]);
                }
                throw new InvalidOperationException(
                    "Incorrect number of parameters for vector3");
            }

            return (Vector3) o;
        }

        public static Quaternion ToQuaternion(object o)
        {
            var items = o as IList<object>;
            if (items == null)
                throw new InvalidOperationException(
                    "Cannot create quaternion from type " + o.GetType());

            // axis angle
            if (items.Count == 3)
            {
                var axis = (string) items[0];
                if (axis.Length != 1 || "xyz".IndexOf(axis.ToLower()[0]) == -1)
                    throw new InvalidOperationException("invalid axis");
                var axisVector = new Vector3((axis == "x" ? 1 : 0), (axis == "y" ? 1 : 0), (axis == "z" ? 1 : 0));

                var angletype = (string) items[2];

                float angle;
                if (angletype == "rad")
                    angle = ToFloat(items[1]);
                else if (angletype == "deg")
                    angle = MathHelper.ToRadians(ToFloat(items[1]));
                else
                    throw new InvalidOperationException("Invalid angle type: must be 'rad' or 'deg'.");

                return Quaternion.CreateFromAxisAngle(axisVector, angle);
            }
            else if (items.Count == 4)
            {
                List<float> v = items.Select(ToFloat).ToList();
                return new Quaternion(v[0], v[1], v[2], v[3]);
            }
            throw new ArgumentException("Cannot create quaternion from the given arguments.");
        }

        public static Rectangle ToRectangle(object o)
        {
            var items = o as IList<object>;
            if (items == null)
                throw new InvalidOperationException(
                    "Cannot create rectangle from type " + o.GetType());

            if (items.Count == 4)
            {
                List<int> v = items.Select(ToInt).ToList();
                return new Rectangle(v[0], v[1], v[2], v[3]);
            }
            throw new ArgumentException("Cannot create rectangle from the given arguments.");
        }

        public static RectangleF ToRectangleF(object o)
        {
            var items = o as IList<object>;
            if (items == null)
                throw new InvalidOperationException(
                    "Cannot create vector4 from type " + o.GetType());

            if (items.Count == 4)
            {
                List<float> v = items.Select(ToFloat).ToList();
                return new RectangleF(v[0], v[1], v[2], v[3]);
            }
            throw new ArgumentException("Cannot create vector4 from the given arguments.");
        }

        public static object Translate(Type valueType, object value)
        {
            if (valueType == typeof (int))
                return ToInt(value);
            if (valueType == typeof (float))
                return ToFloat(value);
            if (valueType == typeof (string))
                return value;
            if (valueType == typeof (bool))
                return (bool) value;
            if (valueType == typeof (Vector2))
                return ToVector2(value);
            if (valueType == typeof (Vector3))
                return ToVector3(value);
            if (valueType == typeof (Quaternion))
                return ToQuaternion(value);
            if (valueType == typeof (Rectangle))
                return ToRectangle(value);
            if (valueType == typeof (RectangleF))
                return ToRectangleF(value);
            throw new ArgumentException(
                "Type '" + valueType.Name + "' not supported via argument-based assignment.");
        }

        public static float FromFixedStep(float value, float delta)
        {
            return HandyMath.ScaleValue(value, 1000 / fixedRate, delta);
        }

        public static float FromFixedStep(float value, float delta, float rate)
        {
            return HandyMath.ScaleValue(value, 1000 / rate, delta);
        }
    }
}