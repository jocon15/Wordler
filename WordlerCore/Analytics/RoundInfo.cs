namespace WordlerCore.Analytics
{
	public class RoundInfo
	{
		public int RoundNumber { get; private set; }

		public string GuessWord { get; private set; }

		public int WordsEliminated { get; private set; }

		public int WordsRemaining { get; private set; }

		public long EliminationTimeMS { get; private set; }

		public RoundInfo(int roundNumber, string guessWord, int wordsEliminated, int wordsRemaining, long eliminationTimeMS)
		{
			RoundNumber = roundNumber;
			GuessWord = guessWord;
			WordsEliminated = wordsEliminated;
			WordsRemaining = wordsRemaining;
			EliminationTimeMS = eliminationTimeMS;
		}
	}
}
