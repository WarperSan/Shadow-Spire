using System.Collections;
using System.Linq;
using Dungeon.Drawers;
using Dungeon.Generation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilsModule;

namespace Managers
{
    public class DungeonManager : Singleton<DungeonManager>
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
            originalLevelText = transitionLevelText.text;

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

            var lvl = new DungeonGenerator(random).Generate(18, 12);
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
            yield return new WaitForSeconds(0.2f);

            const int END_TRANSITION_TICKS = 4;

            var blackoutColor = new Color(0, 0, 0, 0);

            for (int i = 1; i <= END_TRANSITION_TICKS; i++)
            {
                blackoutColor.a = 1f / END_TRANSITION_TICKS * i;
                blackout.color = blackoutColor;
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(0.6f);

            transitionLevelText.gameObject.SetActive(true);
            transitionLevelText.text = string.Format(originalLevelText, 1);

            yield return new WaitForSeconds(1f);

            transitionLevelText.text = string.Format(originalLevelText, 2);

            yield return new WaitForSeconds(1.5f);

            transitionLevelText.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.2f); // Level end animation

            // Clear all drawers
            wallDrawer.Clear();
            doorDrawer.Clear();
            groundDrawer.Clear();
            entranceExitDrawer.Clear();

            yield return null; // Wait 1 frame

            StartLevel();

            yield return null;

            for (int i = 1; i <= END_TRANSITION_TICKS; i++)
            {
                blackoutColor.a = 1f - 1f / END_TRANSITION_TICKS * i;
                blackout.color = blackoutColor;
                yield return new WaitForSeconds(0.2f);
            }
        }

        #endregion

        #region UI

        [Header("UI")]
        [SerializeField]
        private Graphic blackout;

        [SerializeField]
        private TextMeshProUGUI transitionLevelText;
        private string originalLevelText;

        #endregion

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}