using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel
{
    public class GraupelMethod
    {
        public string MethodName { get; private set; }
        public Type ReturnType { get; private set; }
        public List<Type> ArgumentTypes { get; private set; }

        public GraupelMethod(string methodName, Type returnType, IEnumerable<Type> argumentTypes)
        {
            MethodName = methodName;
            ReturnType = returnType;
            ArgumentTypes = argumentTypes.ToList();
        }

    }
}
