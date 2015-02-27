using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class EntityBodyExpression : IExpression
    {
        public GroupExpression Group { get; private set; }
        public Dictionary<string, ComponentExpression> Components { get; private set; }

        public EntityBodyExpression()
            : this(null, new Dictionary<string, ComponentExpression>())
        {
        }

        public EntityBodyExpression(GroupExpression group)
            : this(group, new Dictionary<string, ComponentExpression>())
        {
        }

        public EntityBodyExpression(GroupExpression group, IEnumerable<ComponentExpression> components)
            : this(group, components.ToDictionary(c => c.Name))
        {
        }

        public EntityBodyExpression(GroupExpression group, Dictionary<string, ComponentExpression> components)
        {
            Group = group;
            Components = components;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            int lines = (Group != null ? 1 : 0) + Components.Count;
            bool newlines = lines > 0;

            sb.Append("{");
            if (Group != null)
            {
                if (newlines) sb.Append("\n\t\t");
                Group.Print(sb, verbose);
            }
            if (newlines) sb.Append("\n\t\t");
            ExpressionHelper.PrintDelimited(sb, Components, "\n\t\t", verbose);
            if (newlines) sb.Append("\n\t");
            sb.Append("}");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType
        {
            get { return typeof (EntityBodyExpression); }
        }

        public override string ToString()
        {
            return "EntityBodyExpression";
        }
    }
}
