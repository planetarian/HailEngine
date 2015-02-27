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
    [ArtemisComponentPool(InitialSize = 1000, IsResizable= true, ResizeSize = 1000, IsSupportMultiThread = true)]
    public class LookAtComponent : HailComponent
    {
        /// <summary>
        /// Artemis Tag string for the target to be looked at.
        /// </summary>
        [ComponentProperty("")]
        public string TargetTag { get; set; }

        /// <summary>
        /// Speed in full rotations per second.
        /// </summary>
        [ComponentProperty(1)]
        public float Speed { get; set; }

        /// <summary>
        /// Vector for the camera to use as 'up'
        /// </summary>
        [ComponentProperty(typeof(Vector3), 0, 1, 0)]
        public Vector3 UpVector { get; set; }

        /// <summary>
        /// Whether to match the up vector with the target's.
        /// </summary>
        [ComponentProperty(false)]
        public bool UpVectorMatchTarget { get; set; }
    }
}