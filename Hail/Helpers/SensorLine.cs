using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hail.Helpers
{
    public struct SensorLine
    {
        public Vector3 Offset { get; set; }
        public Vector3 Vector { get; set; }
        public SensorLine(Vector3 offset, Vector3 vector)
            : this()
        {
            Offset = offset;
            Vector = vector;
        }
    }
}
