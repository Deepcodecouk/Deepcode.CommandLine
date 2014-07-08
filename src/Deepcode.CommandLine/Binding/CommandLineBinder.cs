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

			
			for (var verbIndex=0; verbIndex<arguments.Verbs.Length; verbIndex++)
			{
				var verb = arguments.Verbs[verbIndex];
				var verbPosition = verbIndex + 1;
				var bindVerbProperties = verbProperties.Where( p => p.Attributes.Any( a => verbPosition >= a.StartPosition && verbPosition <= a.EndPosition));

				foreach (var bindProperty in bindVerbProperties)
					BindTo(instance, bindProperty.Property, new[]{ verb });
			}

			/*foreach (var verbProperty in verbProperties)
				BindTo(instance, verbProperty.Property, arguments.Verbs, true);*/
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
					property.SetValue(instance, copyArray);
				}
			}
			else
			{			
				var valueArray = ConvertArrayToType(targetType, value);
				var index = valueArray.Length - 1;
				property.SetValue(instance, valueArray.GetValue(index));
			}
		}

		private bool IsStringAndHasValue(object instance, PropertyInfo property, Type targetType)
		{
			if (targetType != typeof (string)) return false;

			var value = property.GetValue(instance);
			return !String.IsNullOrEmpty((string) value);
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
