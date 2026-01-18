using System.Collections;
using System.Collections.Generic;
using Dungeon.Generation;
using GridEntities.Abstract;
using GridEntities.Entities;
using GridEntities.Interfaces;
using UnityEngine;

namespace Managers
{
	public class TurnManager : MonoBehaviour, IDungeonReceive
	{
		#region Entities

		private readonly List<GridEntity> _turnEntities = new();
		private readonly List<GridEntity> _foundEntities = new();

		public void AddEntity(GridEntity entity)
		{
			_foundEntities.Add(entity);

			if (entity is not ITurnable)
				return;

			_turnEntities.Add(entity);
		}

		public void RemoveEntity(GridEntity entity)
		{
			_foundEntities.Remove(entity);

			if (entity is not ITurnable)
				return;

			_turnEntities.Remove(entity);
		}

		#endregion

		#region Turn

		private IEnumerator ProcessTurn()
		{
			bool @continue = true;

			while (@continue)
			{
				foreach (GridEntity entity in _turnEntities)
				{
					// If player is in battle or if the level is over, skip turn
					if (GameManager.Instance.IsInBattle || GameManager.Instance.IsLevelOver || GameManager.Instance.isPlayerDead)
					{
						@continue = false;
						break;
					}

					// If entity doesnt exist, skip
					if (entity == null)
						continue;

					yield return entity.ExecuteTurn();

					// Check for event
					foreach (GridEntity item in _foundEntities)
					{
						// If entity doesnt exist, skip
						if (item == null)
							continue;

						// If checking self, skip
						if (item == entity)
							continue;

						// If not on the same position, skip
						if (item.Position != entity.Position)
							continue;

						if (entity is IEventable landOn)
							landOn.OnEntityLand(item);

						if (item is IEventable landedOn)
							landedOn.OnEntityLanded(entity);
					}
				}
			}

			if (GameManager.Instance.isPlayerDead)
				GameManager.Instance.Defeat();
		}

		public void StartTurn() => StartCoroutine(ProcessTurn());

		#endregion

		#region IDungeonReceive

		/// <inheritdoc/>
		public void OnLevelStart(DungeonResult level)
		{
			PlayerEntity player = level.Player;

			_foundEntities.Clear();
			_turnEntities.Clear();

			AddEntity(player); // Make the player the first entity

			GridEntity[] allEntities = FindObjectsByType<GridEntity>(FindObjectsSortMode.None);

			foreach (GridEntity item in allEntities)
			{
				// Don't add player twice
				if (item == player)
					continue;

				AddEntity(item);
			}

			StartTurn();
		}

		/// <inheritdoc/>
		public void OnLevelEnd(DungeonResult level) { }

		#endregion
	}
}