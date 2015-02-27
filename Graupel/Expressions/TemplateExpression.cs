﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class TemplateExpression : EntityBaseExpression
    {
        public TemplateExpression(string name, 
            List<string> templates, EntityBodyExpression body)
            : base(name, templates, body)
        {
        }

        public override void Print(StringBuilder sb, bool verbose)
        {
            sb.Append("template ");
            if (!String.IsNullOrEmpty(Name))
                sb.Append(Name).Append(" ");
            if (Templates.Count > 0)
            {
                sb.Append(": ");
                ExpressionHelper.PrintDelimited(sb, Templates, ", ");
                sb.Append(" ");
            }
            Body.Print(sb, verbose);
        }

        public override TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public override Type ValueType { get { return typeof(TemplateExpression); } }

        public override string ToString()
        {
            return "TemplateExpression: " + Name;
        }
    }
}
