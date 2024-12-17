using System.Collections;
using System.Linq;
using Dungeon.Drawers;
using Dungeon.Drawers.Rooms;
using Dungeon.Drawers.Terrain;
using Dungeon.Generation;
using Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        #endregion

        #region Spawn Items

        [Header("Spawn Items")]
        [SerializeField]
        private Transform spawnItemsParent;

        #endregion

        #region Enemies

        [Header("Enemies")]
        [SerializeField]
        private GameObject enemyPrefab;

        [SerializeField]
        private GameObject spikesPrefab;

        #endregion

        #region Treasures

        [Header("Treasures")]
        [SerializeField]
        private GameObject potionPrefab;

        #endregion

        #region Generation

        private Drawer[] DrawerPipeline;
        private IDungeonReceive[] Receivers;

        public DungeonResult GenerateLevel(DungeonSettings settings, PlayerEntity player)
        {
            // Generate level
            var random = new System.Random(settings.Seed);
            var lvl = DungeonGenerator.Generate(random, settings);

            // Create drawers
            DrawerPipeline = new Drawer[]
            {
                // Terrain
                new WallDrawer(lvl, wallMap, wallTiles),
                new DoorDrawer(lvl, wallMap, doorTile),
                new EntranceExitDrawer(lvl, entrance, exit, player),
                new GroundDrawer(lvl, groundMap, groundTile),

                // Rooms
                new EnemyRoomDrawer(lvl, enemyPrefab, Enemies.EnemyInstance.ENEMIES, spawnItemsParent),
                new TreasureRoomDrawer(lvl, potionPrefab, spawnItemsParent),
                new SpikesRoomDrawer(lvl, spikesPrefab, spawnItemsParent),
            };

            // Process the level
            lvl.Player = player;
            lvl.Random = random;
            lvl.Grid = Drawer.CreateEmpty(lvl.Rooms);
            lvl.Height = lvl.Grid.GetLength(0);
            lvl.Width = lvl.Grid.GetLength(1);

            foreach (var drawer in DrawerPipeline)
                drawer.Process(lvl.Rooms);

            // Compute graphs
            lvl.TileGraph = PathFindingManager.ComputeTileGraph(lvl);

            return lvl;
        }

        public void StartLevel(DungeonResult lvl)
        {
            // Draw the level
            foreach (var drawer in DrawerPipeline)
                drawer.Draw(lvl.Rooms);

            // Notify all receivers
            Receivers = FindObjectsOfType<MonoBehaviour>().Where(m => m is IDungeonReceive).Select(m => m as IDungeonReceive).ToArray();

            foreach (var receiver in Receivers)
                receiver.OnLevelStart(lvl);
        }

        public IEnumerator EndLevel(int currentLevel, int nextLevel)
        {
            yield return new WaitForSeconds(0.2f);

            yield return GameManager.Instance.UIManager.FadeInBlackout();
            yield return GameManager.Instance.UIManager.ShowNextLevel(currentLevel, nextLevel);

            ClearDungeon();

            yield return null; // Wait 1 frame
        }

        public void ClearDungeon()
        {
            // Clear all drawers
            foreach (var drawer in DrawerPipeline)
                drawer.Clear();
        }

        #endregion
    }
}