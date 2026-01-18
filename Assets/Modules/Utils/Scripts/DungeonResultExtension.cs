using Dungeon.Generation;

namespace Utils
{
	public static class DungeonResultExtension
	{
		/// <summary>
		/// Sets the given tile at the given position
		/// </summary>
		public static void Set(
			this DungeonResult level,
			int                x,
			int                y,
			Tile               tile
		) => level.Grid[y, x] = tile;

		/// <summary>
		/// Gets the tile at the given position
		/// </summary>
		public static Tile Get(this DungeonResult level, int x, int y) => level.Grid[y, x];

		/// <summary>
		/// Adds the given tile at the given position
		/// </summary>
		public static void Add(
			this DungeonResult level,
			int                x,
			int                y,
			Tile               tile
		) => level.Set(x, y, level.Get(x, y) | tile);

		/// <summary>
		/// Removes the given tile at the given position
		/// </summary>
		public static void Remove(
			this DungeonResult level,
			int                x,
			int                y,
			Tile               tile
		) => level.Set(x, y, level.Get(x, y) & ~tile);

		/// <summary>
		/// Checks if the tile at the given position has the given tile
		/// </summary>
		public static bool Has(
			this DungeonResult level,
			int                x,
			int                y,
			Tile               tile
		) => (level.Get(x, y) & tile) != 0;

		#region Predicate

		/// <summary>
		/// Checks if the given tile has a wall
		/// </summary>
		public static bool HasWall(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.Wall);

		/// <summary>
		/// Checks if the given tile has ground
		/// </summary>
		public static bool HasGround(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.Ground | Tile.CoveredGround);

		/// <summary>
		/// Checks if the given tile has an obstacle
		/// </summary>
		public static bool HasObstacle(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.Entrance | Tile.Exit);

		/// <summary>
		/// Checks if the given tile is blocked
		/// </summary>
		public static bool IsBlocked(this DungeonResult level, int x, int y) => level.HasWall(x, y) || level.HasObstacle(x, y);

		/// <summary>
		/// Checks if the given tile has a door
		/// </summary>
		public static bool HasDoor(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.DoorClosed | Tile.DoorOpened);

		#endregion
	}
}