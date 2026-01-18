using System.Collections;
using System.Collections.Generic;
using BattleEntity;
using Enemies;
using Enemies.UI;
using GridEntities.Entities;
using UI.Battle;
using UnityEngine;
using Weapons;

namespace Managers
{
	public class BattleManager : MonoBehaviour
	{
		private EnemyEntity _enemyEntity;
		private BattleEnemyEntity[] _battleEnemyEntities;
		private BattlePlayerEntity _battlePlayerEntity;
		private PlayerEntity _playerEntity;

		#region Battle State

		private bool _hasBattleEnded;

		public IEnumerator StartBattle(EnemyEntity enemyEntity, PlayerEntity playerEntity)
		{
			_hasBattleEnded = false;

			// Initialize entities
			_enemyEntity = enemyEntity;
			_playerEntity = playerEntity;
			_battlePlayerEntity = new BattlePlayerEntity(playerEntity);

			// Find all elements
			yield return FindBattleUI();
			yield return FindMinigameManager();

			// Load options
			LoadBattleOptions();
			LoadEnemyOptions(playerEntity.Weapon, GenerateEnemies(enemyEntity));

			// Disable spoiler
			yield return _battleUI.DisableSpoiler();

			// Wait for spawn animation
			yield return new WaitForSeconds(1f);

			EnableBattleOption();
			AddInputs();
		}

		public void EndBattle(bool isVictory)
		{
			_hasBattleEnded = true;
			StartCoroutine(EndBattleCoroutine(isVictory));
		}

		private IEnumerator EndBattleCoroutine(bool isVictory)
		{
			RemoveInputs();
			DisableBattleOption();
			DisableEnemyOption();

			yield return _battleUI.EnableSpoiler();

			GameManager.Instance.EndBattle(isVictory, _enemyEntity);
		}

		#endregion

		#region Battle UI

		private BattleUI _battleUI;

		private IEnumerator FindBattleUI()
		{
			do
			{
				_battleUI = FindAnyObjectByType<BattleUI>();
				yield return null;
			} while (_battleUI == null);
		}

		#endregion

		#region Battle Options

		private bool _isSelectingBattleOption;

		private void LoadBattleOptions()
		{
			_battleUI.Load(new BattleOptionData[]
			{
				new()
				{
					Text = "Attack",
					OnEnter = OnAttackPressed,
					IsValid = () => true
				},
				new()
				{
					Text = "Heal",
					OnEnter = () => StartCoroutine(OnHealPressed()),
					IsValid = () => _playerEntity.HasPotions()
				},
				#if UNITY_EDITOR
				// new()
				// {
				//    Text = "Nuke",
				//    OnEnter = () => EndBattle(true)
				// },
				// new()
				// {
				//    Text = "Death",
				//    OnEnter = () => EndBattle(false)
				// }
				#endif
			});
		}

		private void OnAttackPressed()
		{
			DisableBattleOption();
			EnableEnemyOption();
		}

		private IEnumerator OnHealPressed()
		{
			if (!_playerEntity.HasPotions())
				yield break;

			yield return HealPlayer(50);

			RemoveInputs();
			DisableBattleOption();

			yield return EnemyTurn();
		}

		private void EnableBattleOption()
		{
			_isSelectingBattleOption = true;
			_battleUI.ShowSelection<BattleOptionData>();
		}

		private void DisableBattleOption()
		{
			_isSelectingBattleOption = false;
			_battleUI.HideSelection<BattleOptionData>();
		}

		#endregion

		#region Player

		private IEnumerator HealPlayer(int amount)
		{
			_playerEntity.ConsumePotion();
			_battlePlayerEntity.Heal(amount);

			yield return new WaitForSeconds(0.5f);
		}

		public void DamagePlayer(int amount)
		{
			_battlePlayerEntity.TakeDamage(amount);

			if (_battlePlayerEntity.IsDead)
				EndBattle(false);
		}

		#endregion

		#region Enemy Options

		private bool _isSelectingEnemyOption;

		private void LoadEnemyOptions(WeaponInstance weapon, params BattleEnemyEntity[] entities)
		{
			_battleEnemyEntities = entities;
			EnemyOptionData[] options = new EnemyOptionData[entities.Length];

			for (int i = 0; i < options.Length; i++)
			{
				options[i] = new EnemyOptionData
				{
					Weapon = weapon,
					Entity = entities[i],
					OnEnter = () => StartCoroutine(OnEnemyConfirmed()),
					OnEscape = OnEnemyEscape
				};
			}

			_battleUI.Load(options);
		}

		private BattleEnemyEntity[] GenerateEnemies(EnemyEntity enemy)
		{
			int level = GameManager.Instance.Level.Index;
			System.Random random = GameManager.Instance.Level.Random;

			List<BattleEnemyEntity> enemies = new();

			if (random.NextDouble() <= 0.9f && level - Dungeon.Generation.DungeonGenerator.ENEMY_ROOM_INDEX >= 2)
				enemies.Add(new BattleEnemyEntity(EnemyInstance.CreateRandom(level)));

			enemies.Add(new BattleEnemyEntity(enemy.Instance));

			if (random.NextDouble() <= 0.9f && level - Dungeon.Generation.DungeonGenerator.ENEMY_ROOM_INDEX >= 5)
				enemies.Add(new BattleEnemyEntity(EnemyInstance.CreateRandom(level)));

			return enemies.ToArray();
		}

		private IEnumerator OnEnemyConfirmed()
		{
			RemoveInputs();
			DisableEnemyOption();

			EnemyOption enemyOption = _battleUI.GetSelection<EnemyOptionData, EnemyOption>();
			EnemyOptionData enemy = enemyOption.GetOption();

			enemy.Entity.TakeAttack(enemy.Weapon);

			yield return new WaitForSeconds(0.7f);

			// All enemies dead, victory
			if (!VerifyEnemiesState())
			{
				EndBattle(true);
				yield break;
			}

			yield return EnemyTurn();
		}

		private void OnEnemyEscape()
		{
			_isSelectingEnemyOption = false;
			_battleUI.HideSelection<EnemyOptionData>();

			EnableBattleOption();
		}

		private void EnableEnemyOption()
		{
			_isSelectingEnemyOption = true;
			_battleUI.ShowSelection<EnemyOptionData>();
		}

		private void DisableEnemyOption()
		{
			_isSelectingEnemyOption = false;
			_battleUI.HideSelection<EnemyOptionData>();
		}

		private bool VerifyEnemiesState()
		{
			for (int i = 0; i < _battleEnemyEntities.Length; i++)
			{
				// If at least one enemy alive
				if (!_battleEnemyEntities[i].IsDead)
					return true;
			}

			// If all enemies dead
			return false;
		}

		private IEnumerator EnemyTurn()
		{
			// Set up
			_minigameManager.SetupProjectiles(_battleEnemyEntities, _battlePlayerEntity, this);

			// Disable Player BattleUI
			yield return _battleUI.StartEnemyTurn(_playerEntity.playerInformation);

			// Execute enemy attacks
			yield return _minigameManager.SpawnProjectiles();

			if (_hasBattleEnded)
				yield break;

			// Enable Player BattleUI
			yield return _battleUI.EndEnemyTurn(_playerEntity.playerInformation);

			// End attacks
			_minigameManager.CleanProjectiles();

			EnableBattleOption();
			AddInputs();
		}

		#endregion

		#region Minigame

		private MinigameManager _minigameManager;

		private IEnumerator FindMinigameManager()
		{
			do
			{
				_minigameManager = FindAnyObjectByType<MinigameManager>(FindObjectsInactive.Include);
				yield return null;
			} while (_minigameManager == null);
		}

		#endregion

		#region Inputs

		private void Move(Vector2 dir)
		{
			if (_isSelectingBattleOption)
			{
				_battleUI.Move<BattleOptionData>(dir);
				return;
			}

			if (_isSelectingEnemyOption)
			{
				_battleUI.Move<EnemyOptionData>(dir);
				return;
			}
		}

		private void Enter()
		{
			if (_isSelectingBattleOption)
			{
				_battleUI.Enter<BattleOptionData>();
				return;
			}

			if (_isSelectingEnemyOption)
			{
				_battleUI.Enter<EnemyOptionData>();
				return;
			}
		}

		private void Escape()
		{
			if (_isSelectingBattleOption)
			{
				_battleUI.Escape<BattleOptionData>();
				return;
			}

			if (_isSelectingEnemyOption)
			{
				_battleUI.Escape<EnemyOptionData>();
				return;
			}
		}

		/// <summary>
		/// Adds the inputs for this object
		/// </summary>
		private void AddInputs()
		{
			InputManager.Instance.onMoveUI.AddListener(Move);
			InputManager.Instance.onEnterUI.AddListener(Enter);
			InputManager.Instance.onEscapeUI.AddListener(Escape);
		}

		/// <summary>
		/// Removes the inputs for this object
		/// </summary>
		private void RemoveInputs()
		{
			InputManager.Instance.onMoveUI.RemoveListener(Move);
			InputManager.Instance.onEnterUI.RemoveListener(Enter);
			InputManager.Instance.onEscapeUI.RemoveListener(Escape);
		}

		#endregion
	}
}