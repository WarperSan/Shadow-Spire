using System.Collections;
using Battle;
using Battle.Options;
using BattleEntity;
using Enemies;
using UnityEngine;
using Weapons;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        public WeaponSO weapon;
        public EnemySO enemy1;
        public EnemySO enemy2;
        public EnemySO enemy3;

        public IEnumerator StartBattle()
        {
            yield return StartBattleTransition();
            yield return FindBattleUI();
            LoadBattleOptions();

            var entities = new BattleEnemyEntity[3]
            {
                new(enemy1),
                new(enemy2),
                new(enemy3),
            };

            LoadEnemyOptions(weapon, entities);

            yield return battleUI.DisableSpoiler();

            EnableBattleOption();
            AddInputs();
        }

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
                    OnEnter = OnAttackPressed
                },
                new() {
                    Text = "Heal"
                }
            });
        }

        private void OnAttackPressed()
        {
            DisableBattleOption();
            EnableEnemyOption();
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

        #region Enemy Options

        private bool isSelectingEnemyOption;

        private void LoadEnemyOptions(WeaponSO weapon, params BattleEnemyEntity[] entities)
        {
            var options = new EnemyOptionData[entities.Length];

            for (int i = 0; i < options.Length; i++)
            {
                options[i] = new EnemyOptionData()
                {
                    Weapon = weapon,
                    Entity = entities[i],
                    OnEnter = OnEnemyConfirmed,
                    OnEscape = OnEnemyEscape
                };
            }

            battleUI.Load(options);
        }

        private void OnEnemyConfirmed()
        {
            RemoveInputs();
            DisableEnemyOption();

            var enemyOption = battleUI.GetSelection<EnemyOptionData, EnemyOption>();
            var enemy = enemyOption.GetOption();

            enemy.Entity.TakeAttack(enemy.Weapon);

            EnableBattleOption();
            AddInputs();
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

        #endregion

        #region Battle Transition

        [Header("Battle Transition")]
        [SerializeField]
        private Material transitionMaterial;

        [SerializeField]
        private Texture[] transitionTextures;

        private IEnumerator StartBattleTransition() => BattleTransition.ExecuteTransition(transitionMaterial, transitionTextures, 0.9f);

#if UNITY_EDITOR
        // For keeping consistency in editor
        private void OnApplicationQuit() => BattleTransition.ResetMaterial(transitionMaterial);
#endif

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