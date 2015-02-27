using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class IdentifierExpression : IExpression
    {
        public string Name { get; private set; }

        public static Dictionary<string, Type> RegisteredIdentifiers
            = new Dictionary<string, Type>();

        public IdentifierExpression(string name)
        {
            Name = name;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            sb.Append(Name);
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType
        {
            get
            {
                return RegisteredIdentifiers.ContainsKey(Name)
                    ? RegisteredIdentifiers[Name]
                    : typeof(IdentifierExpression);
            }
        }

        public override string ToString()
        {
            return "IdentifierExpression: " + Name;
        }
    }
}
