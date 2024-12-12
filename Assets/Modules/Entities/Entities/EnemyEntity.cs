using System.Collections;
using Enemies;
using Entities.Interfaces;
using Managers;
using UnityEngine;

namespace Entities
{
    public class EnemyEntity : GridEntity, ITurnable, IMovable, IEventable
    {
        #region Data

        private EnemySO _data;

        public EnemySO EnemyData
        {
            get => _data;
            set => SetData(value);
        }

        private void SetData(EnemySO data)
        {
            spriteRenderer.sprite = data.OverworldSprite;

            if (ColorUtility.TryParseHtmlString(BattleEntity.BattleEntity.GetTypeColor(data.Type), out Color color))
                spriteRenderer.color = color;

            turnsRemaining = waitTurns = data.MovementSpeed switch
            {
                EnemyMovementSpeed.VERY_SLOW => 3,
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

        private int waitTurns;
        private int movesPerTurn;
        private int turnsRemaining;

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

            path = PathFindingManager.FindPath(this, GetPathFindingTarget());
            movements = PathFindingManager.GetDirections(path);

            // If no path found or on the same tile
            if (movements == null || movements.Length == 0)
            {
                yield return null;
                yield break;
            }

            yield return movements[0..movesPerTurn];
        }

        #endregion

        #region Path Finding

        private int[] path;
        private Movement[] movements;

        protected virtual Vector2Int GetPathFindingTarget()
        {
            return GameManager.Instance.player.Position;
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
            GameManager.Instance.StartBattle(this);
        }

        #endregion

        #region IMovable

        /// <inheritdoc/>
        void IMovable.OnMoveStart(Movement movement)
        {
            FlipByMovement(movement);
        }

        #endregion

        #region Gizmos
#if UNITY_EDITOR
        /// <inheritdoc/>
        private void OnDrawGizmos()
        {
            if (path == null)
                return;

            foreach (var item in path)
            {
                var pos = GameManager.Instance.Level.TileGraph.GetNode(item).Position + new Vector2(0.5f, -0.5f);
                Gizmos.DrawIcon(pos, "sv_icon_dot3_pix16_gizmo");
            }
        }
#endif
        #endregion
    }
}