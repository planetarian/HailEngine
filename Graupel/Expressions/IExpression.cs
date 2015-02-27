using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public interface IExpression
    {
        void Print(StringBuilder sb, bool verbose);
        TR Accept<TR>(IExpressionVisitor<TR> visitor, string context = null);
        Type ValueType { get; }
    }

}
