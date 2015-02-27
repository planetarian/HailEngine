using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Artemis.System;
using Artemis.Manager;
using Graupel.Expressions;
using Hail.Core;
using Hail.GraupelSemantics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Hail.Components;
using Hail.Helpers;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 0)]
    public class EvalSystem : ParallelEntityProcessingSystem
    {
        private static readonly GraupelExpressionVisitor visitor = GraupelExpressionVisitor.Visitor;

        public EvalSystem()
            : base(Aspect.All(typeof(EvalComponent)))
        {
        }

        public override void Process(Entity e)
        {
            var evalComp = e.GetComponent<EvalComponent>();
            foreach (KeyValuePair<string, Dictionary<string, IExpression>> compEntry in evalComp.Expressions)
            {
                HailComponent comp = e.GetComponent(compEntry.Key);
                foreach (KeyValuePair<string, IExpression> assignment in compEntry.Value)
                {
                    comp.SetValue(assignment.Key, assignment.Value, visitor);
                }
            }
        }

    }
}
