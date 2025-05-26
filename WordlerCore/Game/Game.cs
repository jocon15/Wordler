using WordlerCore.Tile;

namespace WordlerCore.Game
{
	public class Game
	{
		public int Round { get; private set; } = 1;

		private Filter.Filter _filter;

		
		
		public Game() 
		{
			_filter = new Filter.Filter(GameData.WORDS_JSON, GameData.WORDS);
		}

		public void ResetGame()
		{
			_filter = new Filter.Filter(GameData.WORDS_JSON, GameData.WORDS);
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

			Round++;
		}

		public int GetRemainingWords()
		{
			return _filter.GetRemainingWords();
		}
	}
}
