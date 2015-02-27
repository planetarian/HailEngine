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

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 1)]
    public class ModelLoadSystem : EntityProcessingSystem
    {
        private readonly ContentManager content;

        public ModelLoadSystem()
            : base(Aspect.All(typeof(ModelComponent)))
        {
            content = BlackBoard.GetEntry<ContentManager>("ContentManager");
        }

        public override void Process(Entity e)
        {
            var model = e.GetComponent<ModelComponent>();
            if (!model.ModelChanged) return;

            model.Model = content.Load<Model>(@"Models\" + model.ModelName);
            model.Transforms = new Matrix[model.Model.Bones.Count];
            model.Model.CopyAbsoluteBoneTransformsTo(model.Transforms);
            model.ModelChanged = false;

        }

    }
}
