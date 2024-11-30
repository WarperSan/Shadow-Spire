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

        public int overSeed;
        public bool useSeed;

        public DungeonResult Level { get; private set; }
        public bool IsLevelOver { get; private set; }

        public void StartLevel()
        {
            Level = null;
            IsLevelOver = false;

            int seed = useSeed ? overSeed : Random.Range(int.MinValue, int.MaxValue);

            Debug.Log("Seed: " + seed);
            Level = dungeonManager.StartLevel(seed);
        }

        public void EndLevel()
        {
            IsLevelOver = true;
            StartCoroutine(dungeonManager.EndLevel(1, 2, new System.Func<IEnumerator>(EndLevelCallback)));
        }

        private IEnumerator EndLevelCallback()
        {
            StartLevel();
            yield return null;
        }

        #endregion
    }
}
