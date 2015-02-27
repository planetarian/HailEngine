using System;
using System.Collections.Generic;
using System.Linq;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Hail.Helpers;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 10, IsResizable= true, ResizeSize = 50, IsSupportMultiThread = true)]
    public class WaypointFollowerComponent : HailComponent
    {
        /// <summary>
        /// Index of the waypoint being moved to currently.
        /// </summary>
        [ComponentProperty(0)]
        public int CurrentWaypointIndex { get; set; }

        /// <summary>
        /// Whether to move back to the first waypoint after reaching the last.
        /// </summary>
        [ComponentProperty(false)]
        public bool Loop { get; set; }

        /// <summary>
        /// Whether to reverse direction upon reaching the last waypoint.
        /// </summary>
        [ComponentProperty(false)]
        public bool Reverse { get; set; }

        /// <summary>
        /// State value determining whether the follower is currently following the path in reverse.
        /// </summary>
        [ComponentProperty(false)]
        public bool Reversing { get; set; }

        /// <summary>
        /// How fast to move between waypoints, in units per second.
        /// </summary>
        [ComponentProperty(1)]
        public float Speed { get; set; }

        /// <summary>
        /// How close entity must be to a waypoint to consider it reached.
        /// </summary>
        [ComponentProperty(0.001f)]
        public float WaypointReachDistance { get; set; }

        /// <summary>
        /// Whether to rotate the follower object to face the next waypoint.
        /// </summary>
        [ComponentProperty(false)]
        public bool RotateToFaceWaypoint { get; set; }

        /// <summary>
        /// Speed in full rotations per second.
        /// </summary>
        [ComponentProperty(1)]
        public float RotationSpeed { get; set; }

        /// <summary>
        /// List of tags representing waypoint entities.
        /// </summary>
        [ComponentProperty]
        public List<String> WaypointTags { get; set; }
    }
}