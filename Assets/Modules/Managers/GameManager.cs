using System.Collections;
using Dungeon.Generation;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UtilsModule;
using Weapons;

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

        [SerializeField]
        private Camera dungeonCamera;

        public int overSeed;
        public bool useSeed;

        public DungeonResult Level { get; private set; }
        public bool IsLevelOver { get; private set; }

        public void StartLevel()
        {
            Level = null;
            IsLevelOver = false;

            int seed = useSeed ? overSeed : Random.Range(int.MinValue, int.MaxValue);
            overSeed = seed;

            var settings = new DungeonSettings
            {
                Index = levelIndex,
                Seed = seed,
                Width = Mathf.Min(4 + levelIndex * 2, 18),
                Height = Mathf.Min(4 + levelIndex * 2, 12),
                MinimumRoomHeight = levelIndex <= 1 ? 2 : 3,
                MinimumRoomWidth = levelIndex <= 1 ? 2 : 3,
                SliceCount = Mathf.FloorToInt(levelIndex * 1.2f),
                AddHighLoop = levelIndex >= 5,
                AddLowLoop = levelIndex >= 5
            };

            Debug.Log("Seed: " + seed);

            Level = dungeonManager.StartLevel(settings, player);
            dungeonCamera.transform.position = new Vector3(
                Level.Width / 2f + 0.5f,
                -(Level.Height / 2f + 0.5f),
                dungeonCamera.transform.position.z
            );
        }

        public void EndLevel()
        {
            IsLevelOver = true;
            levelIndex++;
            StartCoroutine(dungeonManager.EndLevel(levelIndex, levelIndex + 1, new System.Func<IEnumerator>(EndLevelCallback)));
        }

        private IEnumerator EndLevelCallback()
        {
            StartLevel();
            yield return null;
        }

        #endregion

        #region Battle

        [Header("Battle")]
        [SerializeField]
        private BattleManager battleManager;

        [SerializeField]
        private Image endBattleBackground;

        [SerializeField]
        private TextMeshProUGUI diedText;

        public bool IsInBattle { get; private set; }

        public void StartBattle(EnemyEntity enemy)
        {
            if (IsInBattle)
                return;

            IsInBattle = true;
            StartCoroutine(battleManager.StartBattle());
            InputManager.Instance.SwitchToUI();
        }

        public void EndBattle(bool isVictory) // S'OCCUPE DE GERER LE END BATTLE DEPENDEMENT DU SCENARIO
        {
            IsInBattle = false;
            
            if(!isVictory)
            {
                endBattleBackground.gameObject.SetActive(true);
                StartCoroutine(battleManager.DeadPlayerTextFadeIn(diedText));
            }
            SceneManager.UnloadSceneAsync("BattleScene");
            InputManager.Instance.SwitchToPlayer();
        }

        #endregion

        #region Weapons

        [Header("Weapons")]
        public WeaponSO[] allWeapons;

        #endregion

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}
