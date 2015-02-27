using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graupel.Lexer;

namespace Graupel.Expressions
{
    public class PrefixExpression : IExpression
    {
        public TokenType OperatorType { get; private set; }
        public IExpression RightExpression { get; private set; }

        public PrefixExpression(TokenType operatorType, IExpression rightExpression)
        {
            OperatorType = operatorType;
            RightExpression = rightExpression;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("(").Append(OperatorType);
            RightExpression.Print(sb, verbose);
            sb.Append(")");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType { get { return RightExpression.ValueType; } }

        public override string ToString()
        {
            return "PrefixExpression: " + OperatorType;
        }
    }
}
