using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StandPoint.Reflection
{
    public static class Extensions
    {
        public static bool HasAttribute<T>(this Type type, out T attribute) where T : Attribute
        {
            attribute = (T)type.GetTypeInfo().GetCustomAttributes(typeof(T), true).SingleOrDefault();
            return (attribute != null);
        }

        public static bool HasAttribute<T>(this MemberInfo member, out T attribute) where T : Attribute
        {
            Type attType = typeof(T);
            attribute = (T)member.GetCustomAttribute(attType);
            return (attribute != null);
        }

        public static bool HasAttributes<T>(this MemberInfo member, out List<T> attributes) where T : Attribute
        {
            Type attType = typeof(T);
            attributes = member.GetCustomAttributes(attType).Cast<T>().ToList();
            return (attributes.Any());
        }
    }
}
