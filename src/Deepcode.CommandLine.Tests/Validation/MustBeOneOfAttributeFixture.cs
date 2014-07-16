using System.Linq;
using Deepcode.CommandLine.Validation;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Validation
{
	public class MustBeOneOfAttributeFixture
	{
		public class TestClass
		{
			[MustBeOneOf("option1", "option2", "option3", ErrorMessage="Option must be option1, option2 or option3")]
			public string OptionProperty { get; set; }

			[MustBeOneOf(1,2,4,5, ErrorMessage="Numeric must be 1,2,4 or 5")]
			public int NumericOptionProperty { get; set; }
		}

		[Fact]
		public void Given_Valid_Object_Then_Validation_Succeeds()
		{
			// Arrange
			var model = new TestClass {OptionProperty = "option2", NumericOptionProperty = 4};

			// Act
			var validator = new ObjectValidator();
			var validationResult = validator.Validate(model);

			// Assert
			validationResult.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Given_Invalid_Object_Then_Validation_Fails()
		{
			// Arrange
			var model = new TestClass {OptionProperty = "bunnies!", NumericOptionProperty = 3};

			// Act
			var validator = new ObjectValidator();
			var validationResult = validator.Validate(model);

			// Assert
			validationResult.IsValid.ShouldBeFalse();
			validationResult.Results.Count.ShouldEqual(2);

			var errors = validationResult.Results.Select(vr => vr.ErrorMessage).ToList();
			errors.ShouldContain("Option must be option1, option2 or option3");
			errors.ShouldContain("Numeric must be 1,2,4 or 5");
		}
	}
}
