using System.Collections;
using System.Linq;
using Dungeon.Drawers;
using Dungeon.Generation;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Managers
{
    public class DungeonManager : MonoBehaviour
    {
        #region Ground

        [Header("Ground")]
        [SerializeField]
        private Tilemap groundMap;

        [SerializeField]
        private TileBase groundTile;

        #endregion

        #region Walls

        [Header("Walls")]
        [SerializeField]
        private Tilemap wallMap;

        // TOP    = 0b0001 = 1
        // RIGHT  = 0b0010 = 2
        // BOTTOM = 0b0100 = 4
        // LEFT   = 0b1000 = 8
        [SerializeField]
        private TileBase[] wallTiles;

        #endregion

        #region Doors

        [Header("Doors")]
        [SerializeField]
        private TileBase doorTile;

        #endregion

        #region Entities

        [Header("Entities")]

        [SerializeField]
        private EntranceEntity entrance;

        [SerializeField]
        private ExitEntity exit;

        [SerializeField]
        private PlayerEntity player;

        #endregion

        #region UI

        [Header("UI")]
        [SerializeField]
        private Graphic blackout;

        [SerializeField]
        private TextMeshProUGUI transitionLevelText;
        private string originalLevelText;

        private IEnumerator BlackoutFadeIn(int transitionTicks)
        {
            var blackoutColor = new Color(0, 0, 0, 0);

            blackout.color = blackoutColor;
            blackout.gameObject.SetActive(true);

            for (int i = 1; i <= transitionTicks; i++)
            {
                blackoutColor.a = 1f / transitionTicks * i;
                blackout.color = blackoutColor;
                yield return new WaitForSeconds(0.2f);
            }
        }

        private IEnumerator BlackoutFadeOut(int transitionTicks)
        {
            var blackoutColor = new Color(0, 0, 0, 1);

            blackout.color = blackoutColor;
            blackout.gameObject.SetActive(true);

            for (int i = 1; i <= transitionTicks; i++)
            {
                blackoutColor.a = 1f - 1f / transitionTicks * i;
                blackout.color = blackoutColor;
                yield return new WaitForSeconds(0.2f);
            }

            blackout.gameObject.SetActive(false);
        }

        #endregion

        #region Generation

        private Drawer[] DrawerPipeline;
        private IDungeonReceive[] Receivers;

        public DungeonResult StartLevel(DungeonSettings settings)
        {
            // Generate level
            var random = new System.Random(settings.Seed);
            var lvl = new DungeonGenerator(random).Generate(settings.Width, settings.Height, settings.SliceCount);

            // Create drawers
            DrawerPipeline = new Drawer[]
            {
                new WallDrawer(lvl, wallMap, wallTiles),
                new DoorDrawer(lvl, wallMap, doorTile),
                new EntranceExitDrawer(lvl, entrance, exit, player),
                new GroundDrawer(lvl, groundMap, groundTile),
            };

            // Process the level
            lvl.Random = random;
            lvl.Grid = Drawer.CreateEmpty(lvl.Rooms);

            foreach (var drawer in DrawerPipeline)
                drawer.Process(lvl.Rooms);

            // Draw the level
            foreach (var drawer in DrawerPipeline)
                drawer.Draw(lvl.Rooms);

            // Notify all receivers
            Receivers = FindObjectsOfType<MonoBehaviour>().Where(m => m is IDungeonReceive).Select(m => m as IDungeonReceive).ToArray();

            foreach (var receiver in Receivers)
                receiver.OnLevelStart(lvl);

            return lvl;
        }

        public IEnumerator EndLevel(int currentLevel, int nextLevel, System.Func<IEnumerator> callback = null)
        {
            const int BLACKOUT_TICKS = 4;

            yield return new WaitForSeconds(0.2f);

            yield return BlackoutFadeIn(BLACKOUT_TICKS);

            yield return new WaitForSeconds(0.6f);

            originalLevelText ??= transitionLevelText.text;

            transitionLevelText.gameObject.SetActive(true);
            transitionLevelText.text = string.Format(originalLevelText, currentLevel);

            yield return new WaitForSeconds(1.5f);

            transitionLevelText.text = string.Format(originalLevelText, nextLevel);

            yield return new WaitForSeconds(2f);

            transitionLevelText.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.2f); // Level end animation

            // Clear all drawers
            foreach (var drawer in DrawerPipeline)
                drawer.Clear();

            yield return null; // Wait 1 frame

            if (callback != null)
                yield return callback.Invoke();
            //StartLevel();

            yield return BlackoutFadeOut(BLACKOUT_TICKS);

            yield return new WaitForSeconds(0.2f);
        }

        #endregion
    }
}