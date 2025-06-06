using WordlerCore.Analytics;
using WordlerCore.Tile;

namespace WordlerCore.Game
{
	public class BoardTile
	{
		public readonly char Letter;

		public readonly TileColor Color;

		public BoardTile(char letter, TileColor color)
		{
			Letter = letter;
			Color = color;
		}
	}

	public class BoardRow
	{
		public readonly string Word;

		public readonly List<BoardTile> Tiles;

		public BoardRow(string word, List<BoardTile> tiles)
		{
			Word = word;
			Tiles = tiles;
		}
	}

	public class Game
	{
		public int Round { get; private set; } = 1;

		public List<AnalyticLetter> Vowels { get; private set; } = new List<AnalyticLetter>();

		public List<AnalyticLetter> CommonConsonants { get; private set; } = new List<AnalyticLetter>();

		private List<BoardRow> _rows = new List<BoardRow>();

		private Filter.Filter _filter;
		
		public Game() 
		{
			_filter = new Filter.Filter(GameData.WORDS_JSON);

			Vowels = new List<AnalyticLetter>() { new('A'), new('E'), new('I'), new('O'), new('U'), new('Y') };
			CommonConsonants = new List<AnalyticLetter>() { new('R'), new('S'), new('T') };
		}

		public void ResetGame()
		{
			Round = 1;
			_filter = new Filter.Filter(GameData.WORDS_JSON);
			ClearRows();
		}

		public List<BoardRow> GetRows()
		{
			return _rows;
		}

		public List<string> GetSuggestions()
		{
			if (Round == 1)
			{
				return _filter.GetInitialSuggestions();	
			}
			else
			{
				return _filter.GetRemainingSuggestions();
			}
		}

		public void SubmitWord(string guessWord, List<TileColor> guessWordColors)
		{
			UpdateAnalyticsLists(guessWord);

			_filter.FilterRound(guessWord, guessWordColors);

			AddRow(guessWord, guessWordColors);

			Round++;
		}

		public int GetRemainingWords()
		{
			return _filter.GetRemainingWords();
		}

		private void UpdateAnalyticsLists(string guessWord)
		{
			foreach(char letter in guessWord.ToUpper())
			{
				foreach(AnalyticLetter aLetter in Vowels)
				{
					if (letter == aLetter.Letter){
						aLetter.Guessed = true;
					}
				}
				foreach (AnalyticLetter aLetter in CommonConsonants)
				{
					if (letter == aLetter.Letter)
					{
						aLetter.Guessed = true;
					}
				}
			}
		}

		private void AddRow(string word, List<TileColor> colors)
		{
			List<BoardTile> tiles = new List<BoardTile>();
			BoardTile tile;
			for (int i = 0; i < word.Length; i++)
			{
				tile = new BoardTile(word[i], colors[i]);
				tiles.Add(tile);
			}

			BoardRow row = new BoardRow(word, tiles);
			_rows.Add(row);
		}

		private void ClearRows()
		{
			_rows = new List<BoardRow>();
		}
	}
}
