using System;
using System.Collections.Generic;

namespace Files.Extensions
{
    internal static class StackExtensions
    {
        public static Stack<T> CloneStack<T>(this Stack<T> stack)
        {
            var copy = new T[stack.Count];
            stack.CopyTo(copy, 0);
            Array.Reverse(copy);
            return new Stack<T>(copy);
        }
    }
}
