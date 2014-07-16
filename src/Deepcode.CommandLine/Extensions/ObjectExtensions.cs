using System.ComponentModel;
using System.Linq;

namespace Deepcode.CommandLine.Extensions
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// Looks at the object provided and finds any attributes with a DefaultValue attribute. It then applys that default value.
		/// </summary>
		/// <param name="instance"></param>
		public static void ApplyDefaultValues<T>(this T instance)
		{
			var propertiesWithDefaults = typeof (T).GetPropertiesWithCustomAttributes<DefaultValueAttribute>();
			foreach (var propertyWithDefault in propertiesWithDefaults)
			{
				// *should* only have one attribute, but just in case, get the last one.
				var defaultValueAttributeToApply = propertyWithDefault.Attributes.Last();

				// apply the value
				propertyWithDefault.Property.SetValue(instance, defaultValueAttributeToApply.Value);
			}
		}
	}
}
