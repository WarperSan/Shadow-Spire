using System.Collections;
using System.Collections.Generic;
using Enemies;
using Entities.Interfaces;
using Managers;
using UnityEngine;
using UtilsModule;

namespace Entities
{
    public class EnemyEntity : GridEntity, ITurnable, IMovable, IEventable
    {
        #region Data

        public EnemyInstance Instance;
        private EnemySO _data;

        private int waitTurns;
        private int movesPerTurn;
        private int turnsRemaining;

        public void SetData(EnemySO data, int level)
        {
            Instance = new EnemyInstance(data, level);

            spriteRenderer.sprite = data.OverworldSprite;
            spriteRenderer.color = data.BaseType.GetColor();

            turnsRemaining = waitTurns = data.MovementSpeed switch
            {
                EnemyMovementSpeed.VERY_SLOW => 2,
                EnemyMovementSpeed.SLOW => 1,
                _ => 0
            };

            movesPerTurn = data.MovementSpeed switch
            {
                EnemyMovementSpeed.FAST => 2,
                EnemyMovementSpeed.VERY_FAST => 3,
                _ => 1
            };

            _data = data;
        }

        #endregion

        #region ITurnable

        private int[] path;
        private Movement[] movements;
        private int currentIndex = -1;

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            turnsRemaining--;

            if (turnsRemaining >= 0)
                yield break;

            turnsRemaining = waitTurns;

            if (_data.Pathing == EnemyPathing.DIRECT || path == null || currentIndex >= path.Length)
                UpdatePath();

            // If no path found or on the same tile
            if (movements == null || movements.Length == 0)
                yield break;

            if (movesPerTurn + currentIndex >= movements.Length)
            {
                yield return movements[currentIndex..];
                currentIndex = path.Length;
                yield break;
            }

            yield return movements[currentIndex..(movesPerTurn + currentIndex)];

            currentIndex += movesPerTurn;
        }

        private void UpdatePath()
        {
            Vector2Int target = Position;

            if (_data.Pathing == EnemyPathing.DIRECT)
                target = GameManager.Instance.player.Position;
            else if (_data.Pathing == EnemyPathing.RANDOM)
                target = GetRandomPosition();

            path = PathFindingManager.FindPath(this, target);
            movements = PathFindingManager.GetDirections(path);
            currentIndex = 0;
        }

        private Vector2Int GetRandomPosition()
        {
            Dungeon.Generation.DungeonResult level = GameManager.Instance.Level;
            Dungeon.Generation.Room rdmRoom = level.Rooms[level.Random.Next(0, level.Rooms.Length)];

            List<Vector2Int> positions = new();

            for (int y = rdmRoom.Y; y < rdmRoom.Y + rdmRoom.Height; y++)
            {
                for (int x = rdmRoom.X; x < rdmRoom.X + rdmRoom.Width; x++)
                {
                    // If the tile is blocked, skip
                    if (level.IsBlocked(x, y))
                        continue;

                    positions.Add(new(x, -y));
                }
            }

            // If no valid position, skip
            if (positions.Count == 0)
                return Position;

            return positions[level.Random.Next(0, positions.Count)];
        }

        #endregion

        #region IEventable

        /// <inheritdoc/>
        public void OnEntityLand(GridEntity entity)
        {
            if (entity is PlayerEntity player)
            {
                OnPlayerTouched(player);
                return;
            }
        }

        /// <inheritdoc/>
        public void OnEntityLanded(GridEntity entity)
        {
            if (entity is PlayerEntity player)
            {
                OnPlayerTouched(player);
                return;
            }
        }

        private void OnPlayerTouched(PlayerEntity player)
        {
            // Needs to check if you aleady started a battle.
            // If the player lands on an enemy, this method will be called twice,
            // because the player calls OnEntityLanded and this entity calls OnEntityLand.
            GameManager.Instance.StartBattle(this, player);
        }

        #endregion

        #region IMovable

        /// <inheritdoc/>
        void IMovable.OnMoveStart(Movement movement) => FlipByMovement(movement);

        #endregion
    }
}