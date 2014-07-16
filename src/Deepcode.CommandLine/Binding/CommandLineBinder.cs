using System;
using System.Collections.Generic;
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
		public List<string> UnboundErrors { get; private set; }

		/// <summary>
		/// Creates a new instance of the command line binder
		/// </summary>
		public CommandLineBinder()
		{
			UnboundErrors = new List<string>();
		}
		
		/// <summary>
		/// Creates an instance of type T and binds command line arguments to
		/// it based on the binding attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public T CreateAndBindTo<T>(CommandLineArguments arguments)
		{
			var type = typeof (T);
			var constructor = type.GetConstructor(new Type[0]);
			if (constructor == null)
				throw new InvalidOperationException("Target type does not have a default, empty constructor");

			var entity = (T) constructor.Invoke(new object[0]);
			return BindTo(arguments, entity);
		}

		/// <summary>
		/// Binds the command line arguments to the object passed in
		/// based on the binding attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arguments"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public T BindTo<T>(CommandLineArguments arguments, T instance)
		{
			UnboundErrors.Clear();

			instance.ApplyDefaultValues();

			BindVerbs(arguments, instance);
			BindParameters(arguments, instance);
			return instance;
		}

		private void BindVerbs<T>(CommandLineArguments arguments, T instance)
		{
			if( arguments.Verbs.Length < 1 ) return;

			var verbProperties = typeof(T).GetPropertiesWithCustomAttributes<ParameterVerbAttribute>();

			for (var commandLineVerbIndex=0; commandLineVerbIndex < arguments.Verbs.Length; commandLineVerbIndex++)
			{
				var commandLineVerb = arguments.Verbs[commandLineVerbIndex];
				var commandLineVerbPosition = commandLineVerbIndex + 1;

				var targetVerbProperties = verbProperties
					.Where(p => p.Attributes.Any(a => commandLineVerbPosition >= a.StartPosition && commandLineVerbPosition <= a.EndPosition))
					.ToList();

				if (targetVerbProperties.Count < 1)
				{
					UnboundErrors.Add(string.Format("Unknown verb [{0}] at position {1}", commandLineVerb, commandLineVerbPosition));
					continue;
				}

				foreach (var targetVerbProperty in targetVerbProperties)
					BindTo(instance, targetVerbProperty.Property, new[]{ commandLineVerb });
			}
		}

		private void BindParameters<T>(CommandLineArguments arguments, T instance)
		{
			if (arguments.Switches.Length < 1) return;

			var switchProperties = typeof(T).GetPropertiesWithCustomAttributes<ParameterAliasAttribute>();

			foreach (var commandLineSwitch in arguments.Switches)
			{
				var commandLineSwitchParameters = arguments.Switch(commandLineSwitch);

				var targetSwitchProperties = switchProperties
					.Where(s => s.Attributes.Any(a => a.Alias == commandLineSwitch))
					.ToList();

				if (targetSwitchProperties.Count < 1)
				{
					var allParameters = arguments.SwitchMerged(commandLineSwitch);
					UnboundErrors.Add(string.Format("Unknown switch option [{0}]{1}{2}{3}",
						commandLineSwitch,
						string.IsNullOrEmpty(allParameters) ? "" : " with parameters [",
						allParameters,
						string.IsNullOrEmpty(allParameters) ? "" : "]"));

					continue;
				}

				foreach( var targetSwitchProperty in targetSwitchProperties)
					BindTo(instance, targetSwitchProperty.Property, commandLineSwitchParameters);
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
				if( index >= 0 ) property.SetValue(instance, valueArray.GetValue(index));
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
					if (! Int32.TryParse(v, out newValue)) 
						return new { Value = default(int), Parsed = false };

					return new {Value = newValue, Parsed = true};

				}).Where(v => v.Parsed).Select(v => v.Value).ToArray();
			}

			if (targetType == typeof (bool))
			{
				// Specified switch only - no values = true
				if (value.Length < 1) return new[] {true};

				var parsed = value.Select(v =>
				{
					bool newValue;
					if (! bool.TryParse(v, out newValue))
						return new {Value = default(bool), Parsed = false};

					return new {Value = newValue, Parsed = true};
				}).Where(v => v.Parsed).Select(v => v.Value).ToArray();

				// Specified switch but no parsable values = true (they specified the switch)
				if (parsed.Length == 0) return new[] {true};
				return parsed;
			}

			return value;
		}
	}
}
