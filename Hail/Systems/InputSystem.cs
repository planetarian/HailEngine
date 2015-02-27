using System.Collections.Generic;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Hail.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 10)]
    public class InputSystem : EntitySystem
    {
        public InputSystem()
            : base(Aspect.All(typeof (InputComponent), typeof (MovementComponent)))
        {
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            KeyboardState kbState = Keyboard.GetState();


            foreach (Entity e in entities.Values)
            {
                var move = e.GetComponent<MovementComponent>();
                var trans = e.GetComponent<TransformComponent>();

                float moveAmount = .3f*entityWorld.Delta;
                float rotAmount = .002f*entityWorld.Delta;

                if (kbState.IsKeyDown(Keys.W))
                    move.PositionDelta += Vector3.Transform(Vector3.Forward*moveAmount, trans.Rotation);
                if (kbState.IsKeyDown(Keys.S))
                    move.PositionDelta += Vector3.Transform(Vector3.Backward*moveAmount, trans.Rotation);
                if (kbState.IsKeyDown(Keys.A))
                    move.PositionDelta += Vector3.Transform(Vector3.Left*moveAmount, trans.Rotation);
                if (kbState.IsKeyDown(Keys.D))
                    move.PositionDelta += Vector3.Transform(Vector3.Right*moveAmount, trans.Rotation);
                if (kbState.IsKeyDown(Keys.E))
                    move.PositionDelta += Vector3.Transform(Vector3.Up*moveAmount, trans.Rotation);
                if (kbState.IsKeyDown(Keys.Q))
                    move.PositionDelta += Vector3.Transform(Vector3.Down*moveAmount, trans.Rotation);
                if (kbState.IsKeyDown(Keys.Left))
                    move.RotationDelta *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotAmount);
                if (kbState.IsKeyDown(Keys.Right))
                    move.RotationDelta *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, -rotAmount);
                if (kbState.IsKeyDown(Keys.Down))
                    move.RotationDelta *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, rotAmount);
                if (kbState.IsKeyDown(Keys.Up))
                    move.RotationDelta *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, -rotAmount);
                if (kbState.IsKeyDown(Keys.F))
                {
                    move.PositionDelta = new Vector3(0, 0, 0) - trans.Position;
                    move.RotationDelta = Quaternion.Identity/trans.Rotation;
                }
            }
        }
    }
}