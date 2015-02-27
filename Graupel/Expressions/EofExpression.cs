using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class EofExpression : IExpression
    {
        public void Print(StringBuilder sb, bool verbose)
        {
        }

        public Type ValueType { get { return typeof(EofExpression); } }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }
    }
}
