using Dungeon.Generation;
using Entities;
using UnityEngine;

namespace Dungeon.Drawers
{
    public class EntranceExitDrawer : Drawer
    {
        private readonly EntranceEntity entrance;
        private readonly ExitEntity exit;
        private readonly PlayerEntity player;

        private void PlaceEntrance(int x, int y)
        {
            entrance.transform.position = new Vector3(x, -y, 0);

            bool isLeft = Level.WallGrid[y, x + 1];

            Movement direction = isLeft ? Movement.LEFT : Movement.RIGHT;

            if (isLeft)
                player.transform.position = new Vector3(x - 1, -y, 0);
            else
                player.transform.position = new Vector3(x + 1, -y, 0);

            player.FlipByMovement(direction);
            entrance.FlipByMovement(direction);
        }

        private void PlaceExit(int x, int y)
        {
            bool isLeft = Level.WallGrid[y, x + 1];
            Movement direction = isLeft ? Movement.LEFT : Movement.RIGHT;

            exit.transform.position = new Vector3(x, -y, 0);
            exit.FlipByMovement(direction);
        }

        #region Drawer

        public EntranceExitDrawer(DungeonResult level, EntranceEntity entrance, ExitEntity exit, PlayerEntity player) : base(level)
        {
            this.entrance = entrance;
            this.exit = exit;
            this.player = player;
        }

        /// <inheritdoc/>
        public override void Draw(bool[,] grid, Room[] rooms)
        {
            bool hasPlacedEntrance = false;

            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (!grid[y, x])
                        continue;

                    if (!hasPlacedEntrance)
                    {
                        PlaceEntrance(x, y);
                        hasPlacedEntrance = true;
                    }
                    else
                    {
                        PlaceExit(x, y);
                        return; // Exit because last
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override bool[,] Process(Room[] rooms)
        {
            bool[,] grid = CreateEmpty(rooms);

            // Find smallest
            Room smallest = rooms[0];
            int smallestSize = smallest.Width * smallest.Height;

            for (int i = 1; i < rooms.Length; i++)
            {
                int size = rooms[i].Width * rooms[i].Height;

                if (smallestSize > size)
                {
                    smallest = rooms[i];
                    smallestSize = size;
                }
            }

            // Find biggest
            Room furthest = rooms[0];
            float distance = Vector2.Distance(new Vector2(smallest.X, smallest.Y), new Vector2(furthest.X, furthest.Y));

            for (int i = 1; i < rooms.Length; i++)
            {
                float newDistance = Vector2.Distance(new Vector2(smallest.X, smallest.Y), new Vector2(rooms[i].X, rooms[i].Y));

                if (newDistance > distance)
                {
                    distance = newDistance;
                    furthest = rooms[i];
                }
            }

            grid[smallest.Y + smallest.Height, smallest.X + 1] = true;
            grid[furthest.Y + 1, furthest.X + furthest.Width] = true;

            return grid;
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            entrance.transform.position = Vector3.zero;
            exit.transform.position = Vector3.zero;
            player.transform.position = Vector3.zero;
        }

        #endregion
    }
}