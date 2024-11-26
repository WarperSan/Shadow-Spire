using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.Drawers
{
    public class GroundDrawer : Drawer
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Tilemap groundMap;

        public TileBase tile;

        #endregion

        /// <inheritdoc/>
        public override bool[,] Process(Room[] rooms)
        {
            bool[,] entranceExitGrid = Dungeon.Instance.Level.EntranceExitGrid;
            bool[,] grid = CreateEmpty(rooms);

            foreach (var item in rooms)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    for (int x = 0; x < item.Width; x++)
                    {
                        if (entranceExitGrid[item.Y + y + 1, item.X + x + 1])
                            continue;

                        grid[item.Y + y + 1, item.X + x + 1] = true;
                    }
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

                    groundMap.SetTile(new Vector3Int(x, -y, 0), tile);
                }
            }
        }
    }
}