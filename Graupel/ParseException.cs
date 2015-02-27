using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graupel
{
    public class ParseException : Exception
    {
        public Position Position { get; private set; }
        public ParseException(Position position, String message)
            : base(message)
        {
            Position = position;
        }
    }
}
