using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class SceneExpression : IExpression
    {
        public string Name { get; private set; }
        public List<string> Includes { get; private set; }
        public Dictionary<string, TemplateExpression> Templates { get; private set; }
        public List<EntityExpression> Entities { get; private set; }
        public Dictionary<string, EntityExpression> NamedEntities { get; private set; }

        
        public SceneExpression(string name, List<string> includes,
            IEnumerable<TemplateExpression> templates, List<EntityExpression> entities)
            : this(name, includes, templates.ToDictionary(t=>t.Name), entities)
        {
            
        }

        public SceneExpression(string name, List<string> includes,
            Dictionary<string, TemplateExpression> templates, List<EntityExpression> entities)
        {
            Name = name;
            Includes = includes;
            Templates = templates;
            Entities = entities;
            NamedEntities = entities.Where(e => !String.IsNullOrEmpty(e.Name)).ToDictionary(e => e.Name);
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("scene ").Append(Name);

            if (Includes.Count > 0)
            {
                sb.Append(" : ");
                ExpressionHelper.PrintDelimited(sb, Includes, ", ");
            }

            if (verbose || Templates.Count + Entities.Count > 0)
            {
                sb.Append(" {");
                if (Templates.Count > 0) sb.Append("\n\t");
                ExpressionHelper.PrintDelimited(sb, Templates, "\n\t", verbose);
                if (Entities.Count > 0) sb.Append("\n\t");
                ExpressionHelper.PrintDelimited(sb, Entities, "\n\t", verbose);
                sb.Append("\n}");
            }
            else sb.Append(";");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof(SceneExpression); } }

        public override string ToString()
        {
            return "SceneExpression: " + Name;
        }
    }
}
