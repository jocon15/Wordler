namespace WordlerCore.Analytics
{
	public class AnalyticLetter
	{
		public char Letter { get; private set; }

		public bool Guessed { get; set; }

		public AnalyticLetter(char letter)
		{
			Letter = letter;
			Guessed = false;
		}
	}
}
