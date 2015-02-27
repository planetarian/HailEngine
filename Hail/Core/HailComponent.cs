using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Artemis;
using Graupel.Expressions;
using Hail.Components;
using Hail.GraupelSemantics;
using Hail.Helpers;

namespace Hail.Core
{
    public abstract class HailComponent : ComponentPoolable
    {
        public static readonly Dictionary<string, Type> ComponentTypes
            = new Dictionary<string, Type>();

        // Dictionary to hold type initialization methods' cache 
        private static readonly Dictionary<Type, Action<object>> _typesInitializers =
            new Dictionary<Type, Action<object>>();

        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _properties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public override void Initialize()
        {
            base.Initialize();
            ApplyDefaultValues();
        }

        public void SetValue(
            string name, IExpression expression,
            IExpressionVisitor<object> visitor)
        {
            name = name.ToLower();
            Type compType = GetType();
            if (!_properties.ContainsKey(compType))
                throw new InvalidOperationException(
                    "Component type '" + compType.Name + "' has no registered properties.");
            if (!_properties[compType].ContainsKey(name))
                throw new InvalidOperationException(
                    "Property '" + name + "' is not registered in component type '" + compType.Name + "'");

            PropertyInfo prop = _properties[compType][name];
            Type type = prop.PropertyType;

            object translated = expression.Translate(visitor, type);

            prop
                .SetValue(this, translated
#if !WINRT
                          , null
#endif
                );
        }



        /// <summary>
        /// Implements precompiled setters with embedded constant values from ComponentPropertyAttributes
        /// </summary>
        public void ApplyDefaultValues()
        {
            Action<object> setter;
            Type type = GetType();

            // Attempt to get it from cache
            if (!_typesInitializers.TryGetValue(type, out setter))
            {
                // If no initializers are added do nothing
                setter = o => { };

                // Iterate through each property
                ParameterExpression objectTypeParam = Expression.Parameter(typeof (object), "this");
                foreach (PropertyInfo prop in type
#if WINRT
                    .GetRuntimeProperties())
#else
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance))
#endif
                {
                    Expression dva;

                    // Skip read only properties
                    if (!prop.CanWrite)
                        continue;

                    // There are no more then one attribute of this type
#if WINRT
                    ComponentPropertyAttribute[] attr
                        = prop.GetCustomAttributes(typeof (ComponentPropertyAttribute), false)
                            .Select(a => (ComponentPropertyAttribute) a).ToArray();
#else
                    object[] attrObj = prop.GetCustomAttributes(typeof(ComponentPropertyAttribute), false);
                    var attr = attrObj as ComponentPropertyAttribute[];
#endif

                    // Skip properties with no ComponentPropertyAttribute
                    if ((null == attr) || (attr.Length == 0) || (attr[0] == null))
                        continue;

                    // Make sure the type is registered in the property names list
                    if (!_properties.ContainsKey(type))
                        _properties.Add(type, new Dictionary<string, PropertyInfo>());

                    // Verify the property isn't already registered
                    string propName = prop.Name.ToLower();
                    if (_properties[type].ContainsKey(propName))
                        throw new InvalidOperationException(
                            "Duplicate Property '" + prop.Name +
                            "' found. Component property names in Graupel are case-insensitive.");
                    _properties[type].Add(propName, prop);

                    // Build the Lambda expression
#if DEBUG
                    // Make sure types do match
                    try
                    {
#endif
                        dva = Expression.Convert(
                            Expression.Constant(
                                attr[0].DefaultValue ??
                                (prop.PropertyType
#if WINRT
                                     .GetTypeInfo()
#endif
                                     .IsValueType
                                     ? Activator.CreateInstance(prop.PropertyType)
                                     : null)
                                )
                            , prop.PropertyType);
#if DEBUG
                    }
                    catch (InvalidOperationException e)
                    {
                        string error =
                            String.Format(
                                "Type of ComponentPropertyAttribute({3}{0}{3}) does not match type of property {1}.{2}",
                                attr[0].DefaultValue, type.Name, prop.Name,
                                ((attr[0].DefaultValue is string) ? "\"" : ""));

                        throw (new InvalidOperationException(error, e));
                    }
#endif

                    // ReSharper disable PossiblyMistakenUseOfParamsMethod
                    Expression setExpression = Expression.Call(
                        Expression.TypeAs(objectTypeParam, type),
                        prop
#if WINRT
                            .SetMethod
#else
                            .GetSetMethod()
#endif
                        , dva);
                    // ReSharper restore PossiblyMistakenUseOfParamsMethod

                    Expression<Action<object>> setLambda = Expression.Lambda<Action<object>>(setExpression,
                                                                                             objectTypeParam);

                    // Add this action to multicast delegate
                    setter += setLambda.Compile();
                }

                // Save in the type cache
                _typesInitializers.Add(type, setter);
            }

            // Initialize member properties
            setter(this);
        }
    }
}
