using System;
using System.Collections.Generic;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Hail.Helpers;
using Microsoft.Xna.Framework;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 10000, IsResizable = true, ResizeSize = 10000, IsSupportMultiThread = true)]
    public class TransformComponent : HailComponent
    {
        // TODO public Vector3 Origin;
        [ComponentProperty(typeof (Vector3), 0)]
        public Vector3 Position { get; set; }

        [ComponentProperty(typeof (Quaternion), 0, 0, 0, 1)]
        public Quaternion Rotation { get; set; }

        [ComponentProperty(typeof (Vector3), 1)]
        public Vector3 Scale { get; set; }

        //public bool Updated;

        public Matrix ScaleMatrix
        {
            get { return Matrix.CreateScale(Scale); }
        }

        public Matrix RotationMatrix
        {
            get { return Matrix.CreateFromQuaternion(Rotation); }
        }

        public Matrix PositionMatrix
        {
            get { return Matrix.CreateTranslation(Position); }
        }

        public Matrix World
        {
            get { return ScaleMatrix*RotationMatrix*PositionMatrix; }
        }

        public BoundingBox BoundingBox
        {
            // TODO: scale properly
            get { return new BoundingBox(Position - (Scale*10), Position + (Scale*10)); }
        }
    }
}