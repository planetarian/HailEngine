using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class StringExpression : IExpression
    {
        public string Value { get; private set; }

        public StringExpression(string value)
        {
            Value = value;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append('"').Append(Value).Append('"');
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return typeof(string); } }

        public override string ToString()
        {
            return "StringExpression: \"" + Value + "\"";
        }
    }
}
