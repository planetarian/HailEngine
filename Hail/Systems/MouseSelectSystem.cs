using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Attributes;
using Artemis.System;
using Artemis.Utils;
using Artemis.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Hail.Components;
using Hail.Helpers;
using Microsoft.Xna.Framework.Input;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Asynchronous, GameLoopType = GameLoopType.Update, Layer = 10)]
    public class MouseSelectSystem : EntitySystem
    {
        private readonly GraphicsDevice device;

        private readonly List<ViewportComponent> viewports;
        private readonly List<Entity> transformable;
        private MouseState prevMouseState;


        public MouseSelectSystem()
            : base(Aspect.One(typeof (MouseTargetComponent), typeof (ViewportComponent), typeof(TransformComponent)))
        {
            device = BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
            viewports = new List<ViewportComponent>(5);
            transformable = new List<Entity>(1000);
            prevMouseState = Mouse.GetState();
            BlackBoard.SetEntry("HoveredEntity", "-1");
            BlackBoard.SetEntry("SelectedEntity", "-1");
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            viewports.Clear();
            transformable.Clear();
            TransformComponent mouseEntityTransform = null;

            foreach (var e in entities.Values)
            {
                if (mouseEntityTransform == null
                    && e.GetComponent<MouseTargetComponent>() != null)
                {
                    var trans = e.GetComponent<TransformComponent>();
                    if (trans == null)
                        throw new InvalidOperationException(
                            "Mouse target must have a transform component.");
                    mouseEntityTransform = trans;
                    var move = e.GetComponent<MovementComponent>();
                    if (move == null)
                        throw new InvalidOperationException(
                            "Mouse target must have a movement component.");
                    continue;
                }

                var view = e.GetComponent<ViewportComponent>();
                if (view != null)
                    viewports.Add(view);
                else
                    transformable.Add(e);
            }
            if (viewports.Count == 0) // mouseEntityTransform == null || 
                return;

            MouseState state = Mouse.GetState();

            int x = state.X;
            int y = state.Y;

            if (x < 0 || y < 0 ||
                x >= device.PresentationParameters.BackBufferWidth ||
                y >= device.PresentationParameters.BackBufferHeight)
                return;

            ViewportComponent viewport = null;
            foreach (ViewportComponent v in viewports)
            {
                if (!v.Viewport.Bounds.Contains(x, y))
                    continue;
                viewport = v;
                break;
            }
            if (viewport == null)
                return;

            Entity cameraEntity = entityWorld.TagManager.GetEntity(viewport.CameraTag);
            if (cameraEntity == null)
                return;
            var cameraComp = cameraEntity.GetComponent<CameraComponent>();
            var transComp = cameraEntity.GetComponent<TransformComponent>();
            if (cameraComp == null || transComp == null)
                return;

            // Unproject mouse position

            //Matrix worldMatrix = Matrix.CreateScale(transComp.Scale) * Matrix.CreateFromQuaternion(transComp.Rotation) * Matrix.CreateTranslation(transComp.Position);
            Vector3 near = viewport.Viewport.Unproject(
                new Vector3(x, y, 0), cameraComp.ProjectionMatrix, cameraComp.ViewMatrix, Matrix.Identity);
            Vector3 far = viewport.Viewport.Unproject(
                new Vector3(x, y, 1), cameraComp.ProjectionMatrix, cameraComp.ViewMatrix, Matrix.Identity);
            Vector3 dir = Vector3.Normalize(far - near);
            var ray = new Ray(near, dir);

            // Test for object intersection
            BoundingFrustum frust = cameraComp.Frustum;
            Entity closestEntity = null;
            float closestDistance = cameraComp.FarPlaneDistance;
            foreach (Entity e in transformable)
            {
                if (e == cameraEntity) continue;

                var trans = e.GetComponent<TransformComponent>(); // Can't be null
                var model = e.GetComponent<ModelComponent>(); // Might be null
                
                ContainmentType contains = frust.Contains(trans.BoundingBox);
                if (contains == ContainmentType.Disjoint)
                    continue;

                float len = closestDistance+1;
                if (false && model != null && model.Model != null)
                {
                    Vector3 finalPos =
                        trans.Position +
                        Vector3.Transform(trans.Scale*model.Offset, trans.Rotation);
                    Matrix modelPosMatrix = Matrix.CreateTranslation(finalPos);
                    Matrix world = trans.ScaleMatrix*trans.RotationMatrix*modelPosMatrix;
                    if (model.Model.CheckRayIntersection(ray, model.Transforms, world))
                    {
                        len = (near - finalPos).Length();
                    }
                }
                else
                {
                    if (ray.Intersects(trans.BoundingBox) != null)
                        len = (near - trans.Position).Length();
                    // TODO: no model
                }

                if (len < closestDistance && len > cameraComp.NearPlaneDistance)
                {
                    closestDistance = len;
                    closestEntity = e;
                }
            }

            if (closestEntity == null)
            {

                BlackBoard.SetEntry("HoveredEntity", "-1");
                /*
                // No object intersection, test for Z-axis crossing
                if (dir.Z == 0) return;
                Vector3 pos = near - dir*(near.Z/dir.Z);
                mouseEntityMovement.PositionDelta = pos - mouseEntityTransform.Position;

                if (state.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                {
                    Entity newEntity = entityWorld.CreateEntity();
                    var newSnap = newEntity.AddComponentFromPool<SnapComponent>();
                    newSnap.Increment = 20;
                    newEntity.AddComponentFromPool<MovementComponent>();
                    var newTrans = newEntity.AddComponentFromPool<TransformComponent>();
                    newTrans.Position = pos;
                    newEntity.AddComponentFromPool<CollisionComponent>();
                    newEntity.Refresh();
                }
                */
            }
            else
            {
                int id = closestEntity.Id;
                string name = closestEntity.Tag;
                BlackBoard.SetEntry("HoveredEntity", name ?? id.ToString());

                if (state.LeftButton == ButtonState.Pressed &&
                    prevMouseState.LeftButton == ButtonState.Released)
                    BlackBoard.SetEntry("SelectedEntity", name ?? id.ToString());
                //entityWorld.DeleteEntity(closestEntity);
            }

            prevMouseState = state;
        }



        public static Ray GetMouseRay(Vector2 mousePosition, Viewport viewport, CameraComponent camera)
        {
            var near = new Vector3(mousePosition, 0);
            var far = new Vector3(mousePosition, 1);

            near = viewport.Unproject(near, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            far = viewport.Unproject(far, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

            return new Ray(near, Vector3.Normalize(far - near));
        }

    }
}
