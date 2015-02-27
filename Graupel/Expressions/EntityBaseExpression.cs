using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public abstract class EntityBaseExpression : IExpression
    {
        public string Name { get; private set; }
        public List<string> Templates { get; private set; }
        public EntityBodyExpression Body { get; private set; }

        protected EntityBaseExpression(string name, 
            List<string> templates, EntityBodyExpression body)
        {
            Name = name;
            Templates = templates;
            Body = body;
        }

        public abstract TR Accept<TR>(IExpressionVisitor<TR> visitor, string context);

        public abstract Type ValueType { get; }

        public abstract void Print(StringBuilder sb, bool verbose);
    }
}
