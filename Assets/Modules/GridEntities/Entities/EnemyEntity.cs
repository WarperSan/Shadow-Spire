using System.Collections;
using System.Collections.Generic;
using Enemies;
using GridEntities.Abstract;
using GridEntities.Interfaces;
using Managers;
using UnityEngine;
using Utils;

namespace GridEntities.Entities
{
	public class EnemyEntity : GridEntity, ITurnable, IMovable, IEventable
	{
		#region Data

		public EnemyInstance Instance;
		private EnemySo _data;

		private int _waitTurns;
		private int _movesPerTurn;
		private int _turnsRemaining;

		public void SetData(EnemySo data, int level)
		{
			Instance = new EnemyInstance(data, level);

			spriteRenderer.sprite = data.overworldSprite;
			spriteRenderer.color = data.baseType.GetColor();

			_turnsRemaining = _waitTurns = data.movementSpeed switch
			{
				EnemyMovementSpeed.VerySlow => 2,
				EnemyMovementSpeed.Slow      => 1,
				_                            => 0
			};

			_movesPerTurn = data.movementSpeed switch
			{
				EnemyMovementSpeed.Fast      => 2,
				EnemyMovementSpeed.VeryFast => 3,
				_                            => 1
			};

			_data = data;
		}

		#endregion

		#region ITurnable

		private int[] _path;
		private Movement[] _movements;
		private int _currentIndex = -1;

		/// <inheritdoc/>
		IEnumerator ITurnable.Think()
		{
			_turnsRemaining--;

			if (_turnsRemaining >= 0)
				yield break;

			_turnsRemaining = _waitTurns;

			if (_data.pathing == EnemyPathing.Direct || _path == null || _currentIndex >= _path.Length)
				UpdatePath();

			// If no path found or on the same tile
			if (_movements == null || _movements.Length == 0)
				yield break;

			if (_movesPerTurn + _currentIndex >= _movements.Length)
			{
				yield return _movements[_currentIndex..];

				_currentIndex = _path.Length;
				yield break;
			}

			yield return _movements[_currentIndex..(_movesPerTurn + _currentIndex)];

			_currentIndex += _movesPerTurn;
		}

		private void UpdatePath()
		{
			Vector2Int target = Position;

			if (_data.pathing == EnemyPathing.Direct)
				target = GameManager.Instance.player.Position;
			else if (_data.pathing == EnemyPathing.Random)
				target = GetRandomPosition();

			_path = PathFindingManager.FindPath(this, target);
			_movements = PathFindingManager.GetDirections(_path);
			_currentIndex = 0;
		}

		private Vector2Int GetRandomPosition()
		{
			Dungeon.Generation.DungeonResult level = GameManager.Instance.Level;
			Dungeon.Generation.Room rdmRoom = level.Rooms[level.Random.Next(0, level.Rooms.Length)];

			List<Vector2Int> positions = new();

			for (int y = rdmRoom.Y; y < rdmRoom.Y + rdmRoom.Height; y++)
			{
				for (int x = rdmRoom.X; x < rdmRoom.X + rdmRoom.Width; x++)
				{
					// If the tile is blocked, skip
					if (level.IsBlocked(x, y))
						continue;

					positions.Add(new Vector2Int(x, -y));
				}
			}

			// If no valid position, skip
			if (positions.Count == 0)
				return Position;

			return positions[level.Random.Next(0, positions.Count)];
		}

		#endregion

		#region IEventable

		/// <inheritdoc/>
		public void OnEntityLand(GridEntity entity)
		{
			if (entity is PlayerEntity player)
			{
				OnPlayerTouched(player);
				return;
			}
		}

		/// <inheritdoc/>
		public void OnEntityLanded(GridEntity entity)
		{
			if (entity is PlayerEntity player)
			{
				OnPlayerTouched(player);
				return;
			}
		}

		private void OnPlayerTouched(PlayerEntity player)
		{
			// Needs to check if you aleady started a battle.
			// If the player lands on an enemy, this method will be called twice,
			// because the player calls OnEntityLanded and this entity calls OnEntityLand.
			GameManager.Instance.StartBattle(this, player);
		}

		#endregion

		#region IMovable

		/// <inheritdoc/>
		void IMovable.OnMoveStart(Movement movement) => FlipByMovement(movement);

		#endregion
	}
}