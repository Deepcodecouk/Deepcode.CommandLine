using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Extensions;
using Deepcode.CommandLine.Tests.Helpers;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Extensions
{
	public class CommandLineBinderExtensionsFixture
	{
		public class TargetBindingSimple
		{
			[ParameterVerb]
			public string Action { get; set; }

			[ParameterAlias("name")]
			[ParameterAlias("n")]
			public string Name { get; set; }

			[ParameterAlias("age")]
			public int Age { get; set; }

			[ParameterAlias("vip")]
			public bool IsVip { get; set; }
		}

		[Fact]
		public void Can_Parse_And_Bind_Entirely()
		{
			// Arrange
			var arguments = "addperson -n joe -age 72 -vip".AsCommandLineArgs();

			// Act
			var result = arguments.ParseAndBindCommandLine<TargetBindingSimple>();

			// Assert
			result.Action.ShouldEqual("addperson");
			result.Name.ShouldEqual("joe");
			result.Age.ShouldEqual(72);
			result.IsVip.ShouldBeTrue();
		}

	}
}
