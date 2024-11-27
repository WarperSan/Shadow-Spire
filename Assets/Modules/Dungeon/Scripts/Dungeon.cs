using System.Linq;
using Dungeon.Drawers;
using Dungeon.Generation;
using UnityEngine;
using UtilsModule;

namespace Dungeon
{
    public class Dungeon : Singleton<Dungeon>
    {
        public int overSeed;
        public bool useSeed;

        #region Drawers

        [Header("Drawers")]
        [SerializeField]
        private WallDrawer wallDrawer;

        [SerializeField]
        private DoorDrawer doorDrawer;

        [SerializeField]
        private GroundDrawer groundDrawer;

        [SerializeField]
        private EntranceExitDrawer entranceExitDrawer;

        #endregion

        private void Start()
        {
            var seed = Random.Range(int.MinValue, int.MaxValue);

            if (useSeed)
                seed = overSeed;

            StartLevel(seed);
        }

        public DungeonResult Level { get; private set; }

        private IDungeonReceive[] Receivers;

        public void StartLevel(int? seed = null)
        {
            seed ??= Random.Range(int.MinValue, int.MaxValue);

            var random = new System.Random(seed.Value);

            Debug.Log("Seed: " + seed);

            Level = new DungeonGenerator(random).Generate(12, 12);

            Level.Random = random;
            Level.WallGrid = wallDrawer.Process(Level.Rooms);
            Level.EntranceExitGrid = entranceExitDrawer.Process(Level.Rooms);
            Level.GroundGrid = groundDrawer.Process(Level.Rooms);
            Level.DoorGrid = doorDrawer.Process(Level.Rooms);

            wallDrawer.Draw(Level.WallGrid, Level.Rooms);
            entranceExitDrawer.Draw(Level.EntranceExitGrid, Level.Rooms);
            groundDrawer.Draw(Level.GroundGrid, Level.Rooms);
            doorDrawer.Draw(Level.DoorGrid, Level.Rooms);

            Receivers = FindObjectsOfType<MonoBehaviour>().Where(m => m is IDungeonReceive).Select(m => m as IDungeonReceive).ToArray();

            foreach (var receiver in Receivers)
                receiver.OnLevelStart(Level);
        }

        public void EndLevel()
        {
            
        }

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}