using System.Collections;
using Entities.Interfaces;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Every possible movements
    /// </summary>
    public enum Movement
    {
        LEFT = 0,
        RIGHT = 1,
        UP = 2,
        DOWN = 3
    }

    public abstract class GridEntity : MonoBehaviour
    {
        public Vector2Int Position => new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        #region Turn

        public IEnumerator ExecuteTurn()
        {
            // An entity must be a Turnable
            if (this is not ITurnable turnable)
                yield break;

            turnable.OnTurnStarted();

            yield return ProcessTurn(turnable);

            turnable.OnTurnEnded();
        }

        protected virtual IEnumerator ProcessTurn(ITurnable turnable)
        {
            IEnumerator think = turnable.Think();

            yield return think;

            object result = think.Current;

            if (this is IMovable movable)
            {
                if (result is Movement movement)
                {
                    yield return movable.ApplyMovement(movement);
                }
                else if (result is Movement[] movements)
                {
                    foreach (Movement mov in movements)
                    {
                        yield return movable.ApplyMovement(mov);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }

        #endregion

        #region Render

        [Header("Render")]
        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        /// <inheritdoc/>
        public void FlipByMovement(Movement movement)
        {
            // If going right, flip
            if (movement == Movement.RIGHT)
            {
                spriteRenderer.flipX = true;
            }
            // If going left, unflip
            else if (movement == Movement.LEFT)
            {
                spriteRenderer.flipX = false;
            }
        }

        #endregion
    }
}