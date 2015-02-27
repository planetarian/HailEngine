using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class GlobalExpression : IExpression
    {
        public Dictionary<string, TemplateExpression> Templates { get; private set; }

        public GlobalExpression()
            : this(new Dictionary<string, TemplateExpression>())
        {
        }

        public GlobalExpression(IEnumerable<TemplateExpression> templates)
            : this(templates.ToDictionary(t => t.Name))
        {
        }

        public GlobalExpression(Dictionary<string, TemplateExpression> templates)
        {
            Templates = templates;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            if (!verbose && Templates.Count == 0)
                return;

            sb.Append("global {");
            if (Templates.Count > 0) sb.Append("\n\t");
            ExpressionHelper.PrintDelimited(sb, Templates, "\n\t", verbose);
            sb.Append("\n}");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof(GlobalExpression); } }

        public override string ToString()
        {
            return "GlobalExpression";
        }

        public TemplateExpression GetTemplate(string templateName)
        {
            return Templates.ContainsKey(templateName)
                ? Templates[templateName] : null;
        }
    }
}
