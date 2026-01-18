using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Dungeon.Drawers.Terrain
{
	/// <summary>
	/// Drawer that puts the ground tiles in the rooms
	/// </summary>
	public class GroundDrawer : Drawer
	{
		private readonly Tilemap _groundMap;
		private readonly TileBase _tile;

		#region Drawer

		public GroundDrawer(DungeonResult level, Tilemap groundMap, TileBase tile) : base(level)
		{
			_groundMap = groundMap;
			_tile = tile;
		}

		/// <inheritdoc/>
		public override void Process(Room[] rooms)
		{
			for (int y = 0; y < Level.Height; y++)
			{
				for (int x = 0; x < Level.Width; x++)
				{
					if (Level.HasWall(x, y) || Level.HasObstacle(x, y))
						continue;

					Level.Add(x, y, Generation.Tile.Ground);
				}
			}
		}

		/// <inheritdoc/>
		public override void Draw(Room[] rooms)
		{
			int height = Level.Grid.GetLength(0);
			int width = Level.Grid.GetLength(1);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (!Level.HasGround(x, y))
						continue;

					if (Level.Has(x, y, Generation.Tile.CoveredGround))
						continue;

					_groundMap.SetTile(new Vector3Int(x, -y, 0), _tile);
				}
			}
		}

		/// <inheritdoc/>
		public override void Clear() => _groundMap.ClearAllTiles();

		#endregion
	}
}