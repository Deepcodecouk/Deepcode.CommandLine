using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Deepcode.CommandLine.Tests.Helpers
{
	public static class StringToCommandLineArgumentsExtension
	{
		private const string QuotedStrings = "([\"'])(?:(?=(\\\\?))\\2.)*?\\1";
		private const string ReplacementToken = "{{@@::space::@@}}";

		// Yeah, this is all a bit crap really :) must be an easier way to preserve quoted values during a split?
		public static string [] AsCommandLineArgs(this string commandLine)
		{
			// Remove quotes from quoted values and replace spaces with a token
			var parseCommandLine = Regex.Replace(commandLine, QuotedStrings, m => m.Value.Replace(" ", ReplacementToken).Replace("\"", ""));
			
			// Split the command line string and 
			var resultArguments = new List<string>();
			foreach (var arg in parseCommandLine.Split(new[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries))
				resultArguments.Add(arg.Replace(ReplacementToken, " "));

			return resultArguments.ToArray();
		}

	}
}
