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
using Microsoft.Xna.Framework.Input;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 10)]
    public class MouseZoomSystem : ParallelEntityProcessingSystem
    {
        private MouseState prevState;

        public MouseZoomSystem()
            : base(Aspect.All(typeof(ZoomComponent), typeof(AttachmentComponent)))
        {
        }

        public override void Process(Entity e)
        {
            MouseState state = Mouse.GetState();
            var zoom = e.GetComponent<ZoomComponent>();
            var attach = e.GetComponent<AttachmentComponent>();

            if (zoom.TargetPos == Vector3.Zero)
                zoom.TargetPos = attach.PositionOffset;

            if (state.ScrollWheelValue > prevState.ScrollWheelValue)
            {
                if (zoom.TargetPos.Length() > zoom.MinZoomLevel)
                zoom.TargetPos *= (1 - zoom.Amount);
            }

            if (state.ScrollWheelValue < prevState.ScrollWheelValue)
            {
                if (zoom.TargetPos.Length() < zoom.MaxZoomLevel)
                zoom.TargetPos *= (1 + zoom.Amount);
            }


            attach.PositionOffset = Vector3.Lerp(attach.PositionOffset, zoom.TargetPos, zoom.Smoothing);

            prevState = state;
        }

    }
}
