using System.Linq;
using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;
using Deepcode.CommandLine.Tests.Helpers;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class ParameterMultipleBindingFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		private CommandLineArguments ParseCommandLine(string commandLine)
		{
			var args = commandLine.AsCommandLineArgs();
			var parser = new CommandLineParser();

			return parser.Parse(args);
		}

		public class Test
		{
			[ParameterAlias("m")]
			public string[] Messages1 { get; set; }

			[ParameterAlias("m")]
			public string[] Messages2 { get; set; }

			[ParameterAlias("m")]
			public string MessageSingle { get; set; }

			[ParameterAlias("m")]
			public int[] MessageValues { get; set; }

			[ParameterAlias("m")]
			public int MessageValueSingle { get; set; }

			[ParameterAlias("m")]
			public bool [] MessageValueBools { get; set; }

			[ParameterAlias("m")]
			public bool MessageValueBoolSingle { get; set; }
		}

		[Fact]
		public void Given_Multiple_Properties_Bound_To_Parameter_All_Are_Bound()
		{
			// Arrange
			var args = ParseCommandLine("-m message1 message2 false 10 20 -m 30 -m:40 true message4");

			// Act
			var result = _binder.CreateAndBindTo<Test>(args);

			// Assert
			result.Messages1.Length.ShouldEqual(9);
			string.Join(" ", result.Messages1).ShouldEqual("message1 message2 false 10 20 30 40 true message4");
			
			result.Messages2.Length.ShouldEqual(9);
			string.Join(" ", result.Messages2).ShouldEqual("message1 message2 false 10 20 30 40 true message4");
			
			result.MessageSingle.ShouldEqual("message4");

			result.MessageValues.Length.ShouldEqual(4);
			result.MessageValues.Sum().ShouldEqual(100);
			result.MessageValueSingle.ShouldEqual(40);

			result.MessageValueBools.Length.ShouldEqual(2);
			result.MessageValueBools[0].ShouldBeFalse();
			result.MessageValueBools[1].ShouldBeTrue();

			result.MessageValueBoolSingle.ShouldBeTrue();
		}
	}
}