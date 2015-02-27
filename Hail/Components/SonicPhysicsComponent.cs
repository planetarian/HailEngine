using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Hail.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 1000, IsResizable = true, ResizeSize = 1000, IsSupportMultiThread = true)]
    public class SonicPhysicsComponent : HailComponent
    {
        // Hard values

        [ComponentProperty(0)]
        public float XSpeed { get; set; }

        [ComponentProperty(0)]
        public float YSpeed { get; set; }

        [ComponentProperty(0)]
        public float GroundSpeed { get; set; }

        // Base Speed

        [ComponentProperty(0.046875f)]
        public float Acceleration { get; set; }

        [ComponentProperty(0.5f)]
        public float Deceleration { get; set; }

        [ComponentProperty(6f)]
        public float TopSpeed { get; set; }

        // Ground/Running

        [ComponentProperty(0.046875f)]
        public float Friction { get; set; }

        [ComponentProperty(0.125f)]
        public float SlopeFactorRunning { get; set; }

        [ComponentProperty(0.078125f)]
        public float SlopeFactorRollingUphill { get; set; }

        [ComponentProperty(0.3125f)]
        public float SlopeFactorRollingDownhill { get; set; }

        // Midair

        [ComponentProperty(0.21875f)]
        public float Gravity { get; set; }

        [ComponentProperty(6.5f)]
        public float JumpSpeed { get; set; }

        [ComponentProperty(4f)]
        public float JumpReleaseSpeed { get; set; }

        [ComponentProperty(0.96875f)]
        public float AirDrag { get; set; }

        // Rolling

        [ComponentProperty(1.03125f)]
        public float MinRollStartSpeed { get; set; }

        [ComponentProperty(0.5f)]
        public float MinRollSpeed { get; set; }

        // State

        [ComponentProperty(false)]
        public bool Midair { get; set; }

        [ComponentProperty(false)]
        public bool Underwater { get; set; }

        [ComponentProperty(false)]
        public bool Rolling { get; set; }

        [ComponentProperty(false)]
        public bool JumpedWhileRolling { get; set; }

        [ComponentProperty(40)]
        public float StandHeight { get; set; }

        [ComponentProperty(30)]
        public float RollHeight { get; set; }

        public bool LeftSensorCollision { get; set; }
        public bool RightSensorCollision { get; set; }
        public bool HoriSensorCollision { get; set; }
        public bool JustLanded { get; set; }

    }
}
