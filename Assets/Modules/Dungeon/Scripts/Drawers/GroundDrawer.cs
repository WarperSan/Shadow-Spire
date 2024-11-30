using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace Dungeon.Drawers
{
    public class GroundDrawer : Drawer
    {
        private readonly Tilemap groundMap;
        private readonly TileBase tile;

        #region Drawer

        public GroundDrawer(DungeonResult level, Tilemap groundMap, TileBase tile) : base(level)
        {
            this.groundMap = groundMap;
            this.tile = tile;
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            foreach (var room in rooms)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    for (int x = 0; x < room.Width; x++)
                    {
                        if (Level.HasObstacle(room.X + x + 1, room.Y + y + 1))
                            continue;

                        Level.Add(room.X + x + 1, room.Y + y + 1, Generation.Tile.GROUND);
                    }
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

                    groundMap.SetTile(new Vector3Int(x, -y, 0), tile);
                }
            }
        }

        /// <inheritdoc/>
        public override void Clear() => groundMap.ClearAllTiles();

        #endregion
    }
}