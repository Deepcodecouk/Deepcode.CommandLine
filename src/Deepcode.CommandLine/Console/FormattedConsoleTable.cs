using System;
using System.Collections.Generic;
using System.Linq;

namespace Deepcode.CommandLine.Console
{
	/// <summary>
	/// Defines a console table layout with columns. This can then be written to and the
	/// output will be formatted accordingly.
	/// </summary>
	public class FormattedConsoleTable
	{
		private readonly IFormattedOutput _output;
		private readonly List<FormattedConsoleTableColumn> _columns;

		/// <summary>
		/// Get/Set the spacing between column outputs
		/// </summary>
		public int GutterWidth { get; set; }

		public FormattedConsoleTable(IFormattedOutput output)
		{
			_output = output;
			_columns = new List<FormattedConsoleTableColumn>();
			
			GutterWidth = 1;
		}

		/// <summary>
		/// Adds the specified column with width and wrap as specified
		/// </summary>
		/// <param name="width"></param>
		/// <param name="wrap"></param>
		/// <returns></returns>
		public FormattedConsoleTable WithColumn(int width, bool wrap)
		{
			_columns.Add(new FormattedConsoleTableColumn(width, wrap));
			return this;
		}

		/// <summary>
		/// Adds the specified column with width and wrap as specified and with a
		/// default output colour.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="wrap"></param>
		/// <param name="colour"></param>
		/// <returns></returns>
		public FormattedConsoleTable WithColumn(int width, bool wrap, ConsoleColor colour)
		{
			_columns.Add(new FormattedConsoleTableColumn(width, wrap, colour));
			return this;
		}

		/// <summary>
		/// Adds the specified column with the width, wrap and default colour specified
		/// and sets up a callback function to dynamically determine the output colour.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="wrap"></param>
		/// <param name="defaultColour"></param>
		/// <param name="colourCallback"></param>
		/// <returns></returns>
		public FormattedConsoleTable WithColumn(int width, bool wrap, ConsoleColor defaultColour, Func<object, ConsoleColor?> colourCallback)
		{
			_columns.Add(new FormattedConsoleTableColumn(width, wrap, defaultColour)
			{
				DynamicColourCallback = colourCallback
			});

			return this;
		}

		/// <summary>
		/// Writes a row to the console using the layout defined in this class. Pass in 
		/// the data to output in each column in the arguments.
		/// </summary>
		/// <param name="arguments"></param>
		public void Write(params object[] arguments)
		{
			var renderColumns = CreateTableFromArguments(arguments);

			var totalLines = renderColumns.Max(c => c.Lines.Count);
			EnsureAllOutputColumnsHaveSameAmountOfLines(totalLines, renderColumns);

			for (var lineIndex = 0; lineIndex < totalLines; lineIndex++)
			{
				for (var columnIndex=0; columnIndex<renderColumns.Count; columnIndex++)
				{
					if (columnIndex != 0) _output.WriteFormat("".PadRight(GutterWidth));

					var cellColour = renderColumns[columnIndex].Colour;
					var cellLineValue = renderColumns[columnIndex].Lines[lineIndex];

					_output.WriteFormat(cellColour, cellLineValue);
				}

				_output.NewLine();
			}
		}

		private IList<OutputColumn> CreateTableFromArguments(object [] arguments)
		{
			var renderColumns = new List<OutputColumn>();
			for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
			{
				var columnDefinition = _columns[columnIndex];
				var outputValue = arguments.Length > columnIndex ? arguments[columnIndex] : null;
				var outputColour = columnDefinition.GetColour(outputValue);
				var outputText = columnDefinition.GetColumnOutput(outputValue);
				var renderColumn = new OutputColumn(outputColour, outputText);

				renderColumns.Add(renderColumn);
			}

			return renderColumns;
		}

		private void EnsureAllOutputColumnsHaveSameAmountOfLines(int totalLines, IList<OutputColumn> columns)
		{
			for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
			{
				var emptyColumn = _columns[columnIndex].GetEmptyColumn();
				columns[columnIndex].EnsureHasAtLeastLines(totalLines, emptyColumn);
			}
		}
	}
}