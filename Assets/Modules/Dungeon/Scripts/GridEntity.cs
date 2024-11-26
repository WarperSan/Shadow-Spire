using System.Collections;
using UnityEngine;
using UtilsModule;

namespace GridModule
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


        #region Turn

        public IEnumerator ExecuteTurn()
        {
            OnTurnStarted();

            CoroutineWithData cd = new(this, Think());

            yield return cd.coroutine;

            Movement movement = (Movement?)cd.result ?? Movement.LEFT;
            yield return ApplyMovement(movement);

            OnTurnEnded();
        }

        protected abstract IEnumerator Think();

        /// <summary>
        /// Called when this entity's turn starts
        /// </summary>
        protected virtual void OnTurnStarted() { }

        /// <summary>
        /// Called when this entity's turn ends
        /// </summary>
        protected virtual void OnTurnEnded() { }

        #endregion

        #region Movement

        public bool IsMoving { get; private set; }

        public Movement Facing { get; private set; } = Movement.RIGHT;

        public IEnumerator ApplyMovement(Movement movement)
        {
            const float duration = MOVEMENT_DURATION_MS / 1000f;

            OnMoveStart(movement);

            // Record that we're moving so we don't accept more input.
            IsMoving = true;
            Facing = movement;

            // Make a note of where we are and where we are going.
            Vector2 startPosition = transform.position;
            Vector2 endPosition = GetNextPosition(movement);

            // If there is an object at the next position
            if (!CanMove(movement))
            {
                yield return new WaitForSeconds(duration);
            }
            else
            {
                // Smoothly move in the desired direction taking the required time.
                float elapsedTime = 0;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    float percent = elapsedTime / duration;
                    transform.position = Vector2.Lerp(startPosition, endPosition, percent);
                    yield return null;
                }

                // Make sure we end up exactly where we want.
                transform.position = endPosition;
            }

            // We're no longer moving so we can accept another move input.
            IsMoving = false;
            OnMoveEnd();
        }

        public bool CanMove(Movement movement)
        {
            Vector2Int gridPosition = GetNextPosition(movement);
            Vector3 endPosition = new(gridPosition.x, gridPosition.y);
            endPosition += new Vector3(1 / 2f, -1 / 2f, 0);
            return Physics2D.OverlapBox(endPosition, 0.9f * Vector2.one, 0, LayerMask.GetMask("Stoppable")) == null;
        }

        private Vector2Int GetNextPosition(Movement movement)
        {
            Vector2Int position = new(
                Mathf.FloorToInt(transform.position.x), 
                Mathf.FloorToInt(transform.position.y)
            );
            Vector2Int movePos = Vector2Int.zero;

            switch (movement)
            {
                case Movement.LEFT:
                    movePos = new(-1, 0);
                    break;
                case Movement.RIGHT:
                    movePos = new(1, 0);
                    break;
                case Movement.UP:
                    movePos = new(0, 1);
                    break;
                case Movement.DOWN:
                    movePos = new(0, -1);
                    break;
                default:
                    break;
            }

            position += movePos;

            if (Dungeon.Dungeon.Instance.Level.DoorGrid[-position.y, position.x])
            {
                position += movePos;
            }

            return position;
        }

        /// <summary>
        /// Called before this object starts moving
        /// </summary>
        protected virtual void OnMoveStart(Movement movement) { }

        /// <summary>
        /// Called after this object moved
        /// </summary>
        protected virtual void OnMoveEnd() { }

        #endregion

        #region Render

        [Header("Render")]
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        /// <inheritdoc/>
        protected void FlipByMovement(Movement movement)
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