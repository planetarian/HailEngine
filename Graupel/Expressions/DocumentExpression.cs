using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Util;

namespace Graupel.Expressions
{
    public class DocumentExpression : IExpression
    {
        public GlobalExpression Global { get; private set; }
        //public List<SceneExpression> SceneExpressions { get; private set; }
        public Dictionary<string, SceneExpression> Scenes { get; private set; }

        public DocumentExpression(GlobalExpression global, IEnumerable<SceneExpression> scenes)
            : this(global, scenes.ToDictionary(s=>s.Name))
        {
        }

        public DocumentExpression(GlobalExpression global, Dictionary<string, SceneExpression> scenes)
        {
            if (global == null)
                global = new GlobalExpression();

            Global = global;
            Scenes = scenes;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            Global.Print(sb, verbose);
            sb.Append("\n");
            ExpressionHelper.PrintDelimited(sb, Scenes, "\n", verbose);
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof (DocumentExpression); } }

        public override string ToString()
        {
            return "DocumentExpression";
        }

        // End-use methods

        /// <summary>
        /// Retrieves a SceneExpression from the document.
        /// </summary>
        /// <param name="sceneName">Name of the scene to look for.</param>
        /// <returns>The requested SceneExpression, or null if not found.</returns>
        public SceneExpression GetScene(string sceneName)
        {
            return Scenes.ContainsKey(sceneName) ? Scenes[sceneName] : null;
        }

        /// <summary>
        /// Adds a SceneExpression to the document.
        /// </summary>
        /// <param name="scene">SceneExpression to add.</param>
        /// <exception cref="InvalidOperationException">
        /// Document already contains a scene with the given name.</exception>
        public void AddScene(SceneExpression scene)
        {
            if (GetScene(scene.Name) != null)
                throw new InvalidOperationException("Scene '"+scene.Name+"' already exists");
            Scenes.Add(scene.Name, scene);
        }

        /// <summary>
        /// Adds a TemplateExpression to the global templates.
        /// </summary>
        /// <param name="template">TemplateExpression to add.</param>
        public void AddTemplate(TemplateExpression template)
        {
            AddTemplate(template, null);
        }

        /// <summary>
        /// Adds a TemplateExpression to the specified scene.
        /// </summary>
        /// <param name="template">TemplateExpression to add.</param>
        /// <param name="sceneName">Name of scene to add the template to.</param>
        /// <remarks>Pass null as the sceneName to add to globals instead.</remarks>
        /// <exception cref="InvalidOperationException">
        /// Document already contains a template with that name, either in the globals
        /// or within the specified scene's inheritance.</exception>
        public void AddTemplate(TemplateExpression template, string sceneName)
        {
            // Look for existing template with same name.
            // Do not recurse. Inheriting scene could contain a duplicate.
            string foundScene;
            if (GetTemplate(template.Name, sceneName, false, out foundScene) != null)
                throw new InvalidOperationException(
                    "Template named '" + template.Name + "' already exists in " +
                    (foundScene == null ? "global templates" : "scene '" + foundScene + "'"));

            // If a scene was specified...
            if (String.IsNullOrEmpty(sceneName))
                Global.Templates.Add(template.Name, template);
            else // Check global templates.
            {
                if (Scenes.ContainsKey(sceneName))
                    throw new InvalidOperationException(
                        "Attemped to add template '"+template.Name+"' to non-existent scene '"+sceneName+"'");
                
                Scenes[sceneName].Templates.Add(template.Name, template);
            }
        }

        /// <summary>
        /// Retrieves a template from the global templates list.
        /// </summary>
        /// <param name="templateName">Name of the template to retrieve.</param>
        /// <returns>The named template, or null if not found.</returns>
        public TemplateExpression GetTemplate(string templateName)
        {
            return Global.GetTemplate(templateName);
        }

        /// <summary>
        /// Retrieves a template from the specified scene,
        /// checking globals and recursing through inherited scenes.
        /// </summary>
        /// <param name="templateName">Name of the template to retrieve.</param>
        /// <param name="sceneName">Scene to retrieve the template from.</param>
        /// <returns>The named template, or null if not found.</returns>
        /// <remarks>This method will recurse through inherited scenes.</remarks>
        public TemplateExpression GetTemplate(string templateName, string sceneName)
        {
            return GetTemplate(templateName, sceneName, true);
        }

        /// <summary>
        /// Retrieves a template from the specified scene,
        /// checking globals and recursing through inherited scenes.
        /// </summary>
        /// <param name="templateName">Name of the template to retrieve.</param>
        /// <param name="sceneName">Scene to retrieve the template from.</param>
        /// <param name="recurse">Whether to recurse through inherited scenes.</param>
        /// <returns>The named template, or null if not found.</returns>
        public TemplateExpression GetTemplate(
            string templateName, string sceneName, bool recurse)
        {
            string containingScene;
            return GetTemplate(templateName, sceneName, recurse, out containingScene);
        }

        /// <summary>
        /// Retrieves a template from the specified scene,
        /// checking globals and recursing through inherited scenes.
        /// </summary>
        /// <param name="templateName">Name of the template to retrieve.</param>
        /// <param name="sceneName">Scene to retrieve the template from.</param>
        /// <param name="recurse">Whether to recurse through inherited scenes.</param>
        /// <param name="containingScene">Scene the template was found in,
        /// or null if it was found in the global templates.</param>
        /// <returns>The named template, or null if not found.</returns>
        public TemplateExpression GetTemplate(
            string templateName, string sceneName, bool recurse, out string containingScene)
        {
            containingScene = null;
            return GetTemplate(templateName)
                ?? GetTemplateRecurse(templateName, sceneName, recurse, out containingScene);
        }

        /// <summary>
        /// Retrieves a template from the specified scene,
        /// checking globals and recursing through inherited scenes.
        /// </summary>
        /// <param name="templateName">Name of the template to retrieve.</param>
        /// <param name="sceneName">Scene to retrieve the template from.</param>
        /// <param name="recurse">Whether to recurse through inherited scenes.</param>
        /// <param name="containingScene">Scene the template was found in,
        /// or null if it was found in the global templates.</param>
        /// <returns>The named template, or null if not found.</returns>
        private TemplateExpression GetTemplateRecurse(
            string templateName, string sceneName, bool recurse, out string containingScene)
        {
            containingScene = null;

            // Check the current scene
            SceneExpression scene = Scenes[sceneName];
            if (scene.Templates.ContainsKey(templateName))
            {
                containingScene = scene.Name;
                return scene.Templates[templateName];
            }

            if (!recurse) return null;

            // Check the current scene's inherited scenes
            foreach (string include in scene.Includes)
            {
                TemplateExpression template = GetTemplateRecurse(
                    templateName, include, true, out containingScene);
                if (template != null)
                    return template;
            }

            // Not found
            return null;
        }

    }
}
