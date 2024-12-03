using System.Collections;
using Enemies;
using Entities.Interfaces;
using Managers;
using UnityEngine;

namespace Entities
{
    public class EnemyEntity : GridEntity, ITurnable, IMovable, IEventable
    {
        public EnemySO temp;

        private void Start()
        {
            EnemyData = temp;
        }

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
            _data = data;
        }

        #endregion

        #region ITurnable

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            path = PathFindingManager.FindPath(this, GetPathFindingTarget());
            movements = PathFindingManager.GetDirections(path);

            // If no path found or on the same tile
            if (movements == null || movements.Length == 0)
                yield return null;
            else
                yield return movements[0];
        }

        #endregion

        #region Path Finding

        private int[] path;
        private Movement[] movements;

        protected virtual GridEntity GetPathFindingTarget() => GameManager.Instance.player;

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