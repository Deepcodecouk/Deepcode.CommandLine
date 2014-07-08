using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class VerbBindingFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		public class TargetBindingSimple
		{
			[ParameterVerb]
			public string [] GreedyVerbs { get; set; }

			[ParameterVerb(StartPosition=1)]
			public string FirstVerb { get; set; }

			[ParameterVerb(StartPosition=2)]
			public string SecondVerb { get; set; }
			
			[ParameterVerb(StartPosition=2, Greedy=true)]
			public string [] SecondVerbGreedy { get; set; }

			[ParameterVerb(StartPosition=1, EndPosition=2)]
			public string[] FirstAndSecondVerb { get; set; }
		}

		[Fact]
		public void Can_Bind_Verbs_With_Position()
		{
			// Arrange
			var args = new CommandLineArguments().AddVerb("first").AddVerb("second").AddVerb("third");
			
			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.GreedyVerbs.Length.ShouldEqual(3);
			result.GreedyVerbs[0].ShouldEqual("first");
			result.GreedyVerbs[1].ShouldEqual("second");
			result.GreedyVerbs[2].ShouldEqual("third");

			result.FirstVerb.ShouldEqual("first");

			result.SecondVerb.ShouldEqual("second");

			result.SecondVerbGreedy.Length.ShouldEqual(2);
			result.SecondVerbGreedy[0].ShouldEqual("second");
			result.SecondVerbGreedy[1].ShouldEqual("third");

			result.FirstAndSecondVerb.Length.ShouldEqual(2);
			result.FirstAndSecondVerb[0].ShouldEqual("first");
			result.FirstAndSecondVerb[1].ShouldEqual("second");
		}
	}
}
