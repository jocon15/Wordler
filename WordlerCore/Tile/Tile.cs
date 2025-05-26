namespace WordlerCore.Tile
{

	public enum TileColor
	{
		Grey = 0,
		Yellow = 1,
		Green = 2,
	}

	public class TileColorAlloy
	{
		public int TileIndex;
		public TileColor Color;

		public TileColorAlloy(int tileIndex, TileColor color)
		{
			TileIndex = tileIndex;
			Color = color;
		}
	}

	public class Tile
	{
	}
}
