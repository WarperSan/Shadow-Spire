using System.Collections;
using GridModule;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerModule
{
    public class PlayerEntity : GridEntity
    {
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            GameManager.Instance.player = this;
            StartCoroutine(GameManager.Instance.ProcessTurn(this));
        }

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
        public override void OnTurnStarted()
        {
            requestMove = null; // Clear previous moves
        }

        /// <inheritdoc/>
        public override IEnumerator ExecuteTurn()
        {
            // Show cursors
            ShowNeededCursors();
            yield return new WaitForSeconds(PlayerCursor.DURATION_MS / 1000f);

            while (requestMove == null)
                yield return null;

            Movement selected = requestMove.Value;

            // Hide all cursors
            HideAllCursors();

            // Process move
            yield return ApplyMovement(selected);
        }

        #endregion

        #region Animation

        [Header("Animations")]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        /// <inheritdoc/>
        protected override void OnMoveStart(Movement movement)
        {
            // If going right, unflip
            if (movement == Movement.RIGHT)
            {
                spriteRenderer.flipX = false;
            }
            // If going left, flip
            else if (movement == Movement.LEFT)
            {
                spriteRenderer.flipX = true;
            }

            animator.SetBool("isWalking", true);
        }

        protected override void OnMoveEnd()
        {
            animator.SetBool("isWalking", false);
        }

        #endregion

        #region Cursors

        [Header("Cursors")]
        [SerializeField]
        private PlayerCursor upCursor;

        [SerializeField]
        private PlayerCursor rightCursor;

        [SerializeField]
        private PlayerCursor downCursor;

        [SerializeField]
        private PlayerCursor leftCursor;

        /// <summary>
        /// Shows the cursors of the movement allowed by the player
        /// </summary>
        private void ShowNeededCursors()
        {
            upCursor.SetVisibility(CanMove(Movement.UP));
            rightCursor.SetVisibility(CanMove(Movement.RIGHT));
            downCursor.SetVisibility(CanMove(Movement.DOWN));
            leftCursor.SetVisibility(CanMove(Movement.LEFT));
        }

        /// <summary>Hides all the cursors</summary>
        private void HideAllCursors()
        {
            upCursor.SetVisibility(false);
            rightCursor.SetVisibility(false);
            downCursor.SetVisibility(false);
            leftCursor.SetVisibility(false);
        }

        #endregion
    }
}