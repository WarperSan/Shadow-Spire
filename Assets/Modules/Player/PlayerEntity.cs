using System.Collections;
using GridModule;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerModule
{
    public class PlayerEntity : GridEntity
    {
        #region Inputs

        private Movement? requestMove = null;

        public void Move(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            Vector2 dir = context.ReadValue<Vector2>();

            Movement movement;

            if (dir.x > 0)
                movement = Movement.RIGHT;
            else if (dir.x < 0)
                movement = Movement.LEFT;
            else if (dir.y > 0)
                movement = Movement.UP;
            else
                movement = Movement.DOWN;

            // If can apply movement, register
            if (CanMove(movement))
                requestMove = movement;
        }

        #endregion

        #region Turn

        /// <inheritdoc/>
        protected override void OnTurnStarted()
        {
            requestMove = null; // Clear previous moves
        }

        protected override IEnumerator Think()
        {
            while (requestMove == null)
                yield return null;

            yield return requestMove.Value;
        }

        #endregion

        /// <inheritdoc/>
        protected override void OnMoveStart(Movement movement)
        {
            FlipByMovement(movement);
        }
    }
}