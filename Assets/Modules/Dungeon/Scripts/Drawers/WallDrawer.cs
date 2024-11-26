using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.Drawers
{
    public class WallDrawer : Drawer
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Tilemap wallMap;

        // TOP    = 0b0001 = 1
        // RIGHT  = 0b0010 = 2
        // BOTTOM = 0b0100 = 4
        // LEFT   = 0b1000 = 8
        [SerializeField]
        private TileBase[] tiles;

        #endregion

        /// <inheritdoc/>
        public override bool[,] Process(Room[] rooms)
        {
            bool[,] grid = CreateEmpty(rooms);

            foreach (var room in rooms)
            {
                //UnityEngine.Debug.Log(room.X + ";" + room.Y + " (" + room.Width + "x" + room.Height + ")");

                // for (int x = 0; x <= room.Width; x++)
                // {
                //     grid[room.Y, room.X + x] = true; // Top border
                //     grid[room.Y + room.Height, room.X + x] = true; // Bottom border
                // }

                // for (int y = 0; y <= room.Height; y++)
                // {
                //     grid[room.Y + y, room.X] = true; // Left border
                //     grid[room.Y + y, room.X + room.Width] = true; // Right border
                // }

                for (int x = 0; x <= room.Width + 1; x++)
                {
                    grid[room.Y, room.X + x] = true; // Top border
                    grid[room.Y + room.Height + 1, room.X + x] = true; // Bottom border
                }

                for (int y = 0; y <= room.Height + 1; y++)
                {
                    grid[room.Y + y, room.X] = true; // Left border
                    grid[room.Y + y, room.X + room.Width + 1] = true; // Right border
                }
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
    }
}