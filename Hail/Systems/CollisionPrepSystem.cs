using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Artemis.System;
using Artemis.Manager;
using Artemis.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Hail.Components;
using Hail.Helpers;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 40)]
    public class CollisionPrepSystem : EntitySystem
    {
        private const float scaleFactor = 10;

        public CollisionPrepSystem()
            : base(Aspect.All(typeof (TransformComponent), typeof (CollisionComponent)))
        {
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            Entity[] ents = entities.Values.ToArray();

            TransformComponent[] trans =
                ents.Select(e => e.GetComponent<TransformComponent>()).ToArray();

            CollisionComponent[] coll =
                ents.Select(e => e.GetComponent<CollisionComponent>()).ToArray();
            foreach (CollisionComponent collisionComponent in coll)
            {
                if (collisionComponent.Collisions == null)
                    collisionComponent.Collisions = new List<Entity>();
                else
                    collisionComponent.Collisions.Clear();
            }


            for (int i = 0; i < ents.Length; i++)
            {
                // Start on the current entity and move forward,
                // so we don't check against entities we've already checked.
                for (int x = i + 1; x < ents.Length; x++)
                {
                    // Simple radius check
                    float x1 = trans[i].Scale.X;
                    float y1 = trans[i].Scale.Y;
                    float z1 = trans[i].Scale.Z;
                    float x2 = trans[x].Scale.X;
                    float y2 = trans[x].Scale.Y;
                    float z2 = trans[x].Scale.Z;
                    var rad1 = (float) Math.Sqrt(x1*x1 + y1*y1 + z1*z1)*scaleFactor;
                    var rad2 = (float) Math.Sqrt(x2*x2 + y2*y2 + z2*z2)*scaleFactor;
                    float distance = Vector3.Distance(trans[i].Position, trans[x].Position);

                    if (distance > rad1 + rad2) continue;

                    // Bounding sphere collision found
                    coll[i].Collisions.Add(ents[x]);
                    coll[x].Collisions.Add(ents[i]);
                }
            }
        }

    }
}
