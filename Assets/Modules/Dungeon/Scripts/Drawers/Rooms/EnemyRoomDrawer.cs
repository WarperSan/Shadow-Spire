using System;
using Dungeon.Generation;
using Enemies;
using GridEntities.Entities;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
    public class EnemyRoomDrawer : RoomDrawer
    {
        private readonly EnemySO[] EnemyPool;
        private readonly Transform SpawnedParent;
        private readonly GameObject EnemyPrefab;

        #region RoomDrawer

        /// <inheritdoc/>
        public override RoomType Type => RoomType.ENEMY;

        /// <inheritdoc/>
        protected override void OnDraw(Room room)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    // If not an enemy, skip
                    if (!Level.Has(x, y, Tile.ENEMY))
                        continue;

                    GameObject enemy = Object.Instantiate(EnemyPrefab, SpawnedParent);
                    enemy.transform.position = new Vector3(x, -y, 0);

                    if (enemy.TryGetComponent(out EnemyEntity entity))
                        entity.SetData(EnemyPool[Level.Random.Next(0, EnemyPool.Length)], Level.Index);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnProcess(Room room)
        {
            Func<int, int, bool> validEnemyPredicate = new((x, y) =>
            {
                // If there is a door to the left, skip
                if (Level.HasDoor(x - 1, y))
                    return false;

                // If there is a door to the right, skip
                if (Level.HasDoor(x + 1, y))
                    return false;

                // If there is a door to the top, skip
                if (Level.HasDoor(x, y - 1))
                    return false;

                // If there is a door to the bottom, skip
                if (Level.HasDoor(x, y + 1))
                    return false;

                return true;
            });

            // Find valid positions
            System.Collections.Generic.List<Vector2Int> positions = GetValidPositions(room, validEnemyPredicate);

            // If no valid position, skip
            if (positions.Count == 0)
                return;

            int count = Mathf.CeilToInt(room.Width * room.Height / 8f * 0.6f);
            count = Mathf.Min(count, positions.Count);

            for (int i = 0; i < count; i++)
            {
                int rdmIndex = Level.Random.Next(0, positions.Count);
                Vector2Int pos = positions[rdmIndex];

                Level.Add(pos.x, pos.y, Tile.ENEMY);

                positions.RemoveAt(rdmIndex);
            }
        }

        #endregion

        #region Drawer

        public EnemyRoomDrawer(DungeonResult level, GameObject enemyPrefab, EnemySO[] enemyPool, Transform spawnedParent) : base(level)
        {
            EnemyPool = enemyPool;
            EnemyPrefab = enemyPrefab;

            GameObject parent = new()
            {
                name = "Enemies"
            };
            parent.transform.parent = spawnedParent;
            SpawnedParent = parent.transform;
        }

        /// <inheritdoc/>
        public override void Clear() => Object.Destroy(SpawnedParent.gameObject);

        #endregion
    }
}