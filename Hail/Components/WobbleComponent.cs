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
    public class WobbleComponent : HailComponent
    {
        [ComponentProperty(1)]
        public float Period { get; set; }

        [ComponentProperty(1)]
        public float Amplitude { get; set; }

        [ComponentProperty(0)]
        public float Time { get; set; }
    }
}
