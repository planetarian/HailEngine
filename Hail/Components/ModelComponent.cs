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
    public class ModelComponent : HailComponent
    {
        private string modelName;

        [ComponentProperty]
        public string ModelName
        {
            get { return modelName; }
            set
            {
                if (modelName == value) return;
                modelName = value;
                ModelChanged = true;
            }
        }

        [ComponentProperty(typeof (Vector3), 0)]
        public Vector3 Offset { get; set; }

        [ComponentProperty(Settable = false)]
        public Model Model { get; set; }

        [ComponentProperty(false, Settable = false)]
        public bool ModelChanged { get; set; }

        [ComponentProperty(Settable = false)]
        public Matrix[] Transforms { get; set; }
    }
}
