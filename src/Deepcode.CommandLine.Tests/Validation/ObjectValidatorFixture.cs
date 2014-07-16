using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Extensions;
using Deepcode.CommandLine.Tests.Helpers;
using Deepcode.CommandLine.Validation;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Validation
{
	public class ObjectValidatorFixture
	{
		public class TestCommandLineBase
		{
			[ParameterVerb]
			[Required(ErrorMessage="Action is required")]
			public string Action { get; set; }

			[ParameterAlias("port")]
			[Range(100, 200, ErrorMessage="Port must be 100-200")]
			public int Port { get; set; }

			[ParameterAlias("threads")]
			[Range(1, 10, ErrorMessage="Threads must be 1-10")]
			[DefaultValue(2)]
			public virtual int Threads { get; set; }
		}

		public class TestCommandLineDerived : TestCommandLineBase
		{
			[ParameterAlias("more")]
			[Required(ErrorMessage="The extended parameter is required")]
			public string ExtendedParameter { get; set; }

			[Range(11, 20, ErrorMessage="Threads must be 11-20")]
			public override int Threads { get; set; }
		}

		private TestCommandLineBase ParseAndBind(string commandLine, bool derived = false)
		{
			if( ! derived )
				return commandLine.AsCommandLineArgs().ParseAndBindCommandLine<TestCommandLineBase>();

			return commandLine.AsCommandLineArgs().ParseAndBindCommandLine<TestCommandLineDerived>();
		}


		[Fact]
		public void Given_Valid_Command_Line_Validates()
		{
			// Arrange
			var command = ParseAndBind("action1 -port 110 -threads 10");

			// Act
			var validator = new ObjectValidator();
			var validationResult = validator.Validate(command);

			// Assert
			validationResult.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Given_Property_With_Default_Validates()
		{
			// Arrange
			var command = ParseAndBind("action1 -port 110");

			// Act
			var validator = new ObjectValidator();
			var validationResult = validator.Validate(command);

			// Assert
			validationResult.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Given_Single_Validation_Failure_Then_Presents_Validation_Failure_Info()
		{
			// Arrange
			var command = ParseAndBind("-port 110");

			// Act
			var validator = new ObjectValidator();
			var validationResult = validator.Validate(command);

			// Assert
			validationResult.IsValid.ShouldBeFalse();
			validationResult.Results.Count.ShouldEqual(1);
			
			var errors = validationResult.Results.Select(vr => vr.ErrorMessage);
			errors.ShouldContain("Action is required");
		}

		[Fact]
		public void Given_Failing_Command_Line_Then_All_Appropriate_Validations_Fail()
		{
			// Arrange
			var command = ParseAndBind("-threads 11");

			// Act
			var validator = new ObjectValidator();
			var validationResult = validator.Validate(command);

			// Assert
			validationResult.IsValid.ShouldBeFalse();
			validationResult.Results.Count.ShouldEqual(3);

			var errors = validationResult.Results.Select(vr => vr.ErrorMessage).ToList();
			errors.ShouldContain("Action is required");
			errors.ShouldContain("Port must be 100-200");
			errors.ShouldContain("Threads must be 1-10");
		}

		[Fact]
		public void Given_Derived_Class_Validation_Inherits_And_Overrides()
		{
			// Arrange
			var command = ParseAndBind("-threads 10", derived:true);

			// Act
			var validator = new ObjectValidator();
			var validationResult = validator.Validate(command);

			// Assert
			validationResult.IsValid.ShouldBeFalse();
			validationResult.Results.Count.ShouldEqual(4);

			var errors = validationResult.Results.Select(vr => vr.ErrorMessage).ToList();
			errors.ShouldContain("Action is required");
			errors.ShouldContain("Port must be 100-200");
			errors.ShouldContain("The extended parameter is required");
			errors.ShouldContain("Threads must be 11-20");
		}
	}
}
