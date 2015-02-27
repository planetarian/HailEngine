using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class EvalExpression : IExpression
    {
        public IExpression Value { get; private set; }

        public EvalExpression(IExpression expression)
        {
            Value = expression;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("eval { ");
            Value.Print(sb, verbose);
            sb.Append(" }");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context = null)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType
        {
            get { return Value.ValueType; }
        }
    }
}
