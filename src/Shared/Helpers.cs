using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;

namespace Transportation.Demo.Shared
{
    public static class Helpers
    {
        public static byte[] GetBytes(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach(T item in enumeration)
            {
                action(item);
            }
        }

        public static IEnumerable<TResult> ForEach<T, TResult>(this IEnumerable<T> enumeration, Func<T, TResult> func)
        {
            foreach (T item in enumeration)
            {
                var result = func(item);
                yield return result;
            }
        }
    }
}
