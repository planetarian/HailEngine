using System;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Hail.Components;
using Microsoft.Xna.Framework;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 70)]
    public class MovementSystem : ParallelEntityProcessingSystem
    {
        public MovementSystem()
            : base(Aspect.All(typeof (MovementComponent), typeof (TransformComponent)))
        {
        }

        public override void Process(Entity e)
        {
            var movement = e.GetComponent<MovementComponent>();
            var transform = e.GetComponent<TransformComponent>();

            transform.Position += movement.PositionDelta;
            transform.Rotation *= movement.RotationDelta;

            movement.PositionDelta = Vector3.Zero;
            movement.RotationDelta = Quaternion.Identity;
        }
    }
}