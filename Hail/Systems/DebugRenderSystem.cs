using System;
using System.Collections.Generic;
using System.Linq;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Hail.Components;
using Hail.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.Systems
{
    [ArtemisEntitySystem(ExecutionType = ExecutionType.Synchronous, GameLoopType = GameLoopType.Draw, Layer = 90)]
    public class DebugRenderSystem : EntitySystem
    {
        private readonly GraphicsDevice device;


        private readonly Dictionary<string, Matrix[]> modelTransforms;
        private readonly Dictionary<string, Model> models;

        private const float boxScale = 10;

        public DebugRenderSystem()
            : base(Aspect.One(typeof (CameraComponent), typeof (ViewportComponent), typeof (TransformComponent), typeof(ModelComponent)))
        {
            device = BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
            modelTransforms = new Dictionary<string, Matrix[]>();
            models = new Dictionary<string, Model>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            Entity[] ents = entities.Values.ToArray();

            var viewports = new List<ViewportComponent>(); // don't need entity data
            var cameras = new Dictionary<string, CameraComponent>();
            var transforms = new Dictionary<long, TransformComponent>();
            var modelComponents = new Dictionary<long, ModelComponent>();
            

            foreach (Entity entity in ents)
            {
                var vpComp = entity.GetComponent<ViewportComponent>();
                var camComp = entity.GetComponent<CameraComponent>();
                var posComp = entity.GetComponent<TransformComponent>();
                var modelComp = entity.GetComponent<ModelComponent>();

                if (vpComp != null)
                    viewports.Add(vpComp);
                else if (camComp != null && entity.Tag != null)
                    cameras.Add(entity.Tag, camComp);
                if (posComp != null)
                {
                    transforms.Add(entity.Id, posComp);
                    if (modelComp != null)
                    {
                        string name = modelComp.ModelName;
                        modelComponents.Add(entity.Id, modelComp);
                        if (!models.ContainsKey(name))
                        {
                            models.Add(name, modelComp.Model);
                            modelTransforms.Add(name, modelComp.Transforms);
                        }

                    }
                }
            }


            if (viewports.Count == 0 || cameras.Count == 0)
                return; // nothing to draw to, nothing to draw with, or nothing to draw.

            int w = device.PresentationParameters.Bounds.Width;
            int h = device.PresentationParameters.Bounds.Height;

            foreach (ViewportComponent viewport in viewports.OrderBy(t => t.Layer))
            {
                // TODO: Determine if we want to ignore viewports without matching cameras (for recreating cameras and auto-attaching)
                if (!cameras.ContainsKey(viewport.CameraTag))
                    continue;
                    //throw new InvalidOperationException("Viewport with no matching camera found.");
                
                // Get the viewport bounds
                // Stored as a RectangleF where each value is portion of screen
                // This allows easy window resizing
                // TODO: support 'sticky' sizing; e.g. auto-size height while width is always x pixels
                RectangleF b = viewport.Bounds;
                if (b.Width <= 0 || b.Height <= 0)
                    continue; // Viewport is collapsed
                // Scale to screen size
                var viewportBounds = (b*new Vector2(w, h)).ToRectangle();

                // Create viewport from bounds
                viewport.Viewport = new Viewport(viewportBounds);
                device.Viewport = viewport.Viewport;

                // Set camera matrices
                CameraComponent camera = cameras[viewport.CameraTag];
                camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    camera.FieldOfView, viewport.Viewport.AspectRatio, camera.NearPlaneDistance, camera.FarPlaneDistance);

                // Add a base grid just for movement reference
                Primitives.AddGrid(device, camera.ViewMatrix, camera.ProjectionMatrix,
                                   Matrix.CreateScale(10,1,1.1f)*Matrix.CreateTranslation(0,0,0));

                // Render visible objects
                foreach (KeyValuePair<long, TransformComponent> trans in transforms)
                {
                    long id = trans.Key;
                    TransformComponent transform = trans.Value;

                    // Determine if the object is visible
                    ContainmentType contains = camera.Frustum.Contains(transform.BoundingBox);
                    if (contains == ContainmentType.Disjoint)
                        continue;

                    // Set transform matrices
                    Matrix scaleMatrix = Matrix.CreateScale(transform.Scale);
                    Matrix rotMatrix = Matrix.CreateFromQuaternion(transform.Rotation);
                    Matrix posMatrix = Matrix.CreateTranslation(transform.Position);


                    // If the object has a model, draw that
                    if (modelComponents.ContainsKey(id))
                    {
                        string modelName = modelComponents[id].ModelName;

                        Matrix modelPosMatrix = Matrix.CreateTranslation(
                            transform.Position +
                            Vector3.Transform(
                                transform.Scale*modelComponents[id].Offset, transform.Rotation));

                        Model model = models[modelName];
                        Matrix[] modelXforms = modelTransforms[modelName];

                        foreach (ModelMesh mesh in model.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                effect.Projection = camera.ProjectionMatrix;
                                effect.View = camera.ViewMatrix;
                                effect.World =
                                    modelXforms[mesh.ParentBone.Index] *
                                    scaleMatrix * rotMatrix * modelPosMatrix;

                                effect.EnableDefaultLighting();

                                
                            }
                            mesh.Draw();
                        }
                    }
                    //else // Otherwise draw a box primitive
                    {
                        Matrix worldMatrix = scaleMatrix * rotMatrix * posMatrix;
                        Primitives.AddBoundingBox(device, camera.ViewMatrix, camera.ProjectionMatrix, worldMatrix);
                    }

                }

                // Draw whatever is left in the primitives vertex buffer
                Primitives.Flush(device, camera.ViewMatrix, camera.ProjectionMatrix);
            }
        }
    }
}