using System.Collections;
using Entities.Interfaces;
using Managers;
using UnityEngine;

namespace Entities
{
    public class EnemyEntity : GridEntity, ITurnable, IMovable, IEventable
    {
        #region ITurnable

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            path = PathFindingManager.FindPath(this, GetPathFindingTarget());
            movements = PathFindingManager.GetDirections(path);

            // If no path found
            if (movements == null)
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
            // If not player, skip
            if (entity is not PlayerEntity)
                return;

            Debug.Log("hit");
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