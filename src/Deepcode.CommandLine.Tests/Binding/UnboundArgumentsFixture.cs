using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;
using Deepcode.CommandLine.Tests.Helpers;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class UnboundArgumentsFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		private CommandLineArguments ParseCommandLine(string commandLine)
		{
			var args = commandLine.AsCommandLineArgs();
			var parser = new CommandLineParser();

			return parser.Parse(args);
		}

		public class BindingTest
		{
			[ParameterVerb(StartPosition = 1)]
			public string Command { get; set; }

			[ParameterAlias("r")]
			[ParameterAlias("recursive")]
			public bool IsRecursive { get; set; }

			[ParameterAlias("m")]
			[ParameterAlias("message")]
			public string Message { get; set; }
		}

		[Fact]
		public void Can_Discover_Unbound_Verbs()
		{
			// Arrange
			var args = ParseCommandLine("verb1 verb2 -r -m \"stuff\"");

			// Act
			_binder.CreateAndBindTo<BindingTest>(args);

			// Assert
			_binder.UnboundErrors.Count.ShouldEqual(1);
			_binder.UnboundErrors[0].ShouldEqual("Unknown verb [verb2] at position 2");
		}

		[Fact]
		public void Can_Discover_Unbound_Parameters()
		{
			// Arrange
			var args = ParseCommandLine("verb1 -a do that -b -r -c delete this -m \"stuff\"");

			// Act
			_binder.CreateAndBindTo<BindingTest>(args);

			// Assert
			_binder.UnboundErrors.Count.ShouldEqual(3);
			_binder.UnboundErrors[0].ShouldEqual("Unknown switch option [a] with parameters [do that]");
			_binder.UnboundErrors[1].ShouldEqual("Unknown switch option [b]");
			_binder.UnboundErrors[2].ShouldEqual("Unknown switch option [c] with parameters [delete this]");
		}



		[Fact]
		public void Can_Discover_Unbound_Verbs_And_Parameters()
		{
			// Arrange
			var args = ParseCommandLine("verb1 verb2 -a -b something -r -c delete this -m:\"stuff\"");

			// Act
			_binder.CreateAndBindTo<BindingTest>(args);

			// Assert
			_binder.UnboundErrors.Count.ShouldEqual(4);
			_binder.UnboundErrors[0].ShouldEqual("Unknown verb [verb2] at position 2");
			_binder.UnboundErrors[1].ShouldEqual("Unknown switch option [a]");
			_binder.UnboundErrors[2].ShouldEqual("Unknown switch option [b] with parameters [something]");
			_binder.UnboundErrors[3].ShouldEqual("Unknown switch option [c] with parameters [delete this]");
		}
	}
}
