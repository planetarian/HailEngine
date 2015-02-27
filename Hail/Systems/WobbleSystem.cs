using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Hail.Components;
using Hail.Helpers;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 20)]
    public class WobbleSystem : ParallelEntityProcessingSystem
    {
        public WobbleSystem()
            : base(Aspect.All(typeof(WobbleComponent), typeof(TransformComponent), typeof(MovementComponent)))
        {
        }

        public override void Process(Entity e)
        {
            var wobble = e.GetComponent<WobbleComponent>();
            var movement = e.GetComponent<MovementComponent>();
            float period = wobble.Period*1000;
            wobble.Time += entityWorld.Delta;
            wobble.Time %= period;
            float x = HandyMath.ScaleValue(wobble.Time, period, MathHelper.TwoPi);
            //var y = (float)Math.Sin(x) / (amplitudeDivisor * wobble.Period) * wobble.Amplitude;
            var y = (float)Math.Sin(x) / wobble.Period * wobble.Amplitude;
            movement.PositionDelta += new Vector3(0, y, 0);
            //movement.PositionDelta.Y += y;
        }

    }
}
