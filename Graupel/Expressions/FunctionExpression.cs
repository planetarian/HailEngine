using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel.Expressions
{
    public class FunctionExpression : IExpression
    {
        public IdentifierExpression Function { get; private set; }
        public List<IExpression> Args { get; private set; }

        public static List<GraupelMethod> RegisteredMethods
            = new List<GraupelMethod>();

        public FunctionExpression(IdentifierExpression function, List<IExpression> args)
        {
            Function = function;
            Args = args;
        }

        public void Print(StringBuilder sb, bool verbose)
        {
            Function.Print(sb, verbose);
            sb.Append("(");
            ExpressionHelper.PrintDelimited(sb, Args, ", ", verbose);
            sb.Append(")");
        }

        public TR Accept<TR>(IExpressionVisitor<TR> visitor, string context)
        {
            return visitor.Visit(this, context);
        }

        public Type ValueType
        {
            get
            {
                List<Type> types = Args.Select(expression => expression.ValueType).ToList();

                foreach (GraupelMethod method in RegisteredMethods.Where(
                    m=>m.MethodName==Function.Name&&m.ArgumentTypes.Count==types.Count))
                {
                    List<Type> methodTypes = method.ArgumentTypes;
                    bool match = true;
                    for (int i = 0; i < Args.Count; i++)
                    {
                        if (!(types[i] == typeof(int) && methodTypes[i] == typeof(float))
                            && types[i] != methodTypes[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        return method.ReturnType;
                }
                throw new InvalidOperationException(
                    "No methods have been registered that match the name/types given.");
            }
        }

        public override string ToString()
        {
            return "FunctionExpression: " + Function.Name;
        }
    }
}
