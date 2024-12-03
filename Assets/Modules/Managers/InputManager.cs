using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UtilsModule;

namespace Managers
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : Singleton<InputManager>
    {
        [Header("Player")]
        public UnityEvent<Vector2> OnMovePlayer;

        [Header("UI")]
        public UnityEvent<Vector2> OnMoveUI;

        #region Inputs

        private PlayerInput input;

        public void SwitchToPlayer() => input.SwitchCurrentActionMap("Player");
        public void SwitchToUI() => input.SwitchCurrentActionMap("UI");

        #endregion

        #region Events

        public void MovePlayer(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            Vector2 dir = context.ReadValue<Vector2>();
            OnMovePlayer?.Invoke(dir);
        }

        public void MoveUI(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            Vector2 dir = context.ReadValue<Vector2>();
            OnMoveUI?.Invoke(dir);
        }

        #endregion

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            input = GetComponent<PlayerInput>();
        }

        #endregion
    }
}