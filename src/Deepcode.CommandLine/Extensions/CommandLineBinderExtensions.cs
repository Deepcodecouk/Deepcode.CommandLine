using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;

namespace Deepcode.CommandLine.Extensions
{
	public static class CommandLineBinderStringArrayExtensions
	{
		public static T ParseAndBindCommandLine<T>(this string[] args)
		{
			var arguments = new CommandLineParser().Parse(args);
			var binder = new CommandLineBinder();
			return binder.CreateAndBindTo<T>(arguments);

		}
	}
}
