using System;
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
			return instance;
		}
	}
}
