using System;
using Deepcode.CommandLine.Console;
using FakeItEasy;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Console
{
	public class FormattedConsoleTableColumnFixture
	{
		[Fact]
		public void Given_Column_With_Defaults_Uses_Defaults()
		{
			// arrange / act
			var column = new FormattedConsoleTableColumn();

			// assert
			column.Width.ShouldEqual(80);
			column.Wrap.ShouldEqual(false);
			column.GetColour(null).ShouldEqual(System.Console.ForegroundColor);
		}

		[Fact]
		public void Given_Column_With_Provided_Attributes_Uses_Values_Specified()
		{
			// arrange / act
			var column = new FormattedConsoleTableColumn(width: 15, wrap: true, colour: ConsoleColor.Red);

			// assert
			column.Width.ShouldEqual(15);
			column.Wrap.ShouldEqual(true);
			column.GetColour(null).ShouldEqual(ConsoleColor.Red);
		}

		[Fact]
		public void Given_Column_With_Calculated_Colour_Uses_Colour_Returned()
		{
			// arrange
			var column = new FormattedConsoleTableColumn(colour: ConsoleColor.Cyan)
			{
				DynamicColourCallback = (val) =>
				{
					if (!(val is int)) return null;

					var numericValue = (int)val;
					if (numericValue > 200) return ConsoleColor.Red;
					return null;
				}
			};

			// act
			var defaultColumn1 = column.GetColour("Hello");
			var redColumnUnderThreshold = column.GetColour(200);
			var redColumnOverThreshold = column.GetColour(201);
			var redColumnOverThresholdInvalidValue = column.GetColour("300");

			// assert
			defaultColumn1.ShouldEqual(ConsoleColor.Cyan);
			redColumnUnderThreshold.ShouldEqual(ConsoleColor.Cyan);
			redColumnOverThreshold.ShouldEqual(ConsoleColor.Red);
			redColumnOverThresholdInvalidValue.ShouldEqual(ConsoleColor.Cyan);
		}

		[Fact]
		public void Given_Column_With_Wrap_When_Get_Output_Then_Wraps()
		{
			// arrange
			var column = new FormattedConsoleTableColumn(width: 20, wrap: true);

			// act
			var result = column.GetColumnOutput("This is a long piece of text that has hopefully wrapped");

			// assert
			result.Length.ShouldEqual(3);
			result[0].ShouldEqual("This is a long piece");
			result[1].ShouldEqual("of text that has    ");
			result[2].ShouldEqual("hopefully wrapped   ");
		}

		[Fact]
		public void Given_Column_With_No_Wrap_When_Get_Output_Then_Trims()
		{
			// arrange
			var column = new FormattedConsoleTableColumn(width: 20, wrap: false);

			// act
			var result = column.GetColumnOutput("This is a long piece of text that has hopefully not wrapped");

			// assert
			result.Length.ShouldEqual(1);
			result[0].ShouldEqual("This is a long pi...");
		}

		[Fact]
		public void Given_Column_When_Get_Empty_Then_Returns_CorrectWidth()
		{
			// arrange
			var column = new FormattedConsoleTableColumn(width: 40);

			// act
			var result = column.GetEmptyColumn();

			// assert
			result.Length.ShouldEqual(40);
			result.ShouldEqual("                                        ");
		}
	}
}
