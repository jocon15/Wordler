using Newtonsoft.Json;
using System.Diagnostics;
using WordlerCore.Tile;

namespace WordlerCore.Filter
{
	public class Filter
	{
		public const int WORDLE_LENGTH = 5;

		public const int NUMBER_OF_SUGGESTIONS = 6;

		public const string FILENAME = "words.txt";
		public const string JSON_FILENAME = "words.json";

		private List<string> _potentialWords;
		private Dictionary<string, int> _wordsDictionary;
		private List<string> _blackListWords;
		private List<string> _whiteListWords;
		List<char> _greenLetters;

		public Filter(string wordsJSON, List<string> words)
		{
			_potentialWords = words;
			_wordsDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(wordsJSON);
		}

		public int GetRemainingWords()
		{
			return _potentialWords.Count;
		}

		public void FilterRound(string guessWord, List<TileColor> guessWordColors)
		{
			//Stopwatch sw = new Stopwatch();
			//sw.Start();

			guessWord = guessWord.ToLower();

			// transform the guess word into a list
			List<char> guessWordLetters = guessWord.ToCharArray().ToList();

			_blackListWords = new List<string>();
			_blackListWords.Add(guessWord);

			_greenLetters = new List<char>();
			for (int i = 0; i < WORDLE_LENGTH; i++)
			{
				TileColor currentTileColor = guessWordColors[i];

				if (currentTileColor == TileColor.Green)
				{
					FilterWithGreenTile(i, guessWordLetters, guessWordColors);
				}
				else if (currentTileColor == TileColor.Yellow)
				{
					FilterWithYellowTile(i, guessWordLetters);
				}
				else
				{
					FilterWithGreyTile(i, guessWordLetters);
				}
			}

			_whiteListWords = _potentialWords.Except(_blackListWords).ToList();

			foreach (var word in _blackListWords)
			{
				if (_wordsDictionary.ContainsKey(word))
				{
					_wordsDictionary.Remove(word);
				}
			}

			int previousPotentialWordCount = _potentialWords.Count;
			_potentialWords = _whiteListWords;
			
			//sw.Stop();
			// Console.WriteLine($"Round took {sw.ElapsedMilliseconds}ms");
		}

		public List<string> GetInitialSuggestions()
		{
			Dictionary<string, int> sortedPotentialWordsDictionary = _wordsDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
			if (sortedPotentialWordsDictionary.Count > 100)
			{
				sortedPotentialWordsDictionary = sortedPotentialWordsDictionary.Reverse().Take(100).ToDictionary(x => x.Key, x => x.Value);
			}
			List<string> sortedPotentialWordsList = sortedPotentialWordsDictionary.Reverse().Select(x => x.Key).ToList();

			return BuildSuggestionList(sortedPotentialWordsList);
		}

		public List<string> GetRemainingSuggestions()
		{
			Dictionary<string, int> potentialWordsDictionary = new Dictionary<string, int>();

			foreach (var word in _potentialWords)
			{
				try
				{
					potentialWordsDictionary[word] = _wordsDictionary[word];
				}
				catch (KeyNotFoundException)
				{
					// some words with dashes and such are not shared between the two files, skip thems
					continue;
				}
			}

			Dictionary<string, int> sortedPotentialWordsDictionary = potentialWordsDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
			if (sortedPotentialWordsDictionary.Count > 100)
			{
				// FIXME: may need to reverse the orderby if it is not correctly ordered
				sortedPotentialWordsDictionary = sortedPotentialWordsDictionary.Reverse().Take(100).ToDictionary(x => x.Key, x => x.Value);
			}
			List<string> sortedPotentialWordsList = sortedPotentialWordsDictionary.Reverse().Select(x => x.Key).ToList();

			return BuildSuggestionList(sortedPotentialWordsList);
		}

		private void FilterWithGreenTile(int letterIndex, List<char> guessWordLetters, List<TileColor> guessWordTileColors)
		{
			char tileLetter = guessWordLetters[letterIndex];
			TileColor tileColor = guessWordTileColors[letterIndex];

			if (tileColor == TileColor.Green) 
			{
				// store the green letters to help decide grey letters
				if (_greenLetters.Contains(tileLetter) == false)
				{
					_greenLetters.Add(tileLetter);
				}
				// eliminate all words without that letter in that position
				foreach (var word in _potentialWords) {
					if (tileLetter != word[letterIndex])
					{
						_blackListWords.Add(word);
					}
				}
			}
		}

		private void FilterWithYellowTile(int letterIndex, List<char> guessWordLetters)
		{
			char tileLetter = guessWordLetters[letterIndex];

			foreach (var word in _potentialWords)
			{			
				// eliminate all words with that letter in that position
				if (tileLetter == word[letterIndex])
				{
					_blackListWords.Add(word);
				}
				else
				{
					// eliminate words that do not have that letter in it somewhere else
					bool letterFoundElsewhere = false;
					for (int j = 0; j < WORDLE_LENGTH; j++)
					{
						if (j == letterIndex)
						{
							continue;
						}
						if (tileLetter == word[j])
						{
							letterFoundElsewhere = true;
							break;
						}
					}
					if (letterFoundElsewhere == false)
					{
						_blackListWords.Add(word);
					}
				}
			}
		}

		private void FilterWithGreyTile(int letterIndex, List<char> guessWordLetters)
		{
			char tileLetter = guessWordLetters[letterIndex];

			// checks the case where a letter is green in one spot, but if elsewhere in guessed word, appears grey
			// be sure we do not eliminate words prematurely
			if (_greenLetters.Contains(tileLetter) == false)
			{
				// eliminate all words with that letter anywhere
				foreach (var word in _potentialWords)
				{
					foreach (var letter in word)
					{
						if (tileLetter == letter)
						{
							_blackListWords.Add(word);
							break;
						}
					}
				}
			}
		}

		private static List<string> BuildSuggestionList(List<string> sortedPotentialWordsList)
		{
			List<string> suggestions = new List<string>();

			if (sortedPotentialWordsList.Count <= NUMBER_OF_SUGGESTIONS)
			{
				foreach(var word in sortedPotentialWordsList)
				{
					suggestions.Add(word);
				}
				return suggestions;
			}

			while (suggestions.Count != NUMBER_OF_SUGGESTIONS)
			{
				Random rnd = new Random();
				string randomSuggestion = sortedPotentialWordsList[rnd.Next(sortedPotentialWordsList.Count)];
				if (suggestions.Contains(randomSuggestion))
				{
					continue;
				}
				if (IsLikelyWord(randomSuggestion) == false)
				{
					continue;
				}
				suggestions.Add(randomSuggestion.ToUpper());
			}
			return suggestions;
		}

		private static bool IsLikelyWord(string potentialWord)
		{
			if (potentialWord.Contains('-'))
			{
				return false;
			}
			if (potentialWord.Contains('.'))
			{
				return false;
			}
			if (potentialWord.EndsWith('s') && !potentialWord.EndsWith("ss"))
			{
				return false;
			}
			return true;
		}
	}
}
