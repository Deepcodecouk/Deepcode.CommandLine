using System;

namespace Deepcode.CommandLine.Binding
{
	/// <summary>
	/// Marks a property on a target binding class as receiveing the verb/verbs
	/// from the command line arguments. This is the equivalent of using [ParameterAlias("")]
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class ParameterVerbAttribute : Attribute
	{
	}
}