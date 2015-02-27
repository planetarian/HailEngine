using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Lexer;

namespace Graupel.Expressions
{
    public class PostfixExpression : IExpression
    {
        public TokenType OperatorType { get; private set; }
        public IExpression LeftExpression { get; private set; }

        public PostfixExpression(IExpression leftExpression, TokenType operatorType)
        {
            OperatorType = operatorType;
            LeftExpression = leftExpression;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("(");
            LeftExpression.Print(sb, verbose);
            sb.Append(OperatorType).Append(")");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return LeftExpression.ValueType; } }

        public override string ToString()
        {
            return "PostfixExpression: " + OperatorType;
        }
    }
}
