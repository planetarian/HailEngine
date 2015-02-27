using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class AssignExpression : IExpression
    {
        public string Name { get; private set; }
        public IExpression Value { get; private set; }

        public AssignExpression(string name, IExpression value)
        {
            Name = name;
            Value = value;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append(Name).Append(": ");
            Value.Print(sb, verbose);
            sb.Append(";");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return Value.ValueType; } }

        public override string ToString()
        {
            return "AssignExpression: " + Name;
        }
    }
}
