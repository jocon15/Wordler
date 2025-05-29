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

		private List<BoardRow> _rows = new List<BoardRow>();

		private Filter.Filter _filter;
		
		public Game() 
		{
			_filter = new Filter.Filter(GameData.WORDS_JSON, GameData.WORDS);
		}

		public void ResetGame()
		{
			Round = 1;
			_filter = new Filter.Filter(GameData.WORDS_JSON, GameData.WORDS);
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
			_filter.FilterRound(guessWord, guessWordColors);

			AddRow(guessWord, guessWordColors);

			Round++;
		}

		public int GetRemainingWords()
		{
			return _filter.GetRemainingWords();
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
