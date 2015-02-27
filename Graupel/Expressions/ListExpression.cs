using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class ListExpression : IExpression
    {
        public List<IExpression> Expressions { get; private set; }

        public ListExpression(List<IExpression> expressions)
        {
            foreach (IExpression expression in expressions)
            {
                if (ValueType == null)
                    ValueType = expression.ValueType;
                else
                {
                    if (ValueType != expression.ValueType)
                        throw new ParseException(
                            Position.None, "List values must be of the same type.");
                }
            }
            Expressions = expressions;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            if (verbose) sb.Append("(");
            ExpressionHelper.PrintDelimited(sb, Expressions, ", ", verbose);
            if (verbose) sb.Append(")");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get; private set; }

        public override string ToString()
        {
            return "ListExpression";
        }
    }
}
