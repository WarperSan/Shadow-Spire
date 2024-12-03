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

                    Vector2Int point = new(room.X + room.Width, room.Y + room.Height);

                    if (adjacent.X >= room.X + room.Width)
                    {
                        var minY = Mathf.Max(room.Y, adjacent.Y);
                        var maxY = Mathf.Min(room.Y + room.Height, adjacent.Y + adjacent.Height);

                        if (maxY - minY >= 3)
                        {
                            minY++;
                            maxY--;
                        }

                        point.y = random.Next(minY, maxY);
                    }
                    else
                    {
                        var minX = Mathf.Max(room.X, adjacent.X);
                        var maxX = Mathf.Min(room.X + room.Width, adjacent.X + adjacent.Width);

                        if (maxX - minX >= 3)
                        {
                            minX++;
                            maxX--;
                        }

                        point.x = random.Next(minX, maxX);
                    }

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