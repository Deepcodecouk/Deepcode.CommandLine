using System;

namespace Deepcode.CommandLine.Application
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	public class CommandAliasAttribute : Attribute
	{
		public string Alias { get; set; }

		public CommandAliasAttribute(string alias)
		{
			Alias = alias;
		}
	}
}
