using Dungeon.Generation;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
	/// <summary>
	/// Drawer that creates a treasure room
	/// </summary>
	public class TreasureRoomDrawer : RoomDrawer
	{
		private readonly GameObject _potionPrefab;
		private readonly Transform _spawnedParent;

		#region RoomDrawer

		/// <inheritdoc/>
		protected override RoomType Type => RoomType.Treasure;

		/// <inheritdoc/>
		protected override void OnDraw(Room room)
		{
			for (int y = room.Y; y < room.Y + room.Height; y++)
			{
				for (int x = room.X; x < room.X + room.Width; x++)
				{
					// If not a treasure, skip
					if (!Level.Has(x, y, Tile.Treasure))
						continue;

					GameObject treasure = Object.Instantiate(_potionPrefab, _spawnedParent);
					treasure.transform.position = new Vector3(x, -y, 0);
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnProcess(Room room)
		{
			Level.Add(room.X + room.Width / 2, room.Y + room.Height / 2, Tile.Treasure);
		}

		#endregion

		#region Drawer

		public TreasureRoomDrawer(DungeonResult level, GameObject potionPrefab, Transform spawnedParent) : base(level)
		{
			_potionPrefab = potionPrefab;

			GameObject parent = new()
			{
				name = "Treasures",
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