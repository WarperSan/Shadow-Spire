using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

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
        public override void Draw(Room[] rooms)
        {
            int height = Level.Grid.GetLength(0);
            int width = Level.Grid.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!Level.HasDoor(x, y))
                        continue;

                    wallMap.SetTile(new Vector3Int(x, -y, 0), doorTile);
                }
            }
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            System.Random random = Level.Random;

            List<(Room, Room)> processedLinks = new();

            foreach (var (room, adjacents) in Level.AdjacentRooms)
            {
                foreach (var adjacent in adjacents)
                {
                    // If already processed, skip
                    if (processedLinks.Contains((adjacent, room)))
                        continue;

                    Vector2Int point = new(1, 1);

                    if (adjacent.X >= room.X + room.Width)
                    {
                        var minY = Mathf.Max(room.Y, adjacent.Y);
                        var maxY = Mathf.Min(room.Y + room.Height - 1, adjacent.Y + adjacent.Height - 1);

                        if (maxY - minY >= 3)
                        {
                            minY++;
                            maxY--;
                        }

                        point.x = adjacent.X - 1;
                        point.y = random.Next(minY, maxY);
                    }
                    else if (adjacent.Y >= room.Y + room.Height)
                    {
                        var minX = Mathf.Max(room.X, adjacent.X);
                        var maxX = Mathf.Min(room.X + room.Width - 1, adjacent.X + adjacent.Width - 1);

                        if (maxX - minX >= 3)
                        {
                            minX++;
                            maxX--;
                        }


                        point.x = random.Next(minX, maxX);
                        point.y = adjacent.Y - 1;
                    }
                    else
                        continue;

                    Level.Add(point.x, point.y, Generation.Tile.DOOR_CLOSED);
                    processedLinks.Add((room, adjacent));
                }
            }
        }

        /// <inheritdoc/>
        public override void Clear() => wallMap.ClearAllTiles();

        #endregion
    }
}