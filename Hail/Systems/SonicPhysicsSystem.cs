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
using Microsoft.Xna.Framework.Input;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 10)]
    public class SonicPhysicsSystem : ParallelEntityProcessingSystem
    {
        private KeyboardState prevKeyState;

        public SonicPhysicsSystem()
            : base(Aspect.All(
                typeof (SonicPhysicsComponent),
                typeof (TransformComponent),
                typeof (MovementComponent)
                //, typeof (ModelComponent)
            ))
        {
            prevKeyState = Keyboard.GetState();
        }

        public override void Process(Entity e)
        {
            KeyboardState keyState = Keyboard.GetState();
            var p = e.GetComponent<SonicPhysicsComponent>();
            //var move = e.GetComponent<MovementComponent>();
            //var trans = e.GetComponent<TransformComponent>();
            //var model = e.GetComponent<ModelComponent>();

            // Can only accelerate when not rolling, track it for friction
            bool accelerating = false;

            if (!p.JumpedWhileRolling)
            {
                // Left key held
                if (keyState.IsKeyDown(Keys.Left) && keyState.IsKeyUp(Keys.Right))
                {
                    // Change directions
                    if (p.XSpeed > 0 && !p.Midair)
                        p.XSpeed -= HandyMath.FromFixedStep((p.Rolling ? .25f : 1) * p.Deceleration, entityWorld.Delta);
                        // Accelerate
                    else if (p.XSpeed > -p.TopSpeed && (p.Midair || !p.Rolling))
                    {
                        accelerating = true;
                        p.XSpeed -= HandyMath.FromFixedStep((p.Midair ? 2 : 1) * p.Acceleration, entityWorld.Delta);
                    }
                        // Cap top speed
                    else if (!p.Rolling)
                        p.XSpeed = -p.TopSpeed;
                }

                    // Right key held
                else if (keyState.IsKeyDown(Keys.Right) && keyState.IsKeyUp(Keys.Left))
                {
                    if (p.XSpeed < 0 && !p.Midair)
                        p.XSpeed += HandyMath.FromFixedStep((p.Rolling ? .25f : 1) * p.Deceleration, entityWorld.Delta);
                    else if (p.XSpeed < p.TopSpeed && (p.Midair || !p.Rolling))
                    {
                        accelerating = true;
                        p.XSpeed += HandyMath.FromFixedStep((p.Midair ? 2 : 1) * p.Acceleration, entityWorld.Delta);
                    }
                    else if (!p.Rolling)
                        p.XSpeed = p.TopSpeed;
                }
            }

            // Friction only when we're on the ground and not accelerating
            if (!p.Midair && !accelerating)
            {
                p.XSpeed -= Math.Min(Math.Abs(p.XSpeed),
                                     HandyMath.FromFixedStep((p.Rolling ? .5f : 1) * p.Friction, entityWorld.Delta)) *
                            Math.Sign(p.XSpeed);
            }

            // Currently in mid-air
            if (p.Midair)
            {
                // Air drag
                if (p.YSpeed > 0 && p.YSpeed < 4)
                {
                    if (Math.Abs(p.XSpeed) >= 0.125)
                    {
                        float airDrag = 1 - ((1 - p.AirDrag)*60/(1000f/entityWorld.Delta));
                        p.XSpeed = p.XSpeed*airDrag;
                        //p.XSpeed -= ((int)(p.XSpeed/.125)/256f);
                    }
                }

                // Gravity
                p.YSpeed -= HandyMath.FromFixedStep(p.Gravity, entityWorld.Delta);
                float maxYspeed = -16;
                p.YSpeed = Math.Max(maxYspeed, p.YSpeed);

                // Released jump
                if (keyState.IsKeyUp(Keys.Up) && prevKeyState.IsKeyDown(Keys.Up)
                    && p.YSpeed > p.JumpReleaseSpeed)
                {
                    p.YSpeed = p.JumpReleaseSpeed;
                }

                // Previous ground collision point
            }

            // Jumping doesn't actually do anything until next frame, so you can cancel immediately
            if (!p.Midair && keyState.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up))
            {
                p.YSpeed = 6.5f;
                p.Midair = true;
                p.JumpedWhileRolling = p.Rolling;
                p.Rolling = true;
            }

            // Rolling
            if (!p.Rolling && !p.Midair)
                RollCheck(p, keyState.IsKeyDown(Keys.Down) && (prevKeyState.IsKeyUp(Keys.Down) || p.JustLanded));


            prevKeyState = keyState;
        }

        public static void RollCheck(SonicPhysicsComponent p, bool isKeyDown)
        {
            if (!p.Midair && !p.Rolling && isKeyDown && Math.Abs(p.XSpeed) > p.MinRollStartSpeed)
                p.Rolling = true;
            else if (p.Rolling && Math.Abs(p.XSpeed) < p.MinRollSpeed)
                p.Rolling = false;
        }

    }
}
