using WordlerCore.Filter;
using WordlerCore.Tile;

namespace WordlerCore.Game
{
	public class Game
	{
		public readonly int Round = 0;

		private Filter.Filter _filter;

		
		
		public Game() 
		{
			_filter = new Filter.Filter(GameData.WORDS_JSON, GameData.WORDS);
		}

		public List<string> GetSuggestions()
		{
			if (Round == 0)
			{
				return _filter.GetRemainingSuggestions();
			}
			else
			{
				return _filter.GetInitialSuggestions();
			}
		}

		public void SubmitWord(string guessWord, List<TileColor> guessWordColors)
		{
			_filter.FilterRound(guessWord, guessWordColors);
		}
	}
}
