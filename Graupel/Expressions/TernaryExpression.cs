using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class TernaryExpression : IExpression
    {
        public IExpression Condition { get; private set; }
        public IExpression TrueExpression { get; private set; }
        public IExpression FalseExpression { get; private set; }

        public TernaryExpression(IExpression condition, IExpression trueExpression, IExpression falseExpression)
        {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
            if (Condition.ValueType != typeof(bool))
                throw new InvalidOperationException(
                    "Ternary operation must have boolean condition expression.");
            ValueType = ExpressionHelper.GetMultiExpressionValueType(
                new[] { TrueExpression.ValueType, FalseExpression.ValueType });
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("(");
            Condition.Print(sb, verbose);
            sb.Append(" ? ");
            TrueExpression.Print(sb, verbose);
            sb.Append(" : ");
            FalseExpression.Print(sb, verbose);
            sb.Append(")");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get; private set; }

        public override string ToString()
        {
            return "TernaryExpression";
        }
    }
}
