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
        public UnityEvent OnEnterUI;
        public UnityEvent OnEscapeUI;

        [Header("Minigame")]
        public UnityEvent<Vector2> OnMoveMinigame;

        #region Inputs

        private PlayerInput input;

        public void SwitchToPlayer() => input.SwitchCurrentActionMap("Player");
        public void SwitchToMiniGame() => input.SwitchCurrentActionMap("Minigame-Player");
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

        public void MoveMinigame(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            OnMoveMinigame?.Invoke(dir);
        }

        public void EnterUI(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            OnEnterUI?.Invoke();
        }

        public void EscapeUI(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            OnEscapeUI?.Invoke();
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