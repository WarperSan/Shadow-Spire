using Dungeon.Generation;
using UnityEngine;
using UtilsModule;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
    public class SpikesRoomDrawer : RoomDrawer
    {
        private readonly GameObject SpikesPrefab;
        private readonly Transform SpawnedParent;

        #region RoomDrawer

        /// <inheritdoc/>
        public override RoomType Type => RoomType.SPIKES;

        /// <inheritdoc/>
        protected override void OnDraw(Room room)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    // If not a treasure, skip
                    if (!Level.Has(x, y, Tile.SPIKES))
                        continue;

                    GameObject treasure = Object.Instantiate(SpikesPrefab, SpawnedParent);
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
                    Level.Add(x, y, Tile.SPIKES);
                    Level.Add(x, y, Tile.COVERED_GROUND);
                }
            }
        }

        #endregion

        #region Drawer

        public SpikesRoomDrawer(DungeonResult level, GameObject spikesPrefab, Transform spawnedParent) : base(level)
        {
            SpikesPrefab = spikesPrefab;

            GameObject parent = new GameObject
            {
                name = "Spikes"
            };
            parent.transform.parent = spawnedParent;
            SpawnedParent = parent.transform;
        }

        /// <inheritdoc/>
        public override void Clear() => Object.Destroy(SpawnedParent.gameObject);

        #endregion
    }
}