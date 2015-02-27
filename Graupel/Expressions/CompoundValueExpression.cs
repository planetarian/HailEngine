using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class CompoundValueExpression : IExpression
    {
        public List<IExpression> Expressions { get; private set; }

        public CompoundValueExpression(List<IExpression> expressions)
        {
            Expressions = expressions;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            if (verbose) sb.Append("(");
            ExpressionHelper.PrintDelimited(sb, Expressions, " ", verbose);
            if (verbose) sb.Append(")");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType
        {
            get { return typeof (List<>); }
        }

        public override string ToString()
        {
            return "CompoundValueExpression";
        }
    }
}
