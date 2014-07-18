using Deepcode.CommandLine.Extensions;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Extensions
{
	public class StringWrapperFixture
	{
		[Fact]
		public void Given_String_For_Wrapping_But_Does_Not_Require_It_Then_Returns()
		{
			// Arrange
			// Act
			var result = "This is too short".Wrap(20);

			// Assert
			result.Length.ShouldEqual(1);
			result[0].ShouldEqual("This is too short");
		}

		[Fact]
		public void Given_String_For_Wrapping_And_Padding_But_Does_Not_Require_Wrapping_Then_Returns()
		{
			// Arrange
			// Act
			var result = "This is too short".Wrap(20, pad:true);

			// Assert
			result.Length.ShouldEqual(1);
			result[0].ShouldEqual("This is too short   ");
		}

		[Fact]
		public void Given_String_For_Wrapping_Then_Wraps()
		{
			// Arrange
			// Act
			var result = "This is a string that should longwordhere wrap".Wrap(10);

			// Assert
			result.Length.ShouldEqual(5);
			result[0].ShouldEqual("This is a");
			result[1].ShouldEqual("string tha");
			result[2].ShouldEqual("t should l");
			result[3].ShouldEqual("ongwordher");
			result[4].ShouldEqual("e wrap");
		}

		[Fact]
		public void Given_String_For_Wrapping_With_NewLines_Then_Wraps_And_Preserves_NewLine()
		{
			// Arrange
			// Act
			var result = "This is a string\n that should\n\n\n longwordhere wrap".Wrap(10);

			// Assert
			result.Length.ShouldEqual(8);
			result[0].ShouldEqual("This is a");
			result[1].ShouldEqual("string");
			result[2].ShouldEqual("that shoul");
			result[3].ShouldEqual("d");
			result[4].ShouldEqual("");
			result[5].ShouldEqual("");
			result[6].ShouldEqual("longwordhe");
			result[7].ShouldEqual("re wrap");
		}

		[Fact]
		public void Given_String_For_Wrapping_And_Padding_With_NewLines_Then_Wraps_And_Preserves_NewLine()
		{
			// Arrange
			// Act
			var result = "This is a string\n that should\n\n\n longwordhere wrap".Wrap(10, pad:true);

			// Assert
			result.Length.ShouldEqual(8);
			result[0].ShouldEqual("This is a ");
			result[1].ShouldEqual("string    ");
			result[2].ShouldEqual("that shoul");
			result[3].ShouldEqual("d         ");
			result[4].ShouldEqual("          ");
			result[5].ShouldEqual("          ");
			result[6].ShouldEqual("longwordhe");
			result[7].ShouldEqual("re wrap   ");
		}

		[Fact]
		public void Given_String_For_Word_Wrapping_Then_Wraps_On_Word_Boundaries_Where_Possible()
		{
			// Arrange
			// Act
			var result = "This is a string that should longwordhere wrap".Wrap(10, true);

			// Assert
			result.Length.ShouldEqual(6);
			result[0].ShouldEqual("This is a");
			result[1].ShouldEqual("string");
			result[2].ShouldEqual("that");
			result[3].ShouldEqual("should lon");
			result[4].ShouldEqual("gwordhere");
			result[5].ShouldEqual("wrap");
		}

		[Fact]
		public void Given_String_For_Word_Wrapping_And_Padding_Then_Wraps_And_Pads()
		{
			// Arrange
			// Act
			var result = "This is a string that should longwordhere wrap".Wrap(10, true, true);

			// Assert
			result.Length.ShouldEqual(6);
			result[0].ShouldEqual("This is a ");
			result[1].ShouldEqual("string    ");
			result[2].ShouldEqual("that      ");
			result[3].ShouldEqual("should lon");
			result[4].ShouldEqual("gwordhere ");
			result[5].ShouldEqual("wrap      ");
		}

		[Fact]
		public void Given_String_For_Wrapping_And_Padding_Then_Wraps_And_Pads()
		{
			// Arrange
			// Act
			var result = "This is a string that should longwordhere wrap".Wrap(10, pad:true);

			// Assert
			result.Length.ShouldEqual(5);
			result[0].ShouldEqual("This is a ");
			result[1].ShouldEqual("string tha");
			result[2].ShouldEqual("t should l");
			result[3].ShouldEqual("ongwordher");
			result[4].ShouldEqual("e wrap    ");
		}

		[Fact]
		public void Given_String_For_Wrapping_Normalises_Spaces()
		{
			// Arrange
			// Act
			var result = "This is   a string      \nthat\nshould wrap".Wrap(10, wordWrap:true, pad:true);

			// Assert
			result.Length.ShouldEqual(5);
			result[0].ShouldEqual("This is a ");
			result[1].ShouldEqual("string    ");
			result[2].ShouldEqual("that      ");
			result[3].ShouldEqual("should    ");
			result[4].ShouldEqual("wrap      ");
		}
	}
}
