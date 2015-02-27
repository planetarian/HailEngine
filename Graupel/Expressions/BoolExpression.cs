using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class BoolExpression : IExpression
    {
        public bool Value { get; private set; }

        public BoolExpression(bool value)
        {
            Value = value;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append(Value ? "true" : "false");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof (bool); } }

        public override string ToString()
        {
            return "BoolExpression: " + (Value?"true":"false");
        }
    }
}
