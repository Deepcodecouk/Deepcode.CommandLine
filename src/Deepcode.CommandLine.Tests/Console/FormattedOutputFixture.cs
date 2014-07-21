using System;
using System.Collections.Generic;
using System.Linq;
using Deepcode.CommandLine.Extensions;
using FakeItEasy;
using Should;
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

	public class DebugFormattedOutput : IFormattedOutput
	{
		private List<string> _lines;
		private List<string> _linesWithMarkup;

		public DebugFormattedOutput()
		{
			_lines = new List<string>();
			_linesWithMarkup = new List<string>();
			_lines.Add("");
			_linesWithMarkup.Add("");
		}

		public void WriteFormat(string format, params object[] arguments)
		{
			Append(string.Format(format, arguments));
		}

		public void WriteFormat(int indent, string format, params object[] arguments)
		{
			Append(string.Format(format, arguments), new string('~', indent));
		}

		public void WriteFormat(ConsoleColor colour, string format, params object[] arguments)
		{
			Append(string.Format(format, arguments), "[" + colour + "]");
		}

		public void WriteFormat(ConsoleColor colour, int indent, string format, params object[] arguments)
		{
			Append(string.Format(format, arguments), "[" + colour + "]" + new string('~', indent));
		}

		public void NewLine()
		{
			_lines.Add("");
			_linesWithMarkup.Add("");
		}

		private void Append(string value, string markup = null)
		{
			_lines[_lines.Count-1] += value;
			_linesWithMarkup[_linesWithMarkup.Count - 1] += (markup + value);
		}


		public void AssertLine(int lineIndex, string match)
		{
			_lines.Count.ShouldBeGreaterThan(lineIndex);
			_lines[lineIndex].ShouldEqual(match);
		}

		public void AssertLineMarkup(int lineIndex, string match)
		{
			_linesWithMarkup.Count.ShouldBeGreaterThan(lineIndex);
			_linesWithMarkup[lineIndex].ShouldEqual(match);
		}

		public void AssertLineCount(int count)
		{
			_lines.Count.ShouldEqual(count);
		}
	}

	public class FormattedConsoleTable
	{

		private readonly IFormattedOutput _output;
		private readonly List<FormattedConsoleTableColumn> _columns;

		public FormattedConsoleTable(IFormattedOutput output)
		{
			_output = output;
			_columns = new List<FormattedConsoleTableColumn>();
		}

		public FormattedConsoleTable WithColumn(FormattedConsoleTableColumn column)
		{
			_columns.Add(column);
			return this;
		}

		public FormattedConsoleTable WithColumn(int width, bool wrap)
		{
			_columns.Add(new FormattedConsoleTableColumn(width, wrap));
			return this;
		}

		public FormattedConsoleTable WithColumn(int width, bool wrap, ConsoleColor colour)
		{
			_columns.Add(new FormattedConsoleTableColumn(width, wrap, colour));
			return this;
		}

		public FormattedConsoleTable WithColumn(int width, bool wrap, ConsoleColor defaultColour, Func<object, ConsoleColor?> colourCallback)
		{
			_columns.Add(new FormattedConsoleTableColumn(width, wrap, defaultColour)
			{
				DynamicColourCallback = colourCallback
			});

			return this;
		}

		private class OutputColumn
		{
			public List<string> Lines { get; set; }
			public ConsoleColor Colour { get; set; }

			public OutputColumn(ConsoleColor colour, IEnumerable<string> content)
			{
				Colour = colour;
				Lines = new List<string>(content);
			}

			public void PadLines(int countLines, string padItem)
			{
				while (Lines.Count < countLines)
					Lines.Add(padItem);
			}
		}

		public void Write(params object[] arguments)
		{
			var renderColumns = new List<OutputColumn>();
			for(var columnIndex = 0; columnIndex<_columns.Count; columnIndex++)
			{
				var columnDefinition = _columns[columnIndex];
				var argument = arguments.Length > columnIndex ? arguments[columnIndex] : null;
				var renderColumn = new OutputColumn(columnDefinition.GetColour(argument), columnDefinition.GetColumnOutput(argument));
				renderColumns.Add(renderColumn);
			}

			// max lines?
			var maxLines = renderColumns.Max(c => c.Lines.Count);
			for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
			{
				renderColumns[columnIndex].PadLines(maxLines, _columns[columnIndex].GetEmptyColumn());
			}

			for (var line = 0; line < maxLines; line++)
			{
				for (var column=0; column<renderColumns.Count; column++)
				{
					if (column != 0) _output.WriteFormat(" ");
					_output.WriteFormat(renderColumns[column].Colour, renderColumns[column].Lines[line]);
				}

				_output.NewLine();
			}
		}
	}

	public class FormattedConsoleTableColumn
	{
		private ConsoleColor? _colour;

		public int Width { get; set; }
		public bool Wrap { get; set; }
		public Func<object, ConsoleColor?> DynamicColourCallback { get; set; }

		public ConsoleColor GetColour(object value)
		{
			if (DynamicColourCallback != null)
			{
				var result = DynamicColourCallback(value);
				if (result.HasValue) return result.Value;
			}

			return _colour.HasValue ? _colour.Value : System.Console.ForegroundColor;
		}

		public string[] GetColumnOutput(object value)
		{
			var outputText = value != null ? value.ToString() : "";
			if (Wrap)
			{
				return outputText.Wrap(Width, true, true);
			}
			else
			{
				if (outputText.Length <= Width) return new[] {outputText.PadRight(Width)};
				return new[] {outputText.Substring(0, Width - 3) + "..."};
			}
		}

		public string GetEmptyColumn()
		{
			return "".PadRight(Width);
		}

		public FormattedConsoleTableColumn(int width=80, bool wrap=false, ConsoleColor? colour=null)
		{
			_colour = colour;
			Width = width;
			Wrap = wrap;
		}
	}


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
			_output.AssertLine(0, "This is... While this is column And this is column 3 Lastly this is      ");
			_output.AssertLine(1, "           2 in yellow          in gray or dynamic!  column 4            ");
			_output.AssertLineMarkup(0, "[Gray]This is... [DarkYellow]While this is column [Magenta]And this is column 3 [Gray]Lastly this is      ");
			_output.AssertLineMarkup(1, "[Gray]           [DarkYellow]2 in yellow          [Magenta]in gray or dynamic!  [Gray]column 4            ");
		}
	}


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
	}
}
