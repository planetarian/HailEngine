using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.Components
{
    [ArtemisComponentPool(InitialSize = 1, IsResizable= true, ResizeSize = 5, IsSupportMultiThread = true)]
    public class MouseTargetComponent : HailComponent
    {
    }
}
