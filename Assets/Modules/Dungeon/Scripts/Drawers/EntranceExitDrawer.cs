using Dungeon.Generation;
using Entities;
using UnityEngine;
using UtilsModule;

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

            bool isLeft = Level.HasWall(x + 1, y);

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
            bool isLeft = Level.HasWall(x + 1, y);
            Movement direction = isLeft ? Movement.LEFT : Movement.RIGHT;

            exit.transform.position = new Vector3(x, -y, 0);
            exit.FlipByMovement(direction);
        }

        public static Room FindEntrance(Room[] rooms)
        {
            // Find smallest room
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

            return smallest;
        }

        public static Room FindExit(Room[] rooms, Room entrance)
        {
            Room exit = null;
            var entrancePosition = new Vector2Int(entrance.X, entrance.Y + (entrance.Height - 1) / 2);

            // Find furthest from entrance
            float distance = float.MinValue;

            for (int i = 0; i < rooms.Length; i++)
            {
                var newExit = new Vector2Int(rooms[i].X + (rooms[i].Width - 1) / 2, rooms[i].Y);
                float newDistance = Vector2.Distance(entrancePosition, newExit);

                if (newDistance > distance)
                {
                    distance = newDistance;
                    exit = rooms[i];
                }
            }

            return exit;
        }

        #region Drawer

        public EntranceExitDrawer(DungeonResult level, EntranceEntity entrance, ExitEntity exit, PlayerEntity player) : base(level)
        {
            this.entrance = entrance;
            this.exit = exit;
            this.player = player;
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
                    if (Level.Has(x, y, Tile.ENTRANCE))
                        PlaceEntrance(x, y);
                    else if (Level.Has(x, y, Tile.EXIT))
                        PlaceExit(x, y);
                }
            }
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            ProcessEntrance(Level.Entrance);
            ProcessExit(Level.Exit);
        }

        private void ProcessEntrance(Room entrance)
        {
            var placeLeft = Level.Random.Next(0, 2) == 0;

            // Try placing on left
            if (placeLeft)
            {
                for (int y = entrance.Y; y < entrance.Y + entrance.Height; y++)
                {
                    if (Level.HasWall(entrance.X - 1, y))
                    {
                        Level.Add(entrance.X, y, Tile.ENTRANCE);
                        return;
                    }
                }
            }
            // Try placing on right
            else
            {
                for (int y = entrance.Y; y < entrance.Y + entrance.Height; y++)
                {
                    if (!Level.HasDoor(entrance.X + entrance.Width, y))
                    {
                        Level.Add(entrance.X + entrance.Width - 1, y, Tile.ENTRANCE);
                        return;
                    }
                }
            }
        }

        private void ProcessExit(Room exit)
        {
            var placeLeft = Level.Random.Next(0, 2) == 0;

            // Try placing on left
            if (placeLeft)
            {
                for (int y = exit.Y; y < exit.Y + exit.Height; y++)
                {
                    if (Level.HasWall(exit.X - 1, y))
                    {
                        Level.Add(exit.X, y, Tile.EXIT);
                        return;
                    }
                }
            }
            // Try placing on right
            else
            {
                for (int y = exit.Y; y < exit.Y + exit.Height; y++)
                {
                    if (!Level.HasDoor(exit.X + exit.Width, y))
                    {
                        Level.Add(exit.X + exit.Width - 1, y, Tile.EXIT);
                        return;
                    }
                }
            }
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