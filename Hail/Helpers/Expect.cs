using System;

namespace Hail.Helpers
{
    public static class Expect
    {
        public static void IsEqual<T>(T var1, T var2)
        {
            NotNull(var1, var2);
            if (!var1.Equals(var2))
                throw new InvalidOperationException("Values must match.");
        }

        public static void NotNull(params object[] args)
        {
            foreach (var arg in args)
            {
                if (arg == null)
                    throw new InvalidOperationException("Value cannot be null.");
            }
        }

        public static T NotNull<T>(T arg) where T : class
        {
            if (arg == null)
                throw new InvalidOperationException("Value cannot be null.");
            return arg;
        }

        public static void IsNull<T>(T arg) where T : class
        {
            if (arg != null)
                throw new InvalidOperationException("Value must be null.");
        }

        public static string NotEmpty(string arg)
        {
            NotNull(arg);
            if (arg == String.Empty)
                throw new InvalidOperationException("String cannot be empty.");
            return arg;
        }

        public static void NotEmpty(params string[] args)
        {
            foreach (string t in args)
            {
                NotEmpty(t);
            }
        }

        public static T NonNegative<T>(T arg)
            where T : struct, IComparable, IFormattable // numeric
        {
            if (arg.CompareTo(0) < 0)
                throw new InvalidOperationException("Number must be Non-negative.");
            return arg;
        }

        public static T PositiveNonZero<T>(T arg)
            where T : struct, IComparable, IFormattable // numeric
        {
            if (arg.CompareTo(0) <= 0)
                throw new InvalidOperationException("Number must be greater than zero.");
            return arg;
        }

        public static T NotZero<T>(T arg)
            where T : struct, IComparable, IFormattable // numeric
        {
            if (arg.CompareTo(0) == 0)
                throw new InvalidOperationException("Number cannot be zero.");
            return arg;
        }

    }
}
