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

		private readonly List<GridEntity> turnEntities = new();
		private readonly List<GridEntity> foundEntities = new();

		public void AddEntity(GridEntity entity)
		{
			foundEntities.Add(entity);

			if (entity is not ITurnable)
				return;

			turnEntities.Add(entity);
		}

		public void RemoveEntity(GridEntity entity)
		{
			foundEntities.Remove(entity);

			if (entity is not ITurnable)
				return;

			turnEntities.Remove(entity);
		}

		#endregion

		#region Turn

		private IEnumerator ProcessTurn()
		{
			bool _continue = true;

			while (_continue)
			{
				foreach (GridEntity entity in turnEntities)
				{
					// If player is in battle or if the level is over, skip turn
					if (GameManager.Instance.IsInBattle || GameManager.Instance.IsLevelOver || GameManager.Instance.IsPlayerDead)
					{
						_continue = false;
						break;
					}

					// If entity doesnt exist, skip
					if (entity == null)
						continue;

					yield return entity.ExecuteTurn();

					// Check for event
					foreach (GridEntity item in foundEntities)
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

			if (GameManager.Instance.IsPlayerDead)
				GameManager.Instance.Defeat();
		}

		public void StartTurn() => StartCoroutine(ProcessTurn());

		#endregion

		#region IDungeonReceive

		/// <inheritdoc/>
		public void OnLevelStart(DungeonResult level)
		{
			PlayerEntity player = level.Player;

			foundEntities.Clear();
			turnEntities.Clear();

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