using System.Collections;
using Dungeon.Generation;
using Enemies;
using GridEntities.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Weapons;
using Weapons.UI;

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

		public void Defeat() => StartCoroutine(DeathSequence(true));

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
		private int levelIndex = 1;

		[SerializeField]
		private Camera dungeonCamera;

		public int overSeed;
		public bool useSeed;

		public DungeonResult Level       { get; private set; }
		public bool          IsLevelOver { get; private set; }

		public void StartLevel()
		{
			Level = null;
			IsLevelOver = false;

			int seed = useSeed ? overSeed : Random.Range(int.MinValue, int.MaxValue);
			overSeed = seed;

			DungeonSettings settings = new()
			{
				Index = levelIndex,
				Seed = seed,
				Width = Mathf.Min(4 + (levelIndex - 1) * 2, 17),
				Height = Mathf.Min(4 + (levelIndex - 1) * 2, 8),
				MinimumRoomHeight = levelIndex <= 2 ? 2 : 3,
				MinimumRoomWidth = levelIndex <= 2 ? 2 : 3,
				SliceCount = Mathf.FloorToInt(levelIndex * 1.2f),
				AddHighLoop = levelIndex % 10 == 0,
				AddLowLoop = levelIndex % 4 == 0
			};

			Debug.Log("Seed: " + seed);

			Level = dungeonManager.GenerateLevel(settings, player);
			dungeonManager.StartLevel(Level);

			dungeonCamera.transform.position = new Vector3(
				Level.Width / 2f,
				-(Level.Height / 2f + 0.75f),
				dungeonCamera.transform.position.z
			);

			StartCoroutine(StartLevelSequence());
		}

		public IEnumerator StartLevelSequence()
		{
			UIManager.SetLevel(levelIndex);

			yield return UIManager.FadeOutBlackout();
			yield return new WaitForSeconds(0.2f);

			InputManager.Instance.SwitchToPlayer();
		}

		public void EndLevel()
		{
			IsLevelOver = true;
			InputManager.Instance.SwitchToUI();
			StartCoroutine(EndLevelSequence());
		}

		private IEnumerator EndLevelSequence()
		{
			yield return new WaitForSeconds(0.2f);

			yield return UIManager.FadeInBlackout();

			dungeonManager.ClearDungeon();
			yield return null; // Wait 1 frame

			yield return UIManager.ShowNextLevel(levelIndex, levelIndex + 1);

			levelIndex++;

			yield return EndLevelWeaponOffer();

			StartLevel();
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

			AsyncOperation battle = SceneManager.UnloadSceneAsync("BattleScene");

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
			if (levelIndex % 5 != 0)
				yield break;

			yield return weaponUI.ShowWeapons();
		}

		#endregion

		#region Scenes

		public IEnumerator ReturnToTitle()
		{
			dungeonManager.ClearDungeon();
			yield return null;

			AsyncOperation title = SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Single);

			while (!title.isDone)
				yield return null;

			AsyncOperation gameDeload = SceneManager.UnloadSceneAsync("Game");

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