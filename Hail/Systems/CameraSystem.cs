using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Hail.Components;
using Microsoft.Xna.Framework;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 80)]
    public class CameraSystem : ParallelEntityProcessingSystem
    {
        public CameraSystem()
            : base(Aspect.All(typeof (CameraComponent), typeof (TransformComponent)))
        {
        }

        public override void Process(Entity e)
        {
            var camera = e.GetComponent<CameraComponent>();
            var transform = e.GetComponent<TransformComponent>();

            Matrix rotationMatrix = Matrix.CreateFromQuaternion(transform.Rotation);
            Vector3 forwardPosition = transform.Position + rotationMatrix.Forward;

            camera.ViewMatrix = Matrix.CreateLookAt(transform.Position, forwardPosition, rotationMatrix.Up);
        }
    }
}