using System;

namespace Deepcode.CommandLine.Extensions
{
	public static class StringExtensions
	{
		public static bool StartsWith(this string input, char character)
		{
			return input.StartsWith(character.ToString());
		}

		public static string[] Wrap(this string input, int width, bool wordWrap = false, bool pad = false)
		{
			return new StringWrapper(width, wordWrap, pad).WrapString(input);
		}

		public static string NormaliseWhitespace(this string input)
		{
			return string.Join(" ", input.GetWords());
		}

		public static string[] GetWords(this string input)
		{
			return (input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
		}
	}
}