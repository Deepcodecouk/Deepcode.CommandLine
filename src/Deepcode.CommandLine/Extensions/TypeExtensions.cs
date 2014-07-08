using System;
using System.Linq;
using System.Reflection;

namespace Deepcode.CommandLine.Extensions
{
	public class PropertyInfoWithCustomAttribute<T>
	{
		public PropertyInfo Property { get; private set; }
		public T[] Attributes { get; private set; }

		public PropertyInfoWithCustomAttribute(PropertyInfo property, T[] attributes)
		{
			Property = property;
			Attributes = attributes;
		}
	}

	public static class TypeExtensions
	{
		public static T GetCustomAttribute<T>(this Type type) where T : Attribute
		{
			var attribs = type.GetCustomAttributes<T>();
			return attribs.Length < 1 ? default(T) : attribs[0];
		}

		public static T[] GetCustomAttributes<T>(this Type type) where T : Attribute
		{
			var attribs = type.GetCustomAttributes(typeof (T), true);
			if (attribs.Length < 1) return new T[0];

			return attribs.Select(a => a as T).ToArray();
		}

		public static T[] GetCustomAttributes<T>(this PropertyInfo property) where T : Attribute
		{
			var attribs = property.GetCustomAttributes(typeof (T), true);
			if (attribs.Length < 1) return new T[0];

			return attribs.Select(a => a as T).ToArray();
		}

		public static PropertyInfoWithCustomAttribute<T>[] GetPropertiesWithCustomAttributes<T>(this Type type) where T : Attribute
		{
			var properties = type.GetProperties()
								 .Select(p => new PropertyInfoWithCustomAttribute<T>(p, p.GetCustomAttributes<T>()));


			return properties.Where(p => p.Attributes.Length > 0).ToArray();
		}
	}
}
