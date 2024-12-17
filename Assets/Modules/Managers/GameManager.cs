using System.Collections;
using Dungeon.Generation;
using Enemies;
using Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsModule;
using Weapons;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private void Start()
        {
            WeaponInstance.WEAPONS = Resources.LoadAll<WeaponSO>("Weapons");
            EnemyInstance.ENEMIES = Resources.LoadAll<EnemySO>("Enemies");

            StartLevel();
        }

        #region Player

        public PlayerEntity player;
        public bool IsPlayerDead;

        public void Defeat()
        {
            StartCoroutine(DeathSequence(true));
        }

        private IEnumerator DeathSequence(bool fromOverworld)
        {
            yield return UIManager.DeathSequence(Level.Index + 1, fromOverworld);
            yield return ReturnToTitle();
        }

        #endregion

        #region UI

        [Header("UI")]
        public UIManager UIManager;

        #endregion

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
                Width = Mathf.Min(4 + levelIndex * 2, 17),
                Height = Mathf.Min(4 + levelIndex * 2, 8),
                MinimumRoomHeight = levelIndex <= 1 ? 2 : 3,
                MinimumRoomWidth = levelIndex <= 1 ? 2 : 3,
                SliceCount = Mathf.FloorToInt(levelIndex * 1.2f),
                AddHighLoop = levelIndex >= 5,
                AddLowLoop = levelIndex >= 5
            };

            Debug.Log("Seed: " + seed);

            Level = dungeonManager.GenerateLevel(settings, player);
            dungeonManager.StartLevel(Level);

            dungeonCamera.transform.position = new Vector3(
                Level.Width / 2f,
                -(Level.Height / 2f + 1f),
                dungeonCamera.transform.position.z
            );
            InputManager.Instance.SwitchToPlayer();
        }

        public void EndLevel()
        {
            IsLevelOver = true;
            levelIndex++;
            InputManager.Instance.SwitchToUI();
            StartCoroutine(EndLevelSequence());
        }

        private IEnumerator EndLevelSequence()
        {
            yield return dungeonManager.EndLevel(levelIndex, levelIndex + 1);

            yield return EndLevelWeaponOffer();

            StartLevel();

            yield return UIManager.FadeOutBlackout();

            yield return new WaitForSeconds(0.2f);
        }

        #endregion

        #region Turn

        [Header("Turn")]
        [SerializeField]
        private TurnManager turnManager;

        #endregion

        #region Battle

        [Header("Battle")]
        [SerializeField]
        private BattleManager battleManager;

        public bool IsInBattle { get; private set; }

        public void StartBattle(EnemyEntity enemyEntity, PlayerEntity playerEntity)
        {
            if (IsInBattle)
                return;

            IsInBattle = true;
            InputManager.Instance.SwitchToUI();
            StartCoroutine(StartBattleCoroutine(enemyEntity, playerEntity));
        }

        public void EndBattle(bool isVictory, EnemyEntity enemy)
        {
            IsInBattle = false;

            StartCoroutine(EndBattleCoroutine(isVictory, enemy));
        }

        private IEnumerator StartBattleCoroutine(EnemyEntity enemyEntity, PlayerEntity playerEntity)
        {
            yield return UIManager.StartBattleTransition();
            yield return battleManager.StartBattle(enemyEntity, playerEntity);
        }

        private IEnumerator EndBattleCoroutine(bool isVictory, EnemyEntity enemy)
        {
            yield return UIManager.FadeInBlackout(1, 0);

            var battle = SceneManager.UnloadSceneAsync("BattleScene");

            while (!battle.isDone)
                yield return null;

            if (!isVictory)
            {
                yield return DeathSequence(false);
                yield break;
            }

            Destroy(enemy.gameObject);

            yield return new WaitForSeconds(1f);

            yield return UIManager.FadeOutBlackout();

            turnManager.StartTurn();
            InputManager.Instance.SwitchToPlayer();
        }

        #endregion

        #region Weapons

        [Header("Weapons")]
        [SerializeField]
        private WeaponSelectionUI weaponUI;

        private IEnumerator EndLevelWeaponOffer()
        {
            if (Level.Index < DungeonGenerator.WEAPON_INDEX)
                yield return null;
            else
                yield return weaponUI.ShowWeapons();
        }

        #endregion

        #region Scenes

        public IEnumerator ReturnToTitle()
        {
            dungeonManager.ClearDungeon();
            yield return null;

            var title = SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Single);

            while (!title.isDone)
                yield return null;

            var gameDeload = SceneManager.UnloadSceneAsync("Game");

            while (!gameDeload.isDone)
                yield return null;
        }


        #endregion

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}
