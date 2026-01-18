using System.Collections;
using GridEntities.Abstract;
using UnityEngine;
using Utils;

namespace GridEntities.Interfaces
{
	/// <summary>
	/// Every possible movements
	/// </summary>
	public enum Movement
	{
		None = 0,
		Left = 1,
		Right = 2,
		Up = 3,
		Down = 4
	}

	/// <summary>
	/// Defines an entity that can move
	/// </summary>
	public interface IMovable
	{
		private const float MOVEMENT_DURATION_MS = 100f;

		public IEnumerator ApplyMovement(Movement movement)
		{
			// Must be a GridEntity
			if (this is not GridEntity gridEntity)
				yield break;

			Transform transform = gridEntity.transform;

			const float DURATION = MOVEMENT_DURATION_MS / 1000f;

			OnMoveStart(movement);

			// Make a note of where we are and where we are going.
			Vector2 startPosition = transform.position;
			Vector2 endPosition = GetNextPosition(movement);

			// If there is an object at the next position
			if (!CanMove(movement))
				yield return new WaitForSeconds(DURATION);
			else
			{
				// Smoothly move in the desired direction taking the required time.
				float elapsedTime = 0;

				while (elapsedTime < DURATION)
				{
					elapsedTime += Time.deltaTime;
					float percent = elapsedTime / DURATION;
					transform.position = Vector2.Lerp(startPosition, endPosition, percent);
					yield return null;
				}

				// Make sure we end up exactly where we want.
				transform.position = endPosition;
			}

			OnMoveEnd();
		}

		public bool CanMove(Movement movement)
		{
			Vector2Int gridPosition = GetNextPosition(movement);
			Vector3 endPosition = new(gridPosition.x, gridPosition.y);
			endPosition += new Vector3(1 / 2f, -1 / 2f, 0);

			return Physics2D.OverlapBox(endPosition,
				0.9f * Vector2.one,
				0,
				LayerMask.GetMask("Stoppable")) == null;
		}

		private Vector2Int GetNextPosition(Movement movement)
		{
			Vector2Int position = Vector2Int.zero;
			Vector2Int movePos = Vector2Int.zero;

			if (this is GridEntity gridEntity)
				position = gridEntity.Position;

			switch (movement)
			{
				case Movement.Left:
					movePos = new Vector2Int(-1, 0);
					break;
				case Movement.Right:
					movePos = new Vector2Int(1, 0);
					break;
				case Movement.Up:
					movePos = new Vector2Int(0, 1);
					break;
				case Movement.Down:
					movePos = new Vector2Int(0, -1);
					break;
				default:
					break;
			}

			position += movePos;

			Dungeon.Generation.DungeonResult level = Managers.GameManager.Instance.Level;

			if (level == null)
			{
				Debug.LogError("No level has been initialized.");
				return position;
			}

			if (level.HasDoor(position.x, -position.y))
				position += movePos;

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