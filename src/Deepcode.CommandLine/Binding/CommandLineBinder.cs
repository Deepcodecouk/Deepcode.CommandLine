using System;
using System.Linq;
using System.Reflection;
using Deepcode.CommandLine.Extensions;
using Deepcode.CommandLine.Parser;

namespace Deepcode.CommandLine.Binding
{
	/// <summary>
	/// The command line binder takes parsed command line arguments and binds them
	/// to a runtime class using the binding attributes
	/// </summary>
	public class CommandLineBinder
	{
		public T CreateAndBindTo<T>(CommandLineArguments arguments)
		{
			var type = typeof (T);
			var constructor = type.GetConstructor(new Type[0]);
			if (constructor == null)
				throw new InvalidOperationException("Target type does not have a default, empty constructor");

			var entity = (T) constructor.Invoke(new object[0]);
			return BindTo(arguments, entity);
		}

		public T BindTo<T>(CommandLineArguments arguments, T instance)
		{
			BindVerbs(arguments, instance);
			BindParameters(arguments, instance);
			return instance;
		}

		private void BindVerbs<T>(CommandLineArguments arguments, T instance)
		{
			if( arguments.Verbs.Length < 1 ) return;

			var verbProperties = typeof (T).GetPropertiesWithCustomAttributes<ParameterVerbAttribute>();

			foreach (var verbProperty in verbProperties)
				BindTo(instance, verbProperty.Property, arguments.Verbs);
		}

		private void BindParameters<T>(CommandLineArguments arguments, T instance)
		{
			if (arguments.Switches.Length < 1) return;

			var switchProperties = typeof(T).GetPropertiesWithCustomAttributes<ParameterAliasAttribute>();

			foreach (var commandSwitch in arguments.Switches)
			{
				var propertyToBindTo = switchProperties.FirstOrDefault(s => s.Attributes.Any(a => a.Alias == commandSwitch));
				if (propertyToBindTo != null)
				{
					BindTo(instance, propertyToBindTo.Property, arguments.Switch(commandSwitch));
				}
				// TODO: If we didn't find anything to bind to, should we error?
			}

/*			foreach (var switchProperty in switchProperties)
			{
				foreach (var switchAttribute in switchProperty.Attributes)
				{
					if( ! arguments.HasSwitch(switchAttribute.Alias)) continue;
					BindTo(instance, switchProperty.Property, arguments.Switch(switchAttribute.Alias));
				}
			}*/
		}

		private void BindTo(object instance, PropertyInfo property, string [] value)
		{
			var targetType = property.PropertyType;
			
			if (targetType.IsArray)
			{
				var innerType = targetType.GetElementType();

				var valueArray = ConvertArrayToType(innerType, value);
				var currentArray = (Array) property.GetValue(instance);
				if (currentArray == null)
				{
					currentArray = Array.CreateInstance(innerType, valueArray.Length);
					Array.Copy(valueArray, currentArray, valueArray.Length);
					property.SetValue(instance, currentArray);
				}
				else
				{
					var copyArray = Array.CreateInstance(innerType, currentArray.Length + valueArray.Length);
					Array.Copy(currentArray, copyArray, currentArray.Length);
					Array.Copy(valueArray, 0, copyArray, currentArray.Length, valueArray.Length);
					copyArray.SetValue(value, currentArray.Length);
				}
			}
			else
			{
				var valueArray = ConvertArrayToType(targetType, value);
				property.SetValue(instance, valueArray.GetValue(valueArray.Length-1));
			}
		}

		private Array ConvertArrayToType(Type targetType, string[] value)
		{
			if( targetType == typeof(string)) return value;

			if (targetType == typeof (int))
			{
				return value.Select(v =>
				{
					int newValue;
					if (! Int32.TryParse(v, out newValue)) return default(int);
					return newValue;
				}).ToArray();
			}

			if (targetType == typeof (bool))
			{
				if (value.Length < 1) return new[] {true};
				
				return value.Select(v =>
				{
					bool newValue;
					if (! bool.TryParse(v, out newValue)) return true;
					return newValue;
				}).ToArray();
			}

			return value;
		}
	}
}
