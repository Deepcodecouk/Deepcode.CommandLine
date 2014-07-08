using System;
using System.Linq;

namespace Deepcode.CommandLine.Extensions
{
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
	}
}
