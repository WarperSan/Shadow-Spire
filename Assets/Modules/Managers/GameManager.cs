using System.Collections;
using Dungeon.Generation;
using Entities;
using UnityEngine;
using UtilsModule;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private void Start()
        {
            StartLevel();
        }

        public PlayerEntity player;


        #region Dungeon

        [Header("Dungeon")]
        [SerializeField]
        private DungeonManager dungeonManager;

        [SerializeField]
        private int levelIndex = 0;

        public int overSeed;
        public bool useSeed;

        public DungeonResult Level { get; private set; }
        public bool IsLevelOver { get; private set; }

        public void StartLevel()
        {
            Level = null;
            IsLevelOver = false;
            levelIndex++;

            int seed = useSeed ? overSeed : Random.Range(int.MinValue, int.MaxValue);
            overSeed = seed;

            var settings = new DungeonSettings
            {
                Seed = seed,
                Width = 18,
                Height = 12,
                SliceCount = 10
            };

            Debug.Log("Seed: " + seed);

            Level = dungeonManager.StartLevel(settings);
        }

        public void EndLevel()
        {
            IsLevelOver = true;
            StartCoroutine(dungeonManager.EndLevel(levelIndex, levelIndex + 1, new System.Func<IEnumerator>(EndLevelCallback)));
        }

        private IEnumerator EndLevelCallback()
        {
            StartLevel();
            yield return null;
        }

        #endregion

        #region Gizmos
#if UNITY_EDITOR

        [Header("Gizmos")]
        [SerializeField, Tooltip("Icon used for a node that is in the selected path")]
        private string SelectedPathIcon = "sv_icon_dot3_pix16_gizmo"; // Green dot

        [SerializeField, Tooltip("Icon used for a node that is not in the selected path")]
        private string NodeIcon = "sv_icon_dot1_pix16_gizmo"; // Blue dot

        [SerializeField, Tooltip("Gradient used for the links")]
        private Gradient LinkCostColor;

        /// <inheritdoc/>
        private void OnDrawGizmos()
        {
            if (Level == null || Level.TileGraph == null)
                return;

            var height = Level.Grid.GetLength(0);
            var width = Level.Grid.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var id = Level.TileGraph.GetID(x, y);

                    if (id == -1)
                        continue;

                    var n = Level.TileGraph.GetNode(id);

                    // Draw links
                    foreach (var neighbor in n.GetNeighbors())
                    {
                        Gizmos.color = LinkCostColor.Evaluate(n.GetCost(neighbor) / 3f);
                        Gizmos.DrawLine(n.Position, Level.TileGraph.GetNode(neighbor).Position);
                    }
                }
            }
        }

#endif
        #endregion
    }
}
