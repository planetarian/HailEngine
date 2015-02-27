using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hail.Helpers
{
    public struct RectangleF
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectangleF(float x, float y, float width, float height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        public static RectangleF operator *(RectangleF rect, Vector2 vect)
        {
            return new RectangleF(rect.X*vect.X, rect.Y*vect.Y, rect.Width*vect.X, rect.Height*vect.Y);
        }
    }
}
