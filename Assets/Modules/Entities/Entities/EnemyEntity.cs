using System.Collections;
using Entities.Interfaces;
using Managers;
using UnityEngine;

namespace Entities
{
    public class EnemyEntity : GridEntity, ITurnable, IMovable, IEventable
    {
        #region ITurnable

        private int[] path;

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            var graph = GameManager.Instance.Level.TileGraph;
            var start = graph.GetID(Position.x, -Position.y);
            var end = graph.GetID(GameManager.Instance.player.Position.x, -GameManager.Instance.player.Position.y);

            path = graph.GetPath(start, end);
            var nextPos = graph.GetNode(path[1]).Position + new Vector2(-0.5f, 0.5f);

            if (nextPos.x < Position.x)
                yield return Movement.LEFT;
            else if (nextPos.y < Position.y)
                yield return Movement.DOWN;
            else if (nextPos.x > Position.x)
                yield return Movement.RIGHT;
            else
                yield return Movement.UP;
        }

        /// <inheritdoc/>
        private void OnDrawGizmos()
        {
            if (path == null)
                return;

            foreach (var item in path)
            {
                var pos = GameManager.Instance.Level.TileGraph.GetNode(item).Position;
                Gizmos.DrawIcon(pos, "sv_icon_dot3_pix16_gizmo");
            }
        }

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
    }
}