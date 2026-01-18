using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Dungeon.Drawers.Terrain
{
	/// <summary>
	/// Drawer that puts the walls tiles in the rooms
	/// </summary>
	public class WallDrawer : Drawer
	{
		private readonly Tilemap _wallMap;
		private readonly TileBase[] _tiles;

		#region Drawer

		public WallDrawer(DungeonResult level, Tilemap wallMap, TileBase[] tiles) : base(level)
		{
			_wallMap = wallMap;
			_tiles = tiles;
		}

		/// <inheritdoc/>
		public override void Process(Room[] rooms)
		{
			foreach (Room room in rooms)
			{
				room.X++; // Move room left by 1
				room.Y++; // Move room down by 1
				room.Width--;
				room.Height--;

				// Bottom wall
				for (int y = 0; y <= room.Height; y++)
					Level.Set(room.X + room.Width, room.Y + y, Generation.Tile.Wall);

				// Right wall
				for (int x = 0; x <= room.Width; x++)
					Level.Set(room.X + x, room.Y + room.Height, Generation.Tile.Wall);
			}

			// Left wall
			for (int y = 0; y < Level.Height; y++)
				Level.Set(0, y, Generation.Tile.Wall);

			// Up wall
			for (int x = 0; x < Level.Width; x++)
				Level.Set(x, 0, Generation.Tile.Wall);
		}

		/// <inheritdoc/>
		public override void Draw(Room[] rooms)
		{
			for (int y = 0; y < Level.Height; y++)
			{
				for (int x = 0; x < Level.Width; x++)
				{
					// If no wall, skip
					if (!Level.HasWall(x, y))
						continue;

					int index = 0;

					if (y > 0 && Level.HasWall(x, y - 1))
						index += 0b0001; // TOP

					if (y < Level.Height - 1 && Level.HasWall(x, y + 1))
						index += 0b0100; // BOTTOM

					if (x > 0 && Level.HasWall(x - 1, y))
						index += 0b1000; // LEFT

					if (x < Level.Width - 1 && Level.HasWall(x + 1, y))
						index += 0b0010; // RIGHT

					_wallMap.SetTile(new Vector3Int(x, -y, 0), _tiles[index]);
				}
			}
		}

		/// <inheritdoc/>
		public override void Clear() => _wallMap.ClearAllTiles();

		#endregion
	}
}