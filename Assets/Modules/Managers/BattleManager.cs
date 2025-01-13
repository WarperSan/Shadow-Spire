using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Minigame;
using Battle.Options;
using BattleEntity;
using Enemies;
using Entities;
using UnityEngine;
using Weapons;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        private EnemyEntity enemyEntity;
        private BattleEnemyEntity[] battleEnemyEntities;
        private BattlePlayerEntity battlePlayerEntity;
        private PlayerEntity playerEntity;

        #region Battle State

        private bool hasBattleEnded;

        public IEnumerator StartBattle(EnemyEntity enemyEntity, PlayerEntity playerEntity)
        {
            hasBattleEnded = false;

            // Initialize entities
            this.enemyEntity = enemyEntity;
            this.playerEntity = playerEntity;
            battlePlayerEntity = new BattlePlayerEntity(playerEntity);

            // Find all elements
            yield return FindBattleUI();

            yield return FindMinigameManager();

            // Load options
            LoadBattleOptions();
            LoadEnemyOptions(playerEntity.Weapon, GenerateEnemies(enemyEntity));

            // Disable spoiler
            yield return battleUI.DisableSpoiler();

            // Wait for spawn animation
            yield return new WaitForSeconds(1f);

            EnableBattleOption();
            AddInputs();
        }

        public void EndBattle(bool isVictory)
        {
            hasBattleEnded = true;
            StartCoroutine(EndBattleCoroutine(isVictory));
        }

        private IEnumerator EndBattleCoroutine(bool isVictory)
        {
            RemoveInputs();
            DisableBattleOption();
            DisableEnemyOption();

            yield return battleUI.EnableSpoiler();

            GameManager.Instance.EndBattle(isVictory, enemyEntity);
        }

        #endregion

        #region Battle UI

        private BattleUI battleUI;

        private IEnumerator FindBattleUI()
        {
            do
            {
                battleUI = FindObjectOfType<BattleUI>();
                yield return null;
            } while (battleUI == null);
        }

        #endregion

        #region Battle Options

        private bool isSelectingBattleOption;

        private void LoadBattleOptions()
        {
            battleUI.Load(new BattleOptionData[] {
                new() {
                    Text = "Attack",
                    OnEnter = OnAttackPressed,
                    IsValid = () => true
                },
                new() {
                    Text = "Heal",
                    OnEnter = () => StartCoroutine(OnHealPressed()),
                    IsValid = () => playerEntity.HasPotions()
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
            if (!playerEntity.HasPotions())
                yield break;

            yield return HealPlayer(50);

            RemoveInputs();
            DisableBattleOption();

            yield return EnemyTurn();
        }

        private void EnableBattleOption()
        {
            isSelectingBattleOption = true;
            battleUI.ShowSelection<BattleOptionData>();
        }

        private void DisableBattleOption()
        {
            isSelectingBattleOption = false;
            battleUI.HideSelection<BattleOptionData>();
        }

        #endregion

        #region Player

        private IEnumerator HealPlayer(int amount)
        {
            playerEntity.ConsumePotion();
            battlePlayerEntity.Heal(amount);

            yield return new WaitForSeconds(0.5f);
        }

        public void DamagePlayer(int amount)
        {
            battlePlayerEntity.TakeDamage(amount);

            if (battlePlayerEntity.IsDead)
                EndBattle(false);
        }

        #endregion

        #region Enemy Options

        private bool isSelectingEnemyOption;

        private void LoadEnemyOptions(WeaponInstance weapon, params BattleEnemyEntity[] entities)
        {
            battleEnemyEntities = entities;
            EnemyOptionData[] options = new EnemyOptionData[entities.Length];

            for (int i = 0; i < options.Length; i++)
            {
                options[i] = new EnemyOptionData()
                {
                    Weapon = weapon,
                    Entity = entities[i],
                    OnEnter = () => StartCoroutine(OnEnemyConfirmed()),
                    OnEscape = OnEnemyEscape
                };
            }

            battleUI.Load(options);
        }

        private BattleEnemyEntity[] GenerateEnemies(EnemyEntity enemy)
        {
            int level = GameManager.Instance.Level.Index;
            System.Random random = GameManager.Instance.Level.Random;

            List<BattleEnemyEntity> enemies = new();

            if (random.NextDouble() <= 0.9f && level - Dungeon.Generation.DungeonGenerator.ENEMY_ROOM_INDEX >= 2)
                enemies.Add(new BattleEnemyEntity(EnemyInstance.CreateRandom(level)));

            enemies.Add(new(enemy.Instance));

            if (random.NextDouble() <= 0.9f && level - Dungeon.Generation.DungeonGenerator.ENEMY_ROOM_INDEX >= 5)
                enemies.Add(new BattleEnemyEntity(EnemyInstance.CreateRandom(level)));

            return enemies.ToArray();
        }

        private IEnumerator OnEnemyConfirmed()
        {
            RemoveInputs();
            DisableEnemyOption();

            EnemyOption enemyOption = battleUI.GetSelection<EnemyOptionData, EnemyOption>();
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
            isSelectingEnemyOption = false;
            battleUI.HideSelection<EnemyOptionData>();

            EnableBattleOption();
        }

        private void EnableEnemyOption()
        {
            isSelectingEnemyOption = true;
            battleUI.ShowSelection<EnemyOptionData>();
        }

        private void DisableEnemyOption()
        {
            isSelectingEnemyOption = false;
            battleUI.HideSelection<EnemyOptionData>();
        }

        private bool VerifyEnemiesState()
        {
            for (int i = 0; i < battleEnemyEntities.Length; i++)
            {
                // If at least one enemy alive
                if (!battleEnemyEntities[i].IsDead)
                    return true;
            }

            // If all enemies dead
            return false;
        }

        private IEnumerator EnemyTurn()
        {
            // Set up
            minigameManager.SetupProjectiles(battleEnemyEntities, battlePlayerEntity, this);

            // Disable Player BattleUI
            yield return battleUI.StartEnemyTurn(playerEntity.playerInformation);

            // Execute enemy attacks
            yield return minigameManager.SpawnProjectiles();

            if (hasBattleEnded)
                yield break;

            // Enable Player BattleUI
            yield return battleUI.EndEnemyTurn(playerEntity.playerInformation);

            // End attacks
            minigameManager.CleanProjectiles();

            EnableBattleOption();
            AddInputs();
        }

        #endregion

        #region Minigame

        private MinigameManager minigameManager;

        private IEnumerator FindMinigameManager()
        {
            do
            {
                minigameManager = FindObjectOfType<MinigameManager>(true);
                yield return null;
            } while (minigameManager == null);
        }

        #endregion

        #region Inputs

        private void Move(Vector2 dir)
        {
            if (isSelectingBattleOption)
            {
                battleUI.Move<BattleOptionData>(dir);
                return;
            }

            if (isSelectingEnemyOption)
            {
                battleUI.Move<EnemyOptionData>(dir);
                return;
            }
        }

        private void Enter()
        {
            if (isSelectingBattleOption)
            {
                battleUI.Enter<BattleOptionData>();
                return;
            }

            if (isSelectingEnemyOption)
            {
                battleUI.Enter<EnemyOptionData>();
                return;
            }
        }

        private void Escape()
        {
            if (isSelectingBattleOption)
            {
                battleUI.Escape<BattleOptionData>();
                return;
            }

            if (isSelectingEnemyOption)
            {
                battleUI.Escape<EnemyOptionData>();
                return;
            }
        }

        /// <summary>
        /// Adds the inputs for this object
        /// </summary>
        private void AddInputs()
        {
            InputManager.Instance.OnMoveUI.AddListener(Move);
            InputManager.Instance.OnEnterUI.AddListener(Enter);
            InputManager.Instance.OnEscapeUI.AddListener(Escape);
        }

        /// <summary>
        /// Removes the inputs for this object
        /// </summary>
        private void RemoveInputs()
        {
            InputManager.Instance.OnMoveUI.RemoveListener(Move);
            InputManager.Instance.OnEnterUI.RemoveListener(Enter);
            InputManager.Instance.OnEscapeUI.RemoveListener(Escape);
        }

        #endregion
    }
}