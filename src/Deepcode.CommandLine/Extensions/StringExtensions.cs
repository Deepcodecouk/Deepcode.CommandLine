namespace Deepcode.CommandLine.Extensions
{
	public static class StringExtensions
	{
		public static bool StartsWith(this string input, char character)
		{
			return input.StartsWith(character.ToString());
		}
	}
}