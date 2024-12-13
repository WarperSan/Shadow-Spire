using System.Collections.Generic;
using Dungeon.Generation;
using Enemies;
using Entities;
using UnityEngine;
using UtilsModule;

namespace Dungeon.Drawers.Rooms
{
    public class EnemyRoomDrawer : Drawer
    {
        private readonly EnemySO[] EnemyPool;
        private GameObject EnemyPrefab;
        private readonly List<GameObject> enemiesSpawned;

        #region Drawer

        public EnemyRoomDrawer(DungeonResult level, GameObject enemyPrefab, EnemySO[] enemyPool) : base(level)
        {
            EnemyPool = enemyPool;
            EnemyPrefab = enemyPrefab;
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

        /// <inheritdoc/>
        public override void Draw(Room[] rooms)
        {
            foreach (var room in rooms)
            {
                // If not an enemy room, skip
                if (room.Type != RoomType.ENEMY)
                    continue;

                for (int y = room.Y; y < room.Y + room.Height; y++)
                {
                    for (int x = room.X; x < room.X + room.Width; x++)
                    {
                        // If not an enemy, skip
                        if (!Level.Has(x, y, Tile.ENEMY))
                            continue;

                        var enemy = Object.Instantiate(EnemyPrefab);
                        enemy.transform.position = new Vector3(x, -y, 0);

                        if (enemy.TryGetComponent(out EnemyEntity entity))
                            entity.EnemyData = EnemyPool[Level.Random.Next(0, EnemyPool.Length)];

                        enemiesSpawned.Add(enemy);
                    }
                }
            }
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

                int count = Mathf.CeilToInt(room.Width * room.Height / 8f * 0.6f);
                count = Mathf.Min(count, positions.Count);

                for (int i = 0; i < count; i++)
                {
                    int rdmIndex = Level.Random.Next(0, positions.Count);
                    var pos = positions[rdmIndex];

                    Level.Add(pos.x, pos.y, Tile.ENEMY);

                    positions.RemoveAt(rdmIndex);
                }
            }
        }

        #endregion
    }
}