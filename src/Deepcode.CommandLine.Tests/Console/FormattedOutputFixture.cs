using System;
using Xunit;

namespace Deepcode.CommandLine.Tests.Console
{
	public interface IFormattedOutput
	{
		void WriteFormat(string format, params object[] arguments);
		void WriteFormat(int indent, string format, params object[] arguments);
		void WriteFormat(ConsoleColor colour, string format, params object[] arguments);
		void WriteFormat(ConsoleColor colour, int indent, string format, params object[] arguments);
		void NewLine();
	}

	public class FormattedConsoleTable
	{
		private readonly IFormattedOutput _output;

		public FormattedConsoleTable(IFormattedOutput output)
		{
			_output = output;
		}

		public FormattedConsoleTable WithColumn(int width)
		{
			return this;
		}
	}

	public class FormatterConsoleTableColumn
	{
		public int Width { get; set; }
	}
	
	public class FormattedConsoleTableColumnFixture
	{
		[Fact]
		public void Stuff()
		{
		}
	}
}
