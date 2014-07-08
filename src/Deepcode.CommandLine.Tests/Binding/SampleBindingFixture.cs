using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;
using Deepcode.CommandLine.Tests.Helpers;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class SampleBindingFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		private CommandLineArguments ParseCommandLine(string commandLine)
		{
			var args = commandLine.AsCommandLineArgs();
			var parser = new CommandLineParser();

			return parser.Parse(args);
		}

		public class MySourceControl
		{
			[ParameterVerb(StartPosition = 1)]
			public string Command { get; set; }

			[ParameterVerb(StartPosition = 2, Greedy=true)]
			public string [] FileSpec { get; set; }

			[ParameterAlias("r")]
			[ParameterAlias("recursive")]
			public bool IsRecursive { get; set; }

			[ParameterAlias("m")]
			[ParameterAlias("message")]
			public string Message { get; set; }
		}

		[Fact]
		public void Given_Full_Command_Line_Binds_As_Expected()
		{
			// Arrange
			var args = ParseCommandLine("add *.cs *.proj *.md -r -message \"Some commit message\"");

			// Act
			var result = _binder.CreateAndBindTo<MySourceControl>(args);

			// Assert
			result.Command.ShouldEqual("add");
			result.FileSpec.Length.ShouldEqual(3);
			result.FileSpec[0].ShouldEqual("*.cs");
			result.FileSpec[1].ShouldEqual("*.proj");
			result.FileSpec[2].ShouldEqual("*.md");
			result.IsRecursive.ShouldBeTrue();
			result.Message.ShouldEqual("Some commit message");
		}

		[Fact]
		public void Given_Partial_Command_Line_Binds_As_Expected()
		{
			// Arrange
			var args = ParseCommandLine("stuff -m \"Some message\"");

			// Act
			var result = _binder.CreateAndBindTo<MySourceControl>(args);

			// Assert
			result.Command.ShouldEqual("stuff");
			result.FileSpec.ShouldBeNull();
			result.IsRecursive.ShouldBeFalse();
			result.Message.ShouldEqual("Some message");
		}

		[Fact]
		public void Given_Empty_Command_Line_Binds_As_Expected()
		{
			// Arrange
			var args = ParseCommandLine("");

			// Act
			var result = _binder.CreateAndBindTo<MySourceControl>(args);

			// Assert
			result.Command.ShouldBeNull();
			result.FileSpec.ShouldBeNull();
			result.IsRecursive.ShouldBeFalse();
			result.Message.ShouldBeNull();
		}
	}
}