using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Hail.Helpers;
using Microsoft.Xna.Framework;


namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 100, IsResizable = true, ResizeSize = 1000, IsSupportMultiThread = true)]
    public class AttachmentComponent : HailComponent
    {

        /// <summary>
        /// Tag representing the entity to attach to.
        /// </summary>
        [ComponentProperty("")]
        public string TargetTag { get; set; }


        /// <summary>
        /// Whether to attach this entity's position to the target's.
        /// </summary>
        [ComponentProperty(false)]
        public bool PositionAttach { get; set; }

        /// <summary>
        /// Offset from attach target's position.
        /// If OffsetRelative is true, this will be rotated by the attach target's current rotation.
        /// </summary>
        [ComponentProperty(typeof (Vector3), 0, 0, 0)]
        public Vector3 PositionOffset { get; set; }

        /// <summary>
        /// Whether to rotate the Offset value by the target's current rotation.
        /// </summary>
        [ComponentProperty(false)]
        public bool PositionOffsetRelative { get; set; }


        /// <summary>
        /// Whether to attach this entity's rotation to the target's.
        /// </summary>
        [ComponentProperty(false)]
        public bool RotationAttach { get; set; }

        /// <summary>
        /// Offset from attach target's rotation.
        /// </summary>
        [ComponentProperty(typeof (Quaternion), 0, 0, 0, 1)]
        public Quaternion RotationOffset { get; set; }


    }
}