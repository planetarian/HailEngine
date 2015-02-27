using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class ComponentExpression :IExpression
    {
        public string Name { get; private set; }
        public Dictionary<string, AssignExpression> Assignments { get; private set; }

        public ComponentExpression(string name)
            : this(name, new Dictionary<string, AssignExpression>())
        {
        }

        public ComponentExpression(string name, IEnumerable<AssignExpression> assignments)
            : this(name, assignments.ToDictionary(a => a.Name))
        {
        }

        public ComponentExpression(string name, Dictionary<string, AssignExpression> assignments)
        {
            Name = name;
            Assignments = assignments;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            bool shorthand = Assignments.Count == 1;
            bool newlines = Assignments.Count > 1;
            bool braces = verbose || Assignments.Count > 1;
            sb.Append(Name);
            if (braces) sb.Append(" {"); // componentName { var1:val1; var2:val2; }
            if (newlines) sb.Append("\n\t\t\t");
            if (shorthand) sb.Append(" ");
            ExpressionHelper.PrintDelimited(sb, Assignments, "\n\t\t\t", verbose);
            if (newlines) sb.Append("\n\t\t");
            if (braces) sb.Append("}"); 
            else if (Assignments.Count == 0) sb.Append(";"); // componentName;
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof(ComponentExpression); } }

        public override string ToString()
        {
            return "ComponentExpression: " + Name;
        }
    }
}
