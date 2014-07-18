using System;
using System.Collections.Generic;
using System.Linq;
using Deepcode.CommandLine.Extensions;

namespace Deepcode.CommandLine.Application
{
	public class CommandDictionaryEntry
	{
		private readonly IList<string> _aliases;

		public Type CommandType{ get; set; }
		public string Group { get; set; }
		public string Description { get; set; }
		public string MainAlias { get; set; }
		// Add props for parameter bindings

		public IEnumerable<string> Aliases
		{
			get { return _aliases; }
			set
			{
				_aliases.Clear();

				foreach (var alias in value)
					WithAlias(alias);
			}
		}

		public CommandDictionaryEntry()
		{
			_aliases = new List<string>();
		}

		public CommandDictionaryEntry(Type commandType)
		{
			CommandType = commandType;
		}

		public CommandDictionaryEntry WithGroup(string groupName)
		{
			Group = groupName;
			return this;
		}

		public CommandDictionaryEntry WithDescription(string description)
		{
			Description = description;
			return this;
		}

		public CommandDictionaryEntry WithAlias(string alias)
		{
			if (HasAlias(alias))
				throw new InvalidOperationException(String.Format("Cannot add duplicate alias {0} command {1}", alias, MainAlias));

			if( String.IsNullOrEmpty(MainAlias)) MainAlias = alias;

			_aliases.Add(alias);
			return this;
		}

		public bool HasAlias(string alias)
		{
			return HasAlias(new[] {alias});
		}

		public bool HasAlias(IEnumerable<string> aliases)
		{
			return (_aliases.Any(existing => aliases.Any(alias => alias == existing)));
		}
	}

	public class CommandDictionary
	{
		private readonly List<CommandDictionaryEntry> _dictionary;

		public CommandDictionary()
		{
			_dictionary = new List<CommandDictionaryEntry>();
		}

		public void AddCommand(CommandDictionaryEntry command)
		{
			// Ensure dictionary doesn't contain any aliases that this new entry contains
			if (_dictionary.Any(entry => entry.HasAlias(command.Aliases)))
				throw new InvalidOperationException(String.Format("Cannot add command {0} as another command already binds it's alias", command.MainAlias));
		}

		public void AddCommand(params Type[] commandTypes)
		{
			foreach (var type in commandTypes)
			{
				var info = type.GetCustomAttribute<CommandInfoAttribute>() ?? new CommandInfoAttribute();
				var aliases = type.GetCustomAttributes<CommandAliasAttribute>();

				if( aliases.Length < 1 ) continue;

				if (_dictionary.Any(existing => existing.Aliases.Any(alias => aliases.Any(a => a.Alias == alias))))
					throw new InvalidOperationException("Duplicate switch alias added");

				var entry = new CommandDictionaryEntry
				{
					Group = info.Group,
					Description = info.Description,
					MainAlias = aliases[0].Alias,
					Aliases = aliases.Select( a => a.Alias ).ToArray(),
					CommandType = type
				};

				_dictionary.Add(entry);
			}
		}

/*		public void SetDefault(Type commandType)
		{
			_dictionary.ForEach(d => d.IsDefault = false);
			_dictionary.First(d => d.CommandType == commandType).IsDefault = true;
		}*/

	}
}