using System;
using Con = System.Console;

namespace Deepcode.CommandLine.Console
{
	public class ConsoleFormattedOutput : IFormattedOutput
	{
		public bool AllowWrapping { get; set; }
		private bool _isTrimming { get; set; }

		public void WriteFormat(string format, params object[] arguments)
		{
			WriteTrimmed(format, arguments);
		}

		public void WriteFormat(int indent, string format, params object[] arguments)
		{
			WriteTrimmed(Indent(indent));
			WriteFormat(format, arguments);
		}

		public void WriteLine(string format, params object[] arguments)
		{
			WriteFormat(format, arguments);
			NewLine();
		}

		public void WriteLine(int indent, string format, params object[] arguments)
		{
			WriteFormat(indent, format, arguments);
			NewLine();
		}
		
		public void WriteFormat(ConsoleColor colour, string format, params object[] arguments)
		{
			Con.ForegroundColor = colour;
			WriteTrimmed(format, arguments);
			Con.ResetColor();
		}

		public void WriteFormat(ConsoleColor colour, int indent, string format, params object[] arguments)
		{
			WriteTrimmed(Indent(indent));
			WriteFormat(colour, format, arguments);
		}

		public void NewLine()
		{
			if( ! _isTrimming ) Con.WriteLine();
			_isTrimming = false;
		}

		private static string Indent(int indent)
		{
			return new string(' ', indent);
		}

		private void WriteTrimmed(string output, params object[] arguments)
		{
			var formattedOutput = string.Format(output, arguments);

			if (AllowWrapping)
			{
				Con.Write(formattedOutput);
				return;
			}

			if( _isTrimming ) return;

			var x = Con.CursorLeft;
			var newX = x + formattedOutput.Length;
			var maxWidth = Con.BufferWidth;

			if (newX > maxWidth)
			{
				Con.Write(formattedOutput.Substring(0, maxWidth - x));
				_isTrimming = true;
			}
			else
				Con.Write(formattedOutput);
		}
	}
}