using System.ComponentModel;
using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Extensions;
using Deepcode.CommandLine.Tests.Helpers;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class BinderWithDefaultsFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		private HasDefaultsCommandLine Bind(string commandLine)
		{
			return commandLine.AsCommandLineArgs().ParseAndBindCommandLine<HasDefaultsCommandLine>();
		}

		public class HasDefaultsCommandLine
		{
			[ParameterVerb]
			[DefaultValue("defaultVerb")]
			public string TheVerb { get; set; }

			[ParameterAlias("n")]
			[DefaultValue(10)]
			public int SomeNumber { get; set; }

			[ParameterAlias("b")]
			[DefaultValue(true)]
			public bool SomeBool { get; set; }

			[ParameterAlias("s")]
			[DefaultValue("some string")]
			public string SomeString { get; set; }
		}


		[Fact]
		public void Given_No_Command_Line_Properties_Default()
		{
			// Arrange
			var command = Bind("");

			// Assert
			command.TheVerb.ShouldEqual("defaultVerb");
			command.SomeNumber.ShouldEqual(10);
			command.SomeBool.ShouldEqual(true);
			command.SomeString.ShouldEqual("some string");
		}
	}
}