using System;
using System.Text;
using Graupel.Lexer;

namespace Graupel.Expressions
{
    public class OperatorExpression : IExpression
    {
        public Token Operator { get; private set; }
        public IExpression LeftExpression { get; private set; }
        public IExpression RightExpression { get; private set; }

        public OperatorExpression(IExpression leftExpression, Token operatorToken, IExpression rightExpression)
        {
            Operator = operatorToken;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
            ValueType = ExpressionHelper.GetMultiExpressionValueType(
                new[] { LeftExpression.ValueType, RightExpression.ValueType });
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("(");
            LeftExpression.Print(sb, verbose);
            sb.Append(" ").Append(Operator.Text).Append(" ");
            RightExpression.Print(sb, verbose);
            sb.Append(")");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get; private set; }

        public override string ToString()
        {
            return "OperatorExpression: " + Operator.Type;
        }
    }
}
