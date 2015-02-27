using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Artemis;
using Artemis.Manager;
using Graupel;
using Graupel.Expressions;
using Graupel.Lexer;
using Hail.Components;
using Hail.Core;
using Hail.Helpers;
#if WINRT
using Windows.Storage;
#endif

namespace Hail.GraupelSemantics
{
    public class GraupelLoader
    {
        /// <summary>
        /// The last document loaded.
        /// </summary>
        public DocumentExpression Document { get; private set; }

        private static readonly IExpressionVisitor<object> visitor
            = GraupelExpressionVisitor.Visitor;

        /// <summary>
        /// Initializes all functions and special identifiers available for use.
        /// </summary>
        static GraupelLoader()
        {
            FunctionExpression.RegisteredMethods.Add(new GraupelMethod("rand", typeof(int), new[] { typeof(int) }));
            FunctionExpression.RegisteredMethods.Add(new GraupelMethod("rand", typeof(int), new[] { typeof(int), typeof(int) }));
            FunctionExpression.RegisteredMethods.Add(new GraupelMethod("rand", typeof(float), new[] { typeof(float), typeof(float) }));
            FunctionExpression.RegisteredMethods.Add(new GraupelMethod("rand", typeof(float), new Type[0]));
            
            IdentifierExpression.RegisteredIdentifiers.Add("screenwidth", typeof(int));
            IdentifierExpression.RegisteredIdentifiers.Add("screenheight", typeof(int));
            IdentifierExpression.RegisteredIdentifiers.Add("isphone", typeof(bool));

            HailComponent.ComponentTypes.Add("attachment", typeof(AttachmentComponent));
            HailComponent.ComponentTypes.Add("camera", typeof(CameraComponent));
            //HailComponent.ComponentTypes.Add("collision", typeof(CollisionComponent));
            HailComponent.ComponentTypes.Add("eval", typeof(EvalComponent));
            HailComponent.ComponentTypes.Add("input", typeof(InputComponent));
            HailComponent.ComponentTypes.Add("lookat", typeof(LookAtComponent));
            HailComponent.ComponentTypes.Add("model", typeof(ModelComponent));
            HailComponent.ComponentTypes.Add("mousetarget", typeof(MouseTargetComponent));
            HailComponent.ComponentTypes.Add("movement", typeof(MovementComponent));
            HailComponent.ComponentTypes.Add("transform", typeof(TransformComponent));
            HailComponent.ComponentTypes.Add("viewport", typeof(ViewportComponent));
            HailComponent.ComponentTypes.Add("waypointfollower", typeof(WaypointFollowerComponent));
            HailComponent.ComponentTypes.Add("wobble", typeof(WobbleComponent));
            HailComponent.ComponentTypes.Add("zoom", typeof(ZoomComponent));
            HailComponent.ComponentTypes.Add("snap", typeof(SnapComponent));
            HailComponent.ComponentTypes.Add("sonicphysics", typeof(SonicPhysicsComponent));
        }

        /// <summary>
        /// Loads and verifies the contents of a graupel document.
        /// </summary>
        /// <param name="path">Path to the file to load.</param>
        /// <returns>The loaded GraupelDocument object.</returns>
        public DocumentExpression Load(string path)
        {
            Document = null;
            Expect.NotEmpty(path);
#if WINDOWS_PHONE
            string entitydef = File.OpenText(path).ReadToEnd();
#else
            string entitydef = File.ReadAllText(path);
#endif
            var reader = new StringSourceReader("Source", entitydef);
            var lexer = new Lexer(reader);
            var morpher = new Morpher(lexer); // clear whitespace/comments
            var parser = new GraupelParser(morpher);
            DocumentExpression document = parser.ParseDocument();
            //GraupelDocument document = GraupelObjectVisitor.VisitDocument(expression);
            VerifyDocument(document);
            Document = document;
            return document;
        }

        /// <summary>
        /// Spawns all entities from the last loaded document and the given scene.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="sceneName"></param>
        public void SpawnEntities(EntityWorld world, string sceneName)
        {
            Expect.NotNull(Document);
            Expect.NotEmpty(sceneName);
            if (!Document.Scenes.ContainsKey(sceneName))
                throw new InvalidOperationException(
                    "Document does not contain scene " + sceneName);

            SceneExpression scene = Document.Scenes[sceneName];

            foreach (string extended in scene.Includes)
            {
                SpawnEntities(world, extended);
            }

            foreach (EntityExpression entity in scene.Entities)
            {
                SpawnEntity(world, sceneName, entity);
            }
        }

        private void SpawnEntity(EntityWorld world, string sceneName,
            EntityBaseExpression entityBase)
        {
            Entity entity = world.CreateEntity();
            BuildEntity(world, entity, sceneName, entityBase);

            if (entityBase is EntityExpression && !String.IsNullOrEmpty(entityBase.Name))
                entity.Tag = entityBase.Name;

            entity.Refresh();
        }

        private void BuildEntity(EntityWorld world, Entity entity, string sceneName,
            EntityBaseExpression entityBase)
        {
            foreach (string extended in entityBase.Templates)
            {
                TemplateExpression template = Document.GetTemplate(extended, sceneName);
                BuildEntity(world, entity, sceneName, template);
            }
            foreach (ComponentExpression graupelComponent in entityBase.Body.Components.Values)
            {
                AddComponent(world, entity, graupelComponent);
            }

            if (entityBase.Body.Group != null && !String.IsNullOrEmpty(entityBase.Body.Group.Name))
                entity.Group = entityBase.Body.Group.Name;
        }

        private void AddComponent(EntityWorld world, Entity entity, ComponentExpression graupelComponent)
        {
            if (!HailComponent.ComponentTypes.ContainsKey(graupelComponent.Name))
                throw new InvalidOperationException(
                    "Invalid component type '" + graupelComponent.Name + "'");

            // See if entity already has this component
            Type type = HailComponent.ComponentTypes[graupelComponent.Name];
            ComponentType ctype = ComponentTypeManager.GetTypeFor(type);
            var component = (HailComponent) entity.GetComponent(ctype)
                            ?? entity.AddComponentFromPool(world, type);

            // Set component properties
            foreach (AssignExpression assignment in graupelComponent.Assignments.Values)
            {
                var eval = assignment.Value as EvalExpression;
                if (eval != null)
                {
                    var evalComp = (EvalComponent) entity.GetComponent("eval")
                                   ?? entity.AddComponentFromPool<EvalComponent>();
                    string name = graupelComponent.Name;
                    if (!evalComp.Expressions.ContainsKey(name))
                        evalComp.Expressions.Add(name, new Dictionary<string, IExpression>());
                    var expressions = evalComp.Expressions[name];
                    if (expressions.ContainsKey(assignment.Name))
                        throw new InvalidOperationException(
                            "Attempted to set a duplicate eval for assignment '"
                            + assignment.Name + "' in component '" + name + "'");
                    expressions.Add(assignment.Name, eval.Value);
                }

                component.SetValue(assignment.Name, assignment.Value, visitor);
            }
        }

        #region verify
        /// <summary>
        /// Checks a document to ensure there are no recursive scene/template dependencies
        /// and that all referenced scenes/templates are present and non-duplicate.
        /// </summary>
        /// <param name="document">GraupelDocument to verify.</param>
        private static void VerifyDocument(DocumentExpression document)
        {
            // Make sure there are no templates with recursive dependencies
            foreach (TemplateExpression template in document.Global.Templates.Values)
            {
                if (IsTemplateRecursive(document, template.Name))
                    throw new InvalidOperationException(
                        "Template '" + template.Name + "' has recursive dependencies.");
            }

            foreach (SceneExpression scene in document.Scenes.Values)
            {
                // Make sure scene is not recursive and all extended scenes exist
                if (IsSceneRecursive(document, scene.Name))
                    throw new InvalidOperationException(
                        "Scene '" + scene.Name + "' has recursive dependencies.");

                foreach (TemplateExpression template in scene.Templates.Values)
                {
                    if (IsTemplateDuplicate(document, template.Name, scene.Name))
                        throw new InvalidOperationException(
                            "Duplicate template name " + template.Name);
                }
                foreach (TemplateExpression template in scene.Templates.Values)
                {
                    // Template is not recursive
                    // and extended templates exist
                    if (IsTemplateRecursive(document, template.Name, scene.Name))
                        throw new InvalidOperationException(
                            "Template '" + template.Name + "' has recursive dependencies.");
                }

                // Entity references valid templates
                foreach (EntityExpression entity in scene.Entities)
                {
                    foreach (string extends in entity.Templates)
                    {
                        if (document.GetTemplate(extends, scene.Name) == null)
                            throw new InvalidOperationException(
                                "Entity '" + entity.Name + "' extends template '"
                                + extends + "' which has not been defined");
                    }
                }

                // duplicate entities
                foreach (EntityExpression entity in scene.NamedEntities.Values)
                {
                    if (FindEntityDuplicates(document, entity.Name, scene.Name) > 1)
                        throw new InvalidOperationException(
                            "Duplicate entity name " + entity.Name);

                }
            }

        }

        private static bool IsSceneRecursive(DocumentExpression document,
            string currentScene, List<string> traversed = null)
        {
            if (traversed == null)
                traversed = new List<string>();
            traversed.Add(currentScene);
            
            SceneExpression scene = document.Scenes[currentScene];
            foreach (string extends in scene.Includes)
            {
                if (!document.Scenes.ContainsKey(extends))
                    throw new InvalidOperationException(
                        "Scene '" + scene.Name + "' extends scene '"
                        + extends + "' which has not been defined.");

                if (traversed.Contains(extends)
                    || IsSceneRecursive(document, extends, traversed))
                    return true;
            }
            return false;
        }
        
                 // REFACTOR: Document design now ensures template duplicates are not possible
        private static bool IsTemplateDuplicate(DocumentExpression document, string templateName,
            string sceneName)
        {
            int found = document.Global.Templates.ContainsKey(templateName) ? 1 : 0;
            found += FindTemplateDuplicates(document, templateName, sceneName);
            return found > 1;
        }

        private static int FindTemplateDuplicates(DocumentExpression document, string templateName,
            string sceneName)
        {
            SceneExpression scene = document.Scenes[sceneName];
            int found = scene.Templates.ContainsKey(templateName) ? 1 : 0;
            found += scene.Includes.Sum(i => FindTemplateDuplicates(document, templateName, i));
            return found;
        }

        private static int FindEntityDuplicates(DocumentExpression document, string entityName,
            string sceneName)
        {
            SceneExpression scene = document.Scenes[sceneName];
            int found = scene.NamedEntities.ContainsKey(entityName) ? 1 : 0;
            found += scene.Includes.Sum(i => FindEntityDuplicates(document, entityName, i));
            return found;
        }


        private static bool IsTemplateRecursive(DocumentExpression document, string templateName,
            string sceneName = null, List<string> traversed = null)
        {
            if (traversed == null)
                traversed = new List<string>();
            traversed.Add(templateName);

            TemplateExpression template = document.GetTemplate(templateName, sceneName);
            foreach (string extends in template.Templates)
            {
                /*if (!sceneName.Templates.ContainsKey(extends)
                    && !document.GlobalTemplates.ContainsKey(extends))
                    throw new InvalidOperationException(
                        "Template '" + template.Name + "' extends template '"
                        + extends + "' which has not been defined.");*/
                if (traversed.Contains(extends)
                    || IsTemplateRecursive(document, extends, sceneName, traversed))
                    return true;
            }
            return false;
        }
        #endregion

        /*
        public static GraupelComponent MergeComponents(GraupelComponent primary, GraupelComponent secondary)
        {
            Expect.NotNull(primary, secondary);
            Expect.NotEmpty(primary.Name);
            Expect.IsEqual(primary.Name, secondary.Name);

            Dictionary<string, IExpression> assignments = primary.Assignments.ToDictionary(
                assignment => assignment.Key, assignment => assignment.Value);

            foreach (var assignment in secondary.Assignments)
            {
                if (!assignments.ContainsKey(assignment.Key))
                    assignments.Add(assignment.Key, assignment.Value);
            }

            var newComponent = new GraupelComponent(primary.Name, assignments);
            return newComponent;
        }*/
    }
}
