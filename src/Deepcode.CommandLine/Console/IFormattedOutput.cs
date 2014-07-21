using System;

namespace Deepcode.CommandLine.Console
{
	/// <summary>
	/// Abstraction around writing output to the console.
	/// </summary>
	public interface IFormattedOutput
	{
		void WriteFormat(string format, params object[] arguments);
		void WriteFormat(int indent, string format, params object[] arguments);
		void WriteLine(string format, params object[] arguments);
		void WriteLine(int indent, string format, params object[] arguments);
		void WriteFormat(ConsoleColor colour, string format, params object[] arguments);
		void WriteFormat(ConsoleColor colour, int indent, string format, params object[] arguments);
		void NewLine();
	}
}