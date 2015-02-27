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
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 60)]
    public class SnapSystem : ParallelEntityProcessingSystem
    {
        public SnapSystem()
            : base(Aspect.All(typeof(SnapComponent), typeof(MovementComponent), typeof(TransformComponent)))
        {
        }

        public override void Process(Entity e)
        {
            var snap = e.GetComponent<SnapComponent>();
            var move = e.GetComponent<MovementComponent>();
            var trans = e.GetComponent<TransformComponent>();

            Vector3 targPos = trans.Position + move.PositionDelta;
            float x = (targPos.X/snap.Increment).ToInt()*snap.Increment;
            float y = (targPos.Y/snap.Increment).ToInt()*snap.Increment;
            float z = (targPos.Z/snap.Increment).ToInt()*snap.Increment;
            targPos = new Vector3(x, y, z);
            move.PositionDelta = targPos - trans.Position;
        }

    }
}
