using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace Dungeon.Drawers
{
    public class WallDrawer : Drawer
    {
        private readonly Tilemap wallMap;
        private readonly TileBase[] tiles;

        #region Drawer

        public WallDrawer(DungeonResult level, Tilemap wallMap, TileBase[] tiles) : base(level)
        {
            this.wallMap = wallMap;
            this.tiles = tiles;
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            foreach (var room in rooms)
            {
                // Left wall
                if (room.X == 0)
                {
                    for (int y = 0; y < room.Height; y++)
                        Level.Set(room.X, room.Y + y, Generation.Tile.WALL);
                }

                // Up wall
                if (room.Y == 0)
                {
                    for (int x = 0; x < room.Width; x++)
                        Level.Set(room.X + x, room.Y, Generation.Tile.WALL);
                }

                // Down wall
                for (int y = 0; y <= room.Height; y++)
                    Level.Set(room.X + room.Width, room.Y + y, Generation.Tile.WALL);

                // Right wall
                for (int x = 0; x < room.Width; x++)
                    Level.Set(room.X + x, room.Y + room.Height, Generation.Tile.WALL);

                room.Width--;
                room.Height--;
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
                    // If no wall, skip
                    if (!Level.HasWall(x, y))
                        continue;

                    var index = 0;

                    if (y > 0 && Level.HasWall(x, y - 1))
                        index += 0b0001; // TOP

                    if (y < height - 1 && Level.HasWall(x, y + 1))
                        index += 0b0100; // BOTTOM

                    if (x > 0 && Level.HasWall(x - 1, y))
                        index += 0b1000; // LEFT

                    if (x < width - 1 && Level.HasWall(x + 1, y))
                        index += 0b0010; // RIGHT

                    wallMap.SetTile(new Vector3Int(x, -y, 0), tiles[index]);
                }
            }
        }

        /// <inheritdoc/>
        public override void Clear() => wallMap.ClearAllTiles();

        #endregion
    }
}