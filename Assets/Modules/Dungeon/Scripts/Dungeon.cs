using System.Collections;
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

        #region Game Flow

        public DungeonResult Level { get; private set; }

        private IDungeonReceive[] Receivers;

        public bool IsLevelOver { get; private set; }

        public void StartLevel(int? seed = null)
        {
            IsLevelOver = false;

            seed ??= Random.Range(int.MinValue, int.MaxValue);

            var random = new System.Random(seed.Value);

            Debug.Log("Seed: " + seed);

            var lvl = new DungeonGenerator(random).Generate(12, 12);
            Level = lvl;

            lvl.Random = random;
            lvl.WallGrid = wallDrawer.Process(lvl.Rooms);
            lvl.EntranceExitGrid = entranceExitDrawer.Process(lvl.Rooms);
            lvl.GroundGrid = groundDrawer.Process(lvl.Rooms);
            lvl.DoorGrid = doorDrawer.Process(lvl.Rooms);

            wallDrawer.Draw(Level.WallGrid, lvl.Rooms);
            entranceExitDrawer.Draw(lvl.EntranceExitGrid, lvl.Rooms);
            groundDrawer.Draw(lvl.GroundGrid, lvl.Rooms);
            doorDrawer.Draw(lvl.DoorGrid, lvl.Rooms);

            Receivers = FindObjectsOfType<MonoBehaviour>().Where(m => m is IDungeonReceive).Select(m => m as IDungeonReceive).ToArray();

            foreach (var receiver in Receivers)
                receiver.OnLevelStart(Level);
        }

        public void EndLevel()
        {
            IsLevelOver = true;
            StartCoroutine(LevelEndSequence());
        }

        private IEnumerator LevelEndSequence()
        {
            yield return new WaitForSeconds(1.5f); // Level end animation

            // Clear all drawers
            wallDrawer.Clear();
            doorDrawer.Clear();
            groundDrawer.Clear();
            entranceExitDrawer.Clear();

            yield return null; // Wait 1 frame

            StartLevel();
        }

        #endregion

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}