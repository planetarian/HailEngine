using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class FloatExpression : IExpression
    {
        public float Value { get; private set; }

        public FloatExpression(float value)
        {
            Value = value;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append(Value);
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof(float); } }

        public override string ToString()
        {
            return "FloatExpression: " + Value;
        }
    }
}
