using System.Collections;
using UnityEngine;

namespace Entities.Interfaces
{
    public interface IMovable
    {
        private const float MOVEMENT_DURATION_MS = 100f;

        protected Transform Transform { get; }

        public Vector2Int Position => new(Mathf.FloorToInt(Transform.position.x), Mathf.FloorToInt(Transform.position.y));

        public IEnumerator ApplyMovement(Movement movement)
        {
            const float duration = MOVEMENT_DURATION_MS / 1000f;

            OnMoveStart(movement);

            // Make a note of where we are and where we are going.
            Vector2 startPosition = Transform.position;
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
                    Transform.position = Vector2.Lerp(startPosition, endPosition, percent);
                    yield return null;
                }

                // Make sure we end up exactly where we want.
                Transform.position = endPosition;
            }

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
            Vector2Int position = Position;
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
    }
}