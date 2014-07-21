using System;
using Deepcode.CommandLine.Console;

namespace Deepcode.CommandLine.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			var console = new ConsoleFormattedOutput();

			var layout = new FormattedConsoleTable(console)
				.WithColumn(10, false)
				.WithColumn(20, true, ConsoleColor.Yellow)
				.WithColumn(20, true, ConsoleColor.Magenta)
				.WithColumn(40, true);

			var headerLayout = new FormattedConsoleTable(console)
				.WithColumn(10, false, ConsoleColor.Cyan)
				.WithColumn(20, false, ConsoleColor.Cyan)
				.WithColumn(20, false, ConsoleColor.Cyan)
				.WithColumn(40, false, ConsoleColor.Cyan);

			layout.GutterWidth = 10;
			headerLayout.GutterWidth = 10;

			headerLayout.Write();
			headerLayout.Write("Column 1", "Column 2", "Column 3", "Column 4");
			layout.Write("This is column 1",
				"While this is column 2 in yellow",
				"And this is column 3 in gray or dynamic!",
				"Lastly this is column 4");
			layout.Write();

		}
	}
}
