using System;
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
    [ArtemisComponentPool(InitialSize = 3, IsResizable = true, ResizeSize = 10, IsSupportMultiThread = true)]
    public class ViewportComponent : HailComponent
    {
        [ComponentProperty]
        public string CameraTag { get; set; }

        [ComponentProperty(-1)]
        public int Layer { get; set; }

        [ComponentProperty(Settable = false)]
        public Viewport Viewport { get; set; }

        [ComponentProperty(typeof (RectangleF), 0, 0, 1, 1)]
        public RectangleF Bounds { get; set; }
    }
}