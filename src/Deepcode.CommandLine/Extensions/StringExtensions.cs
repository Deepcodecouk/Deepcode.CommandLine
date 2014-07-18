using System;
using System.Collections.Generic;
using System.Linq;

namespace Deepcode.CommandLine.Extensions
{
	public static class StringExtensions
	{
		public static bool StartsWith(this string input, char character)
		{
			return input.StartsWith(character.ToString());
		}

		public static string [] Wrap(this string input, int width, bool wordWrap=false, bool pad=false)
		{
			return new StringWrapper().WrapString(input, width, wordWrap, pad);
		}

		public static string NormaliseWhitespace(this string input)
		{
			return string.Join(" ", input.GetWords());
		}

		public static string[] GetWords(this string input)
		{
			return (input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries));
		}
	}

	public class StringWrapper
	{
		private IEnumerable<string> GetWordNewLineArray(IEnumerable<string> words)
		{
			var resultList = new List<string>();
			foreach (var word in words)
			{
				if (word.Contains('\n'))
				{
					var split = word.Split(new[] {'\n'});
					resultList.AddRange(split);
				}
				else
					resultList.Add(word);
			}

			return resultList;
		}

		public string[] WrapString(string input, int width, bool wordWrap=false, bool pad=false)
		{
			var words = GetWordNewLineArray(input.GetWords());
			var wrappedLines = new List<string>();
			var currentLine = "";

			foreach (var word in words)
			{
				if (word == "")
				{
					wrappedLines.Add(currentLine.Trim());
					currentLine = "";
					continue;
				}
				if (!string.IsNullOrEmpty(currentLine)) currentLine += " ";

				var wordLength = word.Length;
				var cl = currentLine.Length;
				var combinedLength = currentLine.Length + wordLength;
				var overflow = combinedLength - width;

				if (overflow < 0)
					currentLine += word;
				else
				{
					// Word wrapping and fits on it's own line
					if (wordWrap && (wordLength <= width))
					{
						wrappedLines.Add(currentLine.Trim());
						currentLine = word;
					}
					else
					{
						// Either not word wrapping or won't fit on own line - wrap however it fits
						var splitPoint = wordLength - overflow;
						currentLine += word.Substring(0, splitPoint);
						wrappedLines.Add(currentLine.Trim());

						var remaining = word.Substring(splitPoint);
						if (remaining.Length <= width) currentLine = remaining; 
						else
							do
							{
								var grabWidth = width;
								if( grabWidth > remaining.Length ) grabWidth = remaining.Length;
								currentLine = remaining.Substring(0, grabWidth);
								remaining = remaining.Substring(grabWidth);
								if (!string.IsNullOrEmpty(remaining))
									wrappedLines.Add(currentLine.Trim());
							} while (!string.IsNullOrEmpty(remaining));
					}
				}
			}

			if (! string.IsNullOrEmpty(currentLine))
				wrappedLines.Add(currentLine.Trim());

			if (pad)
				wrappedLines = wrappedLines.Select(line => line.PadRight(width)).ToList();

			return wrappedLines.ToArray();
		}
	}
}