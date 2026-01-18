using Dungeon.Generation;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
	/// <summary>
	/// Drawer that creates a room of spike
	/// </summary>
	public class SpikesRoomDrawer : RoomDrawer
	{
		private readonly GameObject _spikesPrefab;
		private readonly Transform _spawnedParent;

		#region RoomDrawer

		/// <inheritdoc/>
		protected override RoomType Type => RoomType.Spikes;

		/// <inheritdoc/>
		protected override void OnDraw(Room room)
		{
			for (int y = room.Y; y < room.Y + room.Height; y++)
			{
				for (int x = room.X; x < room.X + room.Width; x++)
				{
					// If not a treasure, skip
					if (!Level.Has(x, y, Tile.Spikes))
						continue;

					GameObject treasure = Object.Instantiate(_spikesPrefab, _spawnedParent);
					treasure.transform.position = new Vector3(x, -y, 0);
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnProcess(Room room)
		{
			for (int y = room.Y; y < room.Y + room.Height; y++)
			{
				for (int x = room.X; x < room.X + room.Width; x++)
				{
					Level.Add(x, y, Tile.Spikes);
					Level.Add(x, y, Tile.CoveredGround);
				}
			}
		}

		#endregion

		#region Drawer

		public SpikesRoomDrawer(DungeonResult level, GameObject spikesPrefab, Transform spawnedParent) : base(level)
		{
			_spikesPrefab = spikesPrefab;

			GameObject parent = new()
			{
				name = "Spikes",
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