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
    [ArtemisComponentPool(InitialSize = 10, IsResizable = true, ResizeSize = 20, IsSupportMultiThread = true)]
    public class CameraComponent : HailComponent
    {
        // PiOver4
        [ComponentProperty(0.785398163397448f)]
        public float FieldOfView { get; set; }

        [ComponentProperty(1)]
        public float NearPlaneDistance { get; set; }

        [ComponentProperty(2500)]
        public float FarPlaneDistance { get; set; }

        [ComponentProperty(Settable = false)]
        public Matrix ProjectionMatrix { get; set; }

        [ComponentProperty(Settable = false)]
        public Matrix ViewMatrix { get; set; }

        public BoundingFrustum Frustum
        {
            get { return new BoundingFrustum(ViewMatrix*ProjectionMatrix); }
        }
    }
}