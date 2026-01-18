using Dungeon.Generation;
using Enemies;
using Entities.Grid.Entities;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
	/// <summary>
	/// Drawer that creates enemies in a room
	/// </summary>
	public class EnemyRoomDrawer : RoomDrawer
	{
		private readonly EnemySo[] _enemyPool;
		private readonly Transform _spawnedParent;
		private readonly GameObject _enemyPrefab;

		#region RoomDrawer

		/// <inheritdoc/>
		protected override RoomType Type => RoomType.Enemy;

		/// <inheritdoc/>
		protected override void OnDraw(Room room)
		{
			for (int y = room.Y; y < room.Y + room.Height; y++)
			{
				for (int x = room.X; x < room.X + room.Width; x++)
				{
					// If not an enemy, skip
					if (!Level.Has(x, y, Tile.Enemy))
						continue;

					GameObject enemy = Object.Instantiate(_enemyPrefab, _spawnedParent);
					enemy.transform.position = new Vector3(x, -y, 0);

					if (enemy.TryGetComponent(out EnemyEntity entity))
						entity.SetData(_enemyPool[Level.Random.Next(0, _enemyPool.Length)], Level.Index);
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnProcess(Room room)
		{
			// Find valid positions
			System.Collections.Generic.List<Vector2Int> positions = GetValidPositions(room, ValidEnemyPredicate);

			// If no valid position, skip
			if (positions.Count == 0)
				return;

			int count = Mathf.CeilToInt(room.Width * room.Height / 8f * 0.6f);
			count = Mathf.Min(count, positions.Count);

			for (int i = 0; i < count; i++)
			{
				int rdmIndex = Level.Random.Next(0, positions.Count);
				Vector2Int pos = positions[rdmIndex];

				Level.Add(pos.x, pos.y, Tile.Enemy);

				positions.RemoveAt(rdmIndex);
			}
			return;

			bool ValidEnemyPredicate(int x, int y)
			{
				// If there is a door to the left, skip
				if (Level.HasDoor(x - 1, y))
					return false;

				// If there is a door to the right, skip
				if (Level.HasDoor(x + 1, y))
					return false;

				// If there is a door to the top, skip
				if (Level.HasDoor(x, y - 1))
					return false;

				// If there is a door to the bottom, skip
				if (Level.HasDoor(x, y + 1))
					return false;

				return true;
			}
		}

		#endregion

		#region Drawer

		public EnemyRoomDrawer(
			DungeonResult level,
			GameObject    enemyPrefab,
			EnemySo[]     enemyPool,
			Transform     spawnedParent
		) : base(level)
		{
			_enemyPool = enemyPool;
			_enemyPrefab = enemyPrefab;

			GameObject parent = new()
			{
				name = "Enemies",
				transform =
				{
					parent = spawnedParent
				}
			};
			_spawnedParent = parent.transform;
		}

		/// <inheritdoc/>
		public override void Clear() => Object.Destroy(_spawnedParent.gameObject);

		#endregion
	}
}