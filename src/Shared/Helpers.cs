using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;

namespace Transportation.Demo.Shared
{
    public static class Helpers
    {
        public static byte[] GetBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
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

        public static ExpandoObject ToExpandoObject(this object obj)
        {
            if (obj is null)
            {
                return null;
            }
            var expando = new ExpandoObject() as IDictionary<string, object>;
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
            {
                expando.Add(property.Name, property.GetValue(obj));
            }
            return (ExpandoObject)expando;
        }
    }
}
