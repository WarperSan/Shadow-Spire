using System.Collections;
using Dungeon.Generation;
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
    }
}