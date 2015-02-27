using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Hail.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 1000, IsResizable = true, ResizeSize = 1000, IsSupportMultiThread = true)]
    public class CollisionComponent : HailComponent
    {
        [ComponentProperty]
        public List<string> CollidesWith { get; set; }

        [ComponentProperty(true)]
        public bool Solid { get; set; }

        public List<Entity> Collisions { get; set; }
    }
}
