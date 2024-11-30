using System;
using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.Drawers
{
    public class DoorDrawer : Drawer
    {
        private readonly Tilemap wallMap;
        private readonly TileBase doorTile;

        #region Drawer

        public DoorDrawer(DungeonResult level, Tilemap wallMap, TileBase doorTile) : base(level)
        {
            this.wallMap = wallMap;
            this.doorTile = doorTile;
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

                    wallMap.SetTile(new Vector3Int(x, -y, 0), doorTile);
                }
            }
        }

        /// <inheritdoc/>
        public override bool[,] Process(Room[] rooms)
        {
            System.Random random = Level.Random;

            bool[,] grid = CreateEmpty(rooms);

            foreach (var (room, adjacents) in Level.AdjacentRooms)
            {
                foreach (var adjacent in adjacents)
                {
                    if (room.X < adjacent.X && room.IsBeside(adjacent))
                    {
                        var rdmY = random.Next(
                            Mathf.Max(room.Y, adjacent.Y) + 1, // Start from the point at the most down
                            Mathf.Min(adjacent.Y + adjacent.Height, room.Y + room.Height) - 1 // Start from the point at the most up
                        );
                        grid[rdmY, room.X + room.Width] = true;
                    }
                    else if (room.Y < adjacent.Y && room.IsUnder(adjacent))
                    {
                        var rdmX = random.Next(
                            Mathf.Max(room.X, adjacent.X) + 1, // Start from the point at the most right
                            Mathf.Min(room.X + room.Width, adjacent.X + adjacent.Width) - 1 // Start from the point at the most left
                        );

                        grid[room.Y + room.Height, rdmX] = true;
                    }
                }
            }

            return grid;
        }

        /// <inheritdoc/>
        public override void Clear() => wallMap.ClearAllTiles();

        #endregion
    }
}