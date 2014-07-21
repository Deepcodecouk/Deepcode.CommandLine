using System;
using System.Collections.Generic;

namespace Deepcode.CommandLine.Console
{
	internal sealed class OutputColumn
	{
		public List<string> Lines { get; private set; }
		public ConsoleColor Colour { get; private set; }

		public OutputColumn(ConsoleColor colour, IEnumerable<string> content)
		{
			Colour = colour;
			Lines = new List<string>(content);
		}

		public void EnsureHasAtLeastLines(int countLines, string padItem)
		{
			while (Lines.Count < countLines)
				Lines.Add(padItem);
		}
	}
}