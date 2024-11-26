using Dungeon.Generation;
using UnityEngine;

namespace Dungeon.Drawers
{
    public class EntranceExitDrawer : Drawer
    {
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
                        entrance.position = new Vector3(x, -y, 0);
                        player.position = new Vector3(x + 1, -y, 0);

                        if (player.TryGetComponent(out PlayerModule.PlayerEntity entity))
                            entity.FlipByMovement(GridModule.Movement.RIGHT);

                        hasPlacedEntrance = true;
                    }
                    else
                    {
                        exit.position = new Vector3(x, -y, 0);
                        return;
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

        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Transform entrance;

        [SerializeField]
        private Transform exit;

        [SerializeField]
        private Transform player;

        #endregion
    }
}