using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Hail.Helpers;
using Microsoft.Xna.Framework;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 1000, IsResizable = true, ResizeSize = 1000, IsSupportMultiThread = true)]
    public class MovementComponent : HailComponent
    {
        [ComponentProperty(typeof (Vector3), 0)]
        public Vector3 PositionDelta { get; set; }

        [ComponentProperty(typeof (Quaternion), 0, 0, 0, 1)]
        public Quaternion RotationDelta { get; set; }

        [ComponentProperty(typeof (Vector3), 0)]
        public Vector3 ScaleDelta { get; set; }
    }
}