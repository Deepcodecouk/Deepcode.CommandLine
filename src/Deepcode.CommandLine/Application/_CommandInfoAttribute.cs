using System;

namespace Deepcode.CommandLine.Application
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class CommandInfoAttribute : Attribute
	{
		public string Group { get; set; }
		public string Description { get; set; }

		public CommandInfoAttribute()
		{
		}

		public CommandInfoAttribute(string group, string description)
		{
			Group = group;
			Description = description;
		}

		public CommandInfoAttribute(string description)
		{
			Description = description;
		}
	}
}