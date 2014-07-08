using System;
using System.Collections.Generic;
using System.Linq;

namespace Deepcode.CommandLine.Parser
{
	/// <summary>
	/// Represents a parsed command line - the command line switches and values
	/// are parsed into a structured collection that is exposed from this class.
	/// </summary>
	public class CommandLineArguments
	{
		private readonly Dictionary<string, List<string>> _tokens;

		/// <summary>
		/// Gets the list of switches that have been parsed
		/// </summary>
		public string[] Switches
		{
			get
			{
				return _tokens.Keys.ToArray();
			}
		}

		/// <summary>
		/// Gets the list of verbs - this is the same as using Switch("")
		/// </summary>
		public string[] Verbs
		{
			get
			{
				if (HasSwitch(""))
					return Switch("");

				return new string[0];
			}
		}

		/// <summary>
		/// Initialise an instance of CommandLine 
		/// </summary>
		public CommandLineArguments()
		{
			_tokens = new Dictionary<string, List<string>>();
		}

		/// <summary>
		/// Returns true/false if the parser has found a switch with the given name
		/// </summary>
		/// <param name="switchName"></param>
		/// <returns></returns>
		public bool HasSwitch(string switchName)
		{
			return _tokens.ContainsKey(switchName);
		}

		/// <summary>
		/// Gets the values associated with the switch with the given name
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string[] Switch(string key)
		{
			return _tokens[key].ToArray();
		}

		/// <summary>
		/// Gets a single string value for a given switch. If multiple values have
		/// been presented against the switch, these are joined together.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string SwitchMerged(string key)
		{
			return string.Join(" ", _tokens[key]);
		}

		/// <summary>
		/// Adds the switch and value pair to the structured collection.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public CommandLineArguments AddValue(string key, string value)
		{
			if (! HasSwitch(key)) _tokens.Add(key, new List<string>());
			if (! String.IsNullOrEmpty(value)) _tokens[key].Add(value);

			return this;
		}
	}
}