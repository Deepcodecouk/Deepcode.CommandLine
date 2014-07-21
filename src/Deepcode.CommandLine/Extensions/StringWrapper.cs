using System.Collections.Generic;
using System.Linq;

namespace Deepcode.CommandLine.Extensions
{
	public class StringWrapper
	{
		private readonly int _width;
		private readonly bool _wordWrap;
		private readonly bool _pad;

		private string _currentLine;
		private List<string> _currentResults;

		private bool HasLineBuffer
		{
			get { return !string.IsNullOrEmpty(_currentLine); }
		}


		public StringWrapper(int width, bool wordWrap, bool pad)
		{
			_width = width;
			_wordWrap = wordWrap;
			_pad = pad;
		}

		public string[] WrapString(string input)
		{
			_currentLine = "";
			_currentResults = new List<string>();

			foreach (var word in input.NormaliseWhitespace().GetWords())
				AddWord(word);

			if (HasLineBuffer) Flush();

			if (_pad)
				_currentResults = _currentResults.Select(r => r.PadRight(_width)).ToList();

			return _currentResults.ToArray();
		}


		private void AddWord(string word)
		{
			if (ProcessNewLines(word)) return;

			AddSpaceIfNecessary();
			
			if (WordFitsOnCurrentLine(word))
			{
				_currentLine += word;
				return;
			}

			if (_wordWrap && WordFitsOnOwnLine(word))
			{
				Flush();
				_currentLine = word;
				return;
			}

			// word doesn't fit on current line, and we're
			// either not wrapping, or the word can't fit on it's own line

			var splitPoint = GetWordBreakPoint(word);
			var leftSide = word.Substring(0, splitPoint);
			var rightSide = word.Substring(splitPoint);

			_currentLine += leftSide;
			Flush();
			AddWord(rightSide);

		}

		private void AddSpaceIfNecessary()
		{
			if (!string.IsNullOrEmpty(_currentLine)) _currentLine += " ";
		}

		private int GetWordBreakPoint(string word)
		{
			var wordLength = word.Length;
			var newCombinedLength = _currentLine.Length + wordLength;
			var overflow = newCombinedLength - _width;

			return wordLength - overflow;
		}

		private bool WordFitsOnCurrentLine(string word)
		{
			var wordLength = word.Length;
			var newCombinedLength = _currentLine.Length + wordLength;
			var overflow = newCombinedLength - _width;

			return overflow <= 0;
		}

		private bool WordFitsOnOwnLine(string word)
		{
			return (word.Length <= _width);
		}

		private bool ProcessNewLines(string word)
		{
			if (!word.Contains("\n")) return false;

			var currentWordForAdding = "";
			foreach (var character in word)
			{
				if (character == '\n')
				{
					AddWord(currentWordForAdding);
					currentWordForAdding = "";
					Flush();
				}
				else
				{
					currentWordForAdding += character;
				}
			}

			if (!string.IsNullOrEmpty(currentWordForAdding))
				AddWord(currentWordForAdding);

			return true;
		}

		private void Flush()
		{
			_currentResults.Add(_currentLine.Trim());
			_currentLine = "";
		}
	}
}