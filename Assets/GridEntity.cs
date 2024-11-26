using System.Collections;
using UnityEngine;

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

    public class GridEntity : MonoBehaviour
    {
        private const float MOVEMENT_DURATION_MS = 300f;
        private const float GRID_SIZE = 1;

        // Execute Turn
        // On Turn Ended

        #region Turn

        public virtual IEnumerator ExecuteTurn()
        {
            yield return null; // Nothing
        }

        /// <summary>
        /// Called when this entity's turn starts
        /// </summary>
        public virtual void OnTurnStarted() { }

        /// <summary>
        /// Called when this entity's turn ends
        /// </summary>
        public virtual void OnTurnEnded() { }

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
            Vector3 endPosition = GetNextPosition(movement);
            return true;
            //return Physics2D.OverlapBox(endPosition, new Vector2(GRID_SIZE, GRID_SIZE) * 0.9f, 0) == null;
        }

        private Vector3 GetNextPosition(Movement movement) => transform.position + movement switch
        {
            Movement.LEFT => new Vector3(-1, 0),
            Movement.RIGHT => new Vector3(1, 0),
            Movement.UP => new Vector3(0, 1),
            Movement.DOWN => new Vector3(0, -1),
            _ => new Vector3()
        } * GRID_SIZE;

        /// <summary>
        /// Called before this object starts moving
        /// </summary>
        protected virtual void OnMoveStart(Movement movement) { }

        /// <summary>
        /// Called after this object moved
        /// </summary>
        protected virtual void OnMoveEnd() { }

        #endregion
    }
}