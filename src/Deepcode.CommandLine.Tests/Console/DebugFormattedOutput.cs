using System;
using System.Collections.Generic;
using Deepcode.CommandLine.Console;
using Should;

namespace Deepcode.CommandLine.Tests.Console
{
	/// <summary>
	/// Debug/Assertable IFormattedOutput
	/// </summary>
	public class DebugFormattedOutput : IFormattedOutput
	{
		private readonly List<string> _lines;
		private readonly List<string> _linesWithMarkup;

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
			var actualCount = _lines.Count;
			if (String.IsNullOrEmpty(_lines[actualCount - 1])) actualCount--;
			actualCount.ShouldEqual(count);
		}
	}
}