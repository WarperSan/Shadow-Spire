using System.Collections;
using Enemies;
using Entities.Interfaces;
using Managers;

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
            spriteRenderer.flipX = data.IsFlipped;
            spriteRenderer.color = BattleEntity.BattleEntity.GetTypeColor(data.Type);

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

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            turnsRemaining--;

            if (turnsRemaining >= 0)
            {
                yield return null;
                yield break;
            }

            turnsRemaining = waitTurns;

            //if (_data.Pathing == EnemyPathing.DIRECT)

            int[] path = PathFindingManager.FindPath(this, GameManager.Instance.player);
            Movement[] movements = PathFindingManager.GetDirections(path);

            // If no path found or on the same tile
            if (movements == null || movements.Length == 0)
            {
                yield return null;
                yield break;
            }

            yield return movements[0];

            //if (_data.Pathing == EnemyPathing.RANDOM)
            //    movement += new GoRandomPosition(this).Alias("GoRandomPosition");
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
        void IMovable.OnMoveStart(Movement movement)
        {
            if (_data.IsFlipped)
            {
                if (movement == Movement.LEFT)
                    movement = Movement.RIGHT;
                else if (movement == Movement.RIGHT)
                    movement = Movement.LEFT;
            }

            FlipByMovement(movement);
        }

        #endregion
    }
}