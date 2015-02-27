using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Hail.Helpers;
using Microsoft.Xna.Framework;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 5, IsResizable = true, ResizeSize = 5, IsSupportMultiThread = true)]
    public class ZoomComponent : HailComponent
    {
        [ComponentProperty(.2f)]
        public float Amount { get; set; }

        [ComponentProperty(1)]
        public float Smoothing { get; set; }

        [ComponentProperty(typeof (Vector3), 0)]
        public Vector3 TargetPos { get; set; }

        [ComponentProperty(50)]
        public float MinZoomLevel { get; set; }

        [ComponentProperty(1000)]
        public float MaxZoomLevel { get; set; }
    }
}
