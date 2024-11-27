using System.Collections;
using Entities.Interfaces;
using UnityEngine;
using UtilsModule;

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
        private const float MOVEMENT_DURATION_MS = 100f;

        public IEnumerator ExecuteTurn()
        {
            // An entity must be a Turnable and a Movable in order to move 
            if (this is not ITurnable turnable)
                yield break;

            if (this is not IMovable movable)
                yield break;

            turnable.OnTurnStarted();

            CoroutineWithData cd = new(this, turnable.Think());
            yield return cd.coroutine;

            Movement movement = (Movement?)cd.result ?? Movement.LEFT;
            yield return movable.ApplyMovement(movement);

            turnable.OnTurnEnded();
        }

        #region Render

        [Header("Render")]
        [SerializeField]
        private SpriteRenderer spriteRenderer;

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

        #region MonoBehaviour

        /// <inheritdoc/>
        private void Start()
        {
            if (this is ITurnable && this is not PlayerEntity)
                TurnManager.Instance.RegisterEntity(this);
        }

        #endregion
    }
}