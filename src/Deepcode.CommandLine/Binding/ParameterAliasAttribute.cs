using System;

namespace Deepcode.CommandLine.Binding
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class ParameterAliasAttribute : Attribute
	{
		public string Alias { get; set; }

		public ParameterAliasAttribute(string alias)
		{
			Alias = alias;
		}
	}
}