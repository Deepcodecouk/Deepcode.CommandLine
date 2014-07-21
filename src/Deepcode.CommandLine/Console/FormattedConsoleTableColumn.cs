using System;
using Deepcode.CommandLine.Extensions;

namespace Deepcode.CommandLine.Console
{
	public class FormattedConsoleTableColumn
	{
		private ConsoleColor? _colour;

		public int Width { get; set; }
		public bool Wrap { get; set; }
		public Func<object, ConsoleColor?> DynamicColourCallback { get; set; }

		/// <summary>
		/// Get the colour to use for this column when rendering the value specified. If
		/// a callback has been provided and it returns a value, that is used, otherwise
		/// it uses the set default colour. If that hasn't been provided, it returns the
		/// current foreground colour.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public ConsoleColor GetColour(object value)
		{
			if (DynamicColourCallback != null)
			{
				var result = DynamicColourCallback(value);
				if (result.HasValue) return result.Value;
			}

			return _colour.HasValue ? _colour.Value : System.Console.ForegroundColor;
		}

		/// <summary>
		/// Given the value provided, return the column output text, either
		/// trimmed or in rows as necessary (if wrapping).
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets a string representing an empty column of this width
		/// </summary>
		/// <returns></returns>
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
}