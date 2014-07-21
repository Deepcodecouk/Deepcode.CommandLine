using System;
using Deepcode.CommandLine.Console;
using Xunit;

namespace Deepcode.CommandLine.Tests.Console
{
	public class FormattedConsoleTableFixture
	{
		private readonly DebugFormattedOutput _output;

		public FormattedConsoleTableFixture()
		{
			_output = new DebugFormattedOutput();
		}

		[Fact]
		public void Given_Table_Layout_Writes_Accordingly()
		{
			// Arrange
			var layout = new FormattedConsoleTable(_output)
				.WithColumn(10, false)
				.WithColumn(20, true, ConsoleColor.DarkYellow)
				.WithColumn(20, true, ConsoleColor.Magenta)
				.WithColumn(20, true);

			// Act
			layout.Write("This is column 1", 
				"While this is column 2 in yellow", 
				"And this is column 3 in gray or dynamic!",
				"Lastly this is column 4");

			// Assert
			// |        | |                  | |                  | |                   |      
			// This is... While this is column And this is column 3 Lastly this is       
			_output.AssertLineCount(2);
			_output.AssertLine(0, "This is... While this is column And this is column 3 Lastly this is      ");
			_output.AssertLine(1, "           2 in yellow          in gray or dynamic!  column 4            ");
			_output.AssertLineMarkup(0, "[Gray]This is... [DarkYellow]While this is column [Magenta]And this is column 3 [Gray]Lastly this is      ");
			_output.AssertLineMarkup(1, "[Gray]           [DarkYellow]2 in yellow          [Magenta]in gray or dynamic!  [Gray]column 4            ");
		}

		[Fact]
		public void Given_Table_Layout_With_Wider_Gutter_Writes_Accordingly()
		{
			// Arrange
			var layout = new FormattedConsoleTable(_output)
				.WithColumn(10, false)
				.WithColumn(20, true, ConsoleColor.DarkYellow)
				.WithColumn(20, true, ConsoleColor.Magenta)
				.WithColumn(20, true);

			layout.GutterWidth = 5;

			// Act
			layout.Write("This is column 1",
				"While this is column 2 in yellow",
				"And this is column 3 in gray or dynamic!",
				"Lastly this is column 4");

			// Assert
			// |        | |                  | |                  | |                   |      
			// This is... While this is column And this is column 3 Lastly this is       
			_output.AssertLineCount(2);
			_output.AssertLine(0, "This is...     While this is column     And this is column 3     Lastly this is      ");
			_output.AssertLine(1, "               2 in yellow              in gray or dynamic!      column 4            ");
			_output.AssertLineMarkup(0, "[Gray]This is...     [DarkYellow]While this is column     [Magenta]And this is column 3     [Gray]Lastly this is      ");
			_output.AssertLineMarkup(1, "[Gray]               [DarkYellow]2 in yellow              [Magenta]in gray or dynamic!      [Gray]column 4            ");
		}
	}
}