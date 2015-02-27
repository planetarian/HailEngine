using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class GroupExpression : IExpression
    {
        public string Name { get; private set; }

        public GroupExpression(string name)
        {
            Name = name;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("group: \"");
            sb.Append(Name);
            sb.Append("\";");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof(string); } }

        public override string ToString()
        {
            return "GroupExpression: '" + Name + "'";
        }
    }
}
