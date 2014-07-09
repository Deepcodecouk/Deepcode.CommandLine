using System;
using System.Linq;
using Deepcode.CommandLine.Extensions;

namespace Deepcode.CommandLine.Parser
{
	/// <summary>
	/// Parses the command line into a structured collection of switches and values
	/// </summary>
	public class CommandLineParser
	{
		/// <summary>
		/// Defaults to true - when true will parse out switches into
		/// the CommandLineArguments.Switches structure. When false, treats
		/// all arguments as verbs.
		/// </summary>
		public bool ParseSwitches { get; set; }

		/// <summary>
		/// Defaults to ["-", "/"] sets the switch prefixes used to identify
		/// a switch within the command line. 
		/// </summary>
		public char[] SwitchPrefixes { get; set; }

		/// <summary>
		/// Initialises an instance of the command line parser
		/// </summary>
		public CommandLineParser()
		{
			ParseSwitches = true;
			SwitchPrefixes = new[] {'-', '/'};
		}

		/// <summary>
		/// Parses the command line arguments provided into a CommandLineArguments structure
		/// </summary>
		/// <param name="args"></param>
		public CommandLineArguments Parse(string[] args)
		{
			var result = new CommandLineArguments();
			var currentKey = "";
			
			foreach (var arg in args.Where(a => !String.IsNullOrEmpty(a)))
			{
				var tokens = ParseTokens(currentKey, arg);
				result.AddValue(tokens.Switch, tokens.Value);

				currentKey = tokens.Switch;
			}

			return result;
		}

		private ArgumentToken ParseTokens(string currentKey, string argument)
		{
			var token = new ArgumentToken {Switch = currentKey, Value = argument};
			if( ! ParseSwitches ) return token;

			if (SwitchPrefixes.Any(p => token.Value.StartsWith(p)))
			{
				token.Switch = token.Value.TrimStart(SwitchPrefixes);
				token.Value = String.Empty;

				if (token.Switch.Contains(":"))
				{
					var pair = token.Switch.Split(new[] {':'}, 2);
					token.Switch = pair[0];
					token.Value = pair[1];
				}
			}

			return token;
		}

		private class ArgumentToken
		{
			public string Switch { get; set; }
			public string Value { get; set; }
		}
	}
}
