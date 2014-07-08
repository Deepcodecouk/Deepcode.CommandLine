using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class VerbBindingTypedFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		public class TargetBindingTyped
		{
			[ParameterVerb]
			public string[] GreedyVerbs { get; set; }

			[ParameterVerb(StartPosition=2)]
			public int SecondVerb { get; set; }

			[ParameterVerb(StartPosition=3)]
			public bool ThirdVerb { get; set; }
		}

		[Fact]
		public void Can_Bind_Verbs_With_Position_And_Type()
		{
			// Arrange
			var args = new CommandLineArguments().AddVerb("first").AddVerb("27").AddVerb("true");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingTyped>(args);

			// Assert
			result.GreedyVerbs.Length.ShouldEqual(3);
			result.GreedyVerbs[0].ShouldEqual("first");
			result.GreedyVerbs[1].ShouldEqual("27");
			result.GreedyVerbs[2].ShouldEqual("true");

			result.SecondVerb.ShouldEqual(27);

			result.ThirdVerb.ShouldEqual(true);
		}
	}
}