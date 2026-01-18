using Dungeon.Generation;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
	public class TreasureRoomDrawer : RoomDrawer
	{
		private readonly GameObject PotionPrefab;
		private readonly Transform SpawnedParent;

		#region RoomDrawer

		/// <inheritdoc/>
		public override RoomType Type => RoomType.TREASURE;

		/// <inheritdoc/>
		protected override void OnDraw(Room room)
		{
			for (int y = room.Y; y < room.Y + room.Height; y++)
			{
				for (int x = room.X; x < room.X + room.Width; x++)
				{
					// If not a treasure, skip
					if (!Level.Has(x, y, Tile.TREASURE))
						continue;

					GameObject treasure = Object.Instantiate(PotionPrefab, SpawnedParent);
					treasure.transform.position = new Vector3(x, -y, 0);
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnProcess(Room room)
		{
			Level.Add(room.X + room.Width / 2, room.Y + room.Height / 2, Tile.TREASURE);
		}

		#endregion

		#region Drawer

		public TreasureRoomDrawer(DungeonResult level, GameObject potionPrefab, Transform spawnedParent) : base(level)
		{
			PotionPrefab = potionPrefab;

			GameObject parent = new()
			{
				name = "Treasures"
			};
			parent.transform.parent = spawnedParent;
			SpawnedParent = parent.transform;
		}

		/// <inheritdoc/>
		public override void Clear() => Object.Destroy(SpawnedParent.gameObject);

		#endregion
	}
}