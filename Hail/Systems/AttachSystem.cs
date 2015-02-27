using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Artemis;
using Artemis.Attributes;
using Artemis.System;
using Artemis.Utils;
using Artemis.Manager;
using Hail.Components;
using Hail.Helpers;
using Microsoft.Xna.Framework;

#if WINDOWS_PHONE
using ParallelTasks;
#endif

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 60)]
    public class AttachSystem : EntitySystem
    {
        private Dictionary<String, Bag<Entity>> attachments;
        private object attsLock;
        private Dictionary<String, Bag<Entity>> lookAts;
        private object lookAtsLock;

        private Queue<Entity> updated;
        private object updatedLock;

        public AttachSystem()
            : base(Aspect.All(typeof (MovementComponent), typeof (TransformComponent)))
        {
            updated = new Queue<Entity>(100);
            updatedLock = new object();
            attachments = new Dictionary<string, Bag<Entity>>(100);
            attsLock = new object();
            lookAts = new Dictionary<string, Bag<Entity>>(100);
            lookAtsLock = new object();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            attachments.Clear();
            lookAts.Clear();

            Parallel.ForEach(entities.Values, e =>
                                                  {
                                                      String tag = e.Tag;
                                                      var attach = e.GetComponent<AttachmentComponent>();
                                                      var look = e.GetComponent<LookAtComponent>();

                                                      if (tag != null)
                                                      {
                                                          lock (updatedLock)
                                                          {
                                                              updated.Enqueue(e);
                                                          }
                                                      }

                                                      if (attach != null)
                                                      {
                                                          string targTag = attach.TargetTag;
                                                          if (targTag != null && targTag != tag)
                                                          {
                                                              lock (attsLock)
                                                              {
                                                                  if (!attachments.ContainsKey(targTag))
                                                                      attachments.Add(targTag, new Bag<Entity>(100));
                                                                  attachments[targTag].Add(e);
                                                              }
                                                          }
                                                      }

                                                      if (look != null)
                                                      {
                                                          string targTag = look.TargetTag;
                                                          if (targTag != null && targTag != tag)
                                                          {
                                                              lock (lookAtsLock)
                                                              {
                                                                  if (!lookAts.ContainsKey(targTag))
                                                                      lookAts.Add(targTag, new Bag<Entity>(100));
                                                                  lookAts[targTag].Add(e);
                                                              }
                                                          }
                                                      }
                                                  });

            while (updated.Count > 0) // TODO: better way to keep attachments from recursing
            {
                Entity e = updated.Dequeue();
                string tag = e.Tag;
                var targTrans = e.GetComponent<TransformComponent>();
                var targMove = e.GetComponent<MovementComponent>();
                Vector3 targGoalPos = targTrans.Position + targMove.PositionDelta;
                Quaternion targGoalRot = targTrans.Rotation*targMove.RotationDelta;

                if (tag == null)
                    continue;

                if (attachments.ContainsKey(tag))
                {
                    Bag<Entity> toUpdate = attachments[tag];
                    Parallel.ForEach(toUpdate, entity =>
                                                   {
                                                       var attach = entity.GetComponent<AttachmentComponent>();
                                                       var transform = entity.GetComponent<TransformComponent>();
                                                       var movement = entity.GetComponent<MovementComponent>();
                                                       if (attach.PositionAttach)
                                                       {
                                                           Vector3 goal = targGoalPos +
                                                                          (attach.PositionOffsetRelative
                                                                               ? Vector3.Transform(
                                                                                   attach.PositionOffset, targGoalRot)
                                                                               : attach.PositionOffset);
                                                           movement.PositionDelta = goal - transform.Position;
                                                       }
                                                       if (attach.RotationAttach)
                                                       {
                                                           Quaternion goal = Quaternion.Inverse(transform.Rotation)*
                                                                             targGoalRot*attach.RotationOffset;
                                                           movement.RotationDelta = goal;
                                                       }
                                                       if (entity.Tag != null)
                                                           lock (updatedLock)
                                                           {
                                                               updated.Enqueue(entity);
                                                           }
                                                   });
                }

                if (lookAts.ContainsKey(tag))
                {
                    Bag<Entity> toUpdate = lookAts[tag];
                    Parallel.ForEach(toUpdate, entity =>
                                                   {
                                                       var look = entity.GetComponent<LookAtComponent>();
                                                       var transform = entity.GetComponent<TransformComponent>();
                                                       var movement = entity.GetComponent<MovementComponent>();
                                                       Vector3 goalPos = transform.Position + movement.PositionDelta;
                                                       float maxAngle =
                                                           HandyMath.ScaleValue(look.Speed, 1, MathHelper.Pi)/1000*
                                                           entityWorld.Delta;
                                                       Quaternion goalRotation =
                                                           QuaternionHelper.Face(transform.Rotation, goalPos,
                                                                                 targGoalPos, maxAngle);
                                                       movement.RotationDelta = Quaternion.Inverse(transform.Rotation)*
                                                                                goalRotation;
                                                       if (entity.Tag != null)
                                                           lock (updatedLock)
                                                           {
                                                               updated.Enqueue(entity);
                                                           }
                                                   });
                }
            }


        }
    }
}
