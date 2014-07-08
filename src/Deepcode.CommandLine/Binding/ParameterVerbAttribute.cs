using System;

namespace Deepcode.CommandLine.Binding
{
	/// <summary>
	/// Marks a property on a target binding class as receiveing the verb/verbs
	/// from the command line arguments. Verbs being any arguments not prefixed
	/// with a switch...
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ParameterVerbAttribute : Attribute
	{
	}
}