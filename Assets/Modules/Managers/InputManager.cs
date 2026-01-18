using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utils;

namespace Managers
{
	[RequireComponent(typeof(PlayerInput))]
	public class InputManager : Singleton<InputManager>
	{
		[FormerlySerializedAs("OnMovePlayer")]
		[Header("Player")]
		public UnityEvent<Vector2> onMovePlayer;

		[FormerlySerializedAs("OnMoveUI")]
		[Header("UI")]
		public UnityEvent<Vector2> onMoveUI;

		[FormerlySerializedAs("OnEnterUI")]
		public UnityEvent onEnterUI;
		[FormerlySerializedAs("OnEscapeUI")]
		public UnityEvent onEscapeUI;

		[FormerlySerializedAs("OnMoveMinigame")]
		[Header("Minigame")]
		public UnityEvent<Vector2> onMoveMinigame;

		#region Inputs

		private PlayerInput _input;

		public void SwitchToPlayer()   => _input.SwitchCurrentActionMap("Player");
		public void SwitchToMiniGame() => _input.SwitchCurrentActionMap("Minigame-Player");
		public void SwitchToUI()       => _input.SwitchCurrentActionMap("UI");

		#endregion

		#region Events

		public void MovePlayer(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;

			Vector2 dir = context.ReadValue<Vector2>();
			onMovePlayer?.Invoke(dir);
		}

		public void MoveUI(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;

			Vector2 dir = context.ReadValue<Vector2>();
			onMoveUI?.Invoke(dir);
		}

		public void MoveMinigame(InputAction.CallbackContext context)
		{
			Vector2 dir = context.ReadValue<Vector2>();
			onMoveMinigame?.Invoke(dir);
		}

		public void EnterUI(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;

			onEnterUI?.Invoke();
		}

		public void EscapeUI(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;

			onEscapeUI?.Invoke();
		}

		#endregion

		#region Singleton

		/// <inheritdoc/>
		protected override bool DestroyOnLoad => true;

		/// <inheritdoc/>
		protected override void OnAwake()
		{
			_input = GetComponent<PlayerInput>();
		}

		#endregion
	}
}