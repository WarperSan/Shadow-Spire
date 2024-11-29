using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using Managers;

namespace Dungeon.Drawers
{
    public class DoorDrawer : Drawer
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Tilemap wallMap;

        [SerializeField]
        private TileBase openDoorTile;

        [SerializeField]
        private TileBase closeDoorTile;

        #endregion

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

                    wallMap.SetTile(new Vector3Int(x, -y, 0), closeDoorTile);
                }
            }
        }

        /// <inheritdoc/>
        public override bool[,] Process(Room[] rooms)
        {
            bool[,] groundGrid = DungeonManager.Instance.Level.GroundGrid;
            System.Random random = DungeonManager.Instance.Level.Random;

            bool[,] grid = CreateEmpty(rooms);

            int height = grid.GetLength(0);
            int width = grid.GetLength(1);

            foreach (var item in rooms)
            {
                List<(int x, int y)> positions = new();

                int rightX = item.X + item.Width;
                int downY = item.Y + item.Height;

                if (rightX < width - 2)
                {
                    int x = rightX + 1;
                    int y = item.Y + 1;

                    for (int i = 1; i < item.Height - 1; i++)
                    {
                        if (!groundGrid[y + i, x + 1])
                            continue;

                        if (!groundGrid[y + i, x - 1])
                            continue;

                        positions.Add((x, y + i));
                    }
                }

                if (downY < height - 2)
                {
                    int x = item.X + 1;
                    int y = downY + 1;

                    for (int i = 1; i < item.Width - 1; i++)
                    {
                        if (!groundGrid[y + 1, x + i])
                            continue;

                        if (!groundGrid[y - 1, x + i])
                            continue;

                        positions.Add((x + i, y));
                    }
                }

                if (positions.Count == 0)
                    continue;

                for (int i = positions.Count / 5; i >= 0; i--)
                {
                    var index = random.Next(0, positions.Count);
                    var pos = positions[index];
                    positions.RemoveAt(index);

                    grid[pos.y, pos.x] = true;
                }
            }

            return grid;
        }

        /// <inheritdoc/>
        public override void Clear() => wallMap.ClearAllTiles();
    }
}