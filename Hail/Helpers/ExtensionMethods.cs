using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Artemis.Interface;
using Artemis.Manager;
using Hail.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.Helpers
{
    public static class ExtensionMethods
    {
        public static bool CheckRayIntersection(
            this Model model, Ray ray, Matrix[] modelTransforms, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere sphere =
                    mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index]*world);
                if (ray.Intersects(sphere) != null) return true;
            }
            return false;
        }

        public static HailComponent AddComponentFromPool(this Entity e, EntityWorld world, Type componentType)
        {
            var component = (HailComponent)world.GetComponentFromPool(componentType);
            e.AddComponent(component);
            return component;
        }

        public static HailComponent GetComponent(this Entity e, string componentName)
        {
            Type type = HailComponent.ComponentTypes[componentName];
            return (HailComponent)e.GetComponent(ComponentTypeManager.GetTypeFor(type));
        }
    }
}
