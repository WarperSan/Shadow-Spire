using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        public override bool[,] Process(Room[] rooms)
        {
            bool[,] grid = CreateEmpty(rooms);

            foreach (var room in rooms)
            {
                if (room.X == 0)
                {
                    for (int y = 0; y < room.Height; y++)
                        grid[room.Y + y, room.X] = true;
                }

                if (room.Y == 0)
                {
                    for (int x = 0; x < room.Width; x++)
                        grid[room.Y, room.X + x] = true;
                }

                for (int y = 0; y <= room.Height; y++)
                    grid[room.Y + y, room.X + room.Width] = true;

                for (int x = 0; x < room.Width; x++)
                    grid[room.Y + room.Height, room.X + x] = true;

                room.Width--;
                room.Height--;
            }

            return grid;
        }

        /// <inheritdoc/>
        public override void Draw(bool[,] grid, Room[] rooms)
        {
            int height = grid.GetLength(0);
            int width = grid.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // If no wall, skip
                    if (!grid[y, x])
                        continue;

                    var index = 0;

                    if (y > 0 && grid[y - 1, x])
                        index += 0b0001; // TOP

                    if (y < height - 1 && grid[y + 1, x])
                        index += 0b0100; // BOTTOM

                    if (x > 0 && grid[y, x - 1])
                        index += 0b1000; // LEFT

                    if (x < width - 1 && grid[y, x + 1])
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