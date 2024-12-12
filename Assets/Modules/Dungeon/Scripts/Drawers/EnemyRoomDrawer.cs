using System.Collections.Generic;
using Dungeon.Generation;
using Enemies;
using UnityEngine;
using UtilsModule;

namespace Dungeon.Drawers
{
    public class EnemyRoomDrawer : Drawer
    {
        private readonly EnemySO[] EnemyPool;
        private readonly List<GameObject> enemiesSpawned;

        #region Drawer

        public EnemyRoomDrawer(DungeonResult level, EnemySO[] enemyPool) : base(level)
        {
            EnemyPool = enemyPool;
            enemiesSpawned = new();
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            foreach (var enemy in enemiesSpawned)
            {
                // If already despawned, skip
                if (enemy == null)
                    continue;

                Object.Destroy(enemy);
            }

            enemiesSpawned.Clear();
        }

        public override void Draw(Room[] rooms)
        {
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            foreach (var room in rooms)
            {
                // If not an enemy room, skip
                if (room.Type != RoomType.ENEMY)
                    continue;

                // Find valid positions
                var positions = new List<Vector2Int>();

                for (int y = room.Y; y < room.Y + room.Height; y++)
                {
                    for (int x = room.X; x < room.X + room.Width; x++)
                    {
                        // If the tile is blocked, skip
                        if (Level.IsBlocked(x, y))
                            continue;

                        // If there is a door to the left, skip
                        if (Level.HasDoor(x - 1, y))
                            continue;

                        // If there is a door to the right, skip
                        if (Level.HasDoor(x + 1, y))
                            continue;

                        // If there is a door to the top, skip
                        if (Level.HasDoor(x, y - 1))
                            continue;

                        // If there is a door to the bottom, skip
                        if (Level.HasDoor(x, y + 1))
                            continue;

                        positions.Add(new(x, y));
                    }
                }

                // If no valid position, skip
                if (positions.Count == 0)
                    continue;

                int totalValue = Mathf.CeilToInt(room.Width * room.Height * 0.5f * (Level.Index + 1) * 0.15f);

                for (int i = 0; i < positions.Count; i++)
                {
                    totalValue -= 2;

                    if (totalValue <= 0)
                        break;

                    
                                        
                }


            }
        }

        #endregion

        #region Enemy


        #endregion
    }
}