using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Artemis.System;
using Artemis.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Hail.Components;
using Hail.Helpers;
using Microsoft.Xna.Framework.Input;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous,
        GameLoopType = GameLoopType.Update, Layer = 50)]
    public class SonicCollisionSystem : ParallelEntityProcessingSystem
    {
        public SonicCollisionSystem()
            : base(Aspect.All(typeof (SonicPhysicsComponent), typeof (CollisionComponent),
                              typeof (TransformComponent), typeof (MovementComponent)))
        {
        }

        public override void Process(Entity e)
        {


            var trans = e.GetComponent<TransformComponent>();
            var move = e.GetComponent<MovementComponent>();
            var phys = e.GetComponent<SonicPhysicsComponent>();
            var coll = e.GetComponent<CollisionComponent>();

            //trans.Scale = new Vector3(trans.Scale.X, (phys.Rolling ? phys.RollHeight : phys.StandHeight) / 20, trans.Scale.Z);

            phys.JustLanded = false;

            float newXDelta = HandyMath.FromFixedStep(phys.XSpeed, entityWorld.Delta);
            float newYDelta = HandyMath.FromFixedStep(phys.YSpeed, entityWorld.Delta);

            float newPosX = trans.Position.X + newXDelta;
            float newPosY = trans.Position.Y + newYDelta;


            // Set up collision sensors

            float ySensorOffset = 9;
            float ySensorStart = 0;
            float ySensorLength = 36;

            float xSensorHeight = -4;
            float xSensorLength = 10;

            float airXSensorHeight = 0;

            phys.LeftSensorCollision = false;
            phys.RightSensorCollision = false;
            phys.HoriSensorCollision = false;
            float leftHeight = 0;
            float rightHeight = 0;
            float wallX = 0;

            float yOffset = phys.StandHeight/2;// ((phys.Midair || phys.Rolling) ? phys.RollHeight : phys.StandHeight) / 2;

            if (newPosY - yOffset <= 0)
            {
                phys.LeftSensorCollision = true;
                phys.RightSensorCollision = true;
            }

            //
            // Vertical Detection
            //

            foreach (Entity c in coll.Collisions)
            {
                BoundingBox cbox = GetBoundingBox(c);

                if (phys.YSpeed <= 0)
                {

                    // Left
                    if (cbox.Min.X < newPosX - ySensorOffset &&
                        cbox.Max.X > newPosX - ySensorOffset &&
                        cbox.Max.Y < newPosY + ySensorStart &&
                        cbox.Max.Y > newPosY - ySensorLength)
                    {
                        phys.LeftSensorCollision = true;
                        leftHeight = Math.Max(leftHeight, cbox.Max.Y);
                    }

                    // Right
                    if (cbox.Min.X < newPosX + ySensorOffset &&
                        cbox.Max.X > newPosX + ySensorOffset &&
                        cbox.Max.Y < newPosY + ySensorStart &&
                        cbox.Max.Y > newPosY - ySensorLength)
                    {
                        phys.RightSensorCollision = true;
                        rightHeight = Math.Max(rightHeight, cbox.Max.Y);
                    }

                }
                else if (phys.YSpeed > 0)
                {
                    float height = cbox.Min.Y - yOffset*2;
                    // Left
                    if (cbox.Min.X < newPosX - ySensorOffset &&
                        cbox.Max.X > newPosX - ySensorOffset &&
                        cbox.Min.Y > newPosY + ySensorStart &&
                        cbox.Min.Y < newPosY + yOffset)
                    {
                        leftHeight = phys.LeftSensorCollision ? Math.Min(leftHeight, height) : height;
                        phys.LeftSensorCollision = true;
                    }

                    // Right
                    if (cbox.Min.X < newPosX + ySensorOffset &&
                        cbox.Max.X > newPosX + ySensorOffset &&
                        cbox.Min.Y > newPosY + ySensorStart &&
                        cbox.Min.Y < newPosY + yOffset)
                    {
                        rightHeight = phys.RightSensorCollision ? Math.Min(rightHeight, height) : height;
                        phys.RightSensorCollision = true;
                    }

                    if (phys.LeftSensorCollision || phys.RightSensorCollision)
                        phys.YSpeed = 0;
                }
            }


            float newHeight = Math.Max(leftHeight, rightHeight);
            if (phys.LeftSensorCollision || phys.RightSensorCollision)
            {
                if (!phys.Midair)
                {
                    newYDelta = newHeight - (trans.Position.Y - yOffset);
                }
                if (phys.Midair && newHeight > newPosY - yOffset)
                {
                    phys.YSpeed = 0;
                    phys.Midair = false;
                    newYDelta = newHeight - (trans.Position.Y - yOffset);
                    phys.JumpedWhileRolling = false;
                    phys.JustLanded = true;
                    phys.Rolling = false;
                }
            }
            else
            {
                phys.Midair = true;
            }

            newPosY = trans.Position.Y + newYDelta;

            //
            // Horizontal Detection
            //

            foreach (Entity c in coll.Collisions)
            {
                BoundingBox cbox = GetBoundingBox(c);

                if (phys.XSpeed != 0)
                {
                    float sensorHeight = phys.Midair ? airXSensorHeight : xSensorHeight;

                    // Hori
                    if (cbox.Min.X < newPosX + xSensorLength
                        && cbox.Max.X > newPosX - xSensorLength
                        && cbox.Min.Y < newPosY + sensorHeight
                        && cbox.Max.Y > newPosY + sensorHeight)
                    {

                        if (phys.XSpeed > 0) // Moving right
                        {
                            wallX = phys.HoriSensorCollision ? Math.Min(wallX, cbox.Min.X) : cbox.Min.X;
                        }
                        else // Moving left
                        {
                            wallX = phys.HoriSensorCollision ? Math.Max(wallX,cbox.Max.X) : cbox.Max.X;
                        }

                        if (phys.XSpeed > 0)
                        {
                            newXDelta = wallX - xSensorLength - trans.Position.X;
                        }
                        else
                        {
                            newXDelta = wallX + xSensorLength - trans.Position.X;
                        }

                        phys.HoriSensorCollision = true;
                        phys.XSpeed = 0;
                    }
                }
            }



            // Detected




            // Finalize motion
            move.PositionDelta = new Vector3(newXDelta, newYDelta, 0);
        }

        private BoundingBox GetBoundingBox(Entity e)
        {
            var transform = e.GetComponent<TransformComponent>();
            var movement = e.GetComponent<MovementComponent>();
            Vector3 cpos = transform.Position;
            Vector3 cscale = transform.Scale;
            if (movement != null)
            {
                cpos += movement.PositionDelta;
                cscale += movement.ScaleDelta;
            }
            Vector3 cmin = cpos - (cscale*10);
            Vector3 cmax = cpos + (cscale*10);
            return new BoundingBox(cmin, cmax);
        }
    }
}
