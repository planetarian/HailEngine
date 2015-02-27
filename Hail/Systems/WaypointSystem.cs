using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Hail.Components;
using Hail.Helpers;
using Microsoft.Xna.Framework;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 10)]
    public class WaypointSystem : ParallelEntityProcessingSystem
    {

        public WaypointSystem()
            : base(
                Aspect.All(typeof (WaypointFollowerComponent), typeof (MovementComponent), typeof (TransformComponent)))
        {
        }
        public override void Process(Entity e)
        {
            // TODO: rewrite this piece of shit =/

            var follower = e.GetComponent<WaypointFollowerComponent>();
            if (follower.WaypointTags.Count == 0)
                return; // No waypoints to follow, just stay still.

            // Reset before the loop so we don't have issues reaching it again later
            int count = follower.WaypointTags.Count;
            if (follower.CurrentWaypointIndex >= count)
            {
                if (!follower.Loop && !follower.Reverse)
                    return;
                if (follower.Reverse)
                    follower.Reversing = true;
                follower.CurrentWaypointIndex = follower.Reverse ? follower.WaypointTags.Count - 1 : 0;
            }
            else if (follower.CurrentWaypointIndex < 0) // Reverse must inherently be true
            {
                if (!follower.Loop)
                    return;
                follower.CurrentWaypointIndex = 0;
                follower.Reversing = false;
            }

            Entity waypoint = entityWorld.TagManager.GetEntity(follower.WaypointTags[follower.CurrentWaypointIndex]);
            TransformComponent wayTrans = null;
            if (waypoint != null)
                wayTrans = waypoint.GetComponent<TransformComponent>();
            int start = follower.CurrentWaypointIndex;
            for (int current = 0; wayTrans == null && current < count; current++)
            {
                if (!follower.Reversing)
                {
                    follower.CurrentWaypointIndex++;
                    if (follower.CurrentWaypointIndex >= count)
                    {
                        if (follower.Reverse)
                        {
                            follower.CurrentWaypointIndex = start - 1;
                            follower.Reversing = true;
                        }
                        else if (!follower.Loop)
                            return;
                        else
                            follower.CurrentWaypointIndex = 0;
                    }
                }
                else
                {
                    follower.CurrentWaypointIndex--;
                    if (follower.CurrentWaypointIndex < 0)
                    {
                        if (!follower.Loop)
                            return;
                        follower.CurrentWaypointIndex = start + 1;
                        follower.Reversing = false;
                    }
                }
                waypoint = entityWorld.TagManager.GetEntity(follower.WaypointTags[follower.CurrentWaypointIndex]);
                if (waypoint != null)
                    wayTrans = waypoint.GetComponent<TransformComponent>();
            }

            if (wayTrans == null) // no valid waypoint, do nothing
                return;


            // I got to move it move it

            var transform = e.GetComponent<TransformComponent>();
            var move = e.GetComponent<MovementComponent>();
            var wayMove = waypoint.GetComponent<MovementComponent>();
            Vector3 wayGoalPos = wayTrans.Position + (wayMove != null ? wayMove.PositionDelta : Vector3.Zero);
            float speed = follower.Speed/1000*entityWorld.Delta;


            Vector3 toMove = VectorHelper.Move(transform.Position, wayGoalPos, speed) - transform.Position;
            move.PositionDelta += toMove;

            if (follower.RotateToFaceWaypoint)
            {
                Vector3 goalPos = transform.Position + move.PositionDelta;
                float maxAngle =
                    HandyMath.ScaleValue(follower.RotationSpeed, 1, MathHelper.Pi)/1000*
                    entityWorld.Delta;
                Quaternion goalRotation =
                    QuaternionHelper.Face(transform.Rotation, goalPos, wayGoalPos, maxAngle);
                move.RotationDelta = Quaternion.Inverse(transform.Rotation)*
                                         goalRotation;
            }

            float dist = (wayTrans.Position - transform.Position).Length();
            if (dist <= follower.WaypointReachDistance)
                follower.CurrentWaypointIndex += follower.Reversing ? -1 : 1;
        }
    }
}