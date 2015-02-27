using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hail.Helpers
{
    public class ComponentPropertyAttribute : Attribute
    {
        /// <summary>
        /// The property's default value.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Whether the property's value should be settable in a Graupel file.
        /// </summary>
        public bool Settable { get; set; }

        public ComponentPropertyAttribute()
        {
            DefaultValue = null;
            Settable = true;
        }

        public ComponentPropertyAttribute(object defaultValue)
        {
            Settable = true;
            DefaultValue = defaultValue;
        }

        public ComponentPropertyAttribute(Type valueType, params object[] arguments)
        {
            DefaultValue = HandyMath.Translate(valueType, arguments);
            Settable = true;
        }
    }
}
