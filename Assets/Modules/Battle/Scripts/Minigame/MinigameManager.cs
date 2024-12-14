using System.Collections;
using BattleEntity;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Battle.Minigame
{
    public class MinigameManager : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private GameObject projectile;

        [SerializeField]
        private PlayerBattleMovement player;

        [SerializeField]
        private Transform projectilesParent;

        [SerializeField]
        private Animator animator;

        #endregion

        #region Data

        private BattleEntityType[] enemyTypes;
        private BattlePlayerEntity playerEntity;

        #endregion

        #region Manager

        public void SetupProjectiles(BattleEnemyEntity[] battleEnemyEntities, BattlePlayerEntity playerEntity, BattleManager battleManager)
        {
            player.battleManager = battleManager;
            this.playerEntity = playerEntity;

            if (battleEnemyEntities == null || battleEnemyEntities.Length == 0)
                return;

            var enemyCount = 0;

            // Compile types
            var combinedType = BattleEntityType.NONE;

            foreach (var enemy in battleEnemyEntities)
            {
                if (!enemy.IsDead)
                {
                    enemyCount++;
                    combinedType |= enemy.Type;
                }
            }

            enemyTypes = BattleEntity.BattleEntity.GetTypes(combinedType);

            // Calculate how many projectiles to spawn per seconds

            spawnInterval = 0.3f / enemyCount;
        }

        public IEnumerator SpawnProjectiles(float duration)
        {
            InputManager.Instance.SwitchToMiniGame();
            InputManager.Instance.OnMoveMinigame.AddListener(Move);
            player.canTakeDamage = true;

            while (duration > 0 && !playerEntity.IsDead)
            {
                var rdmType = enemyTypes[Random.Range(0, enemyTypes.Length)];
                SpawnSingleProjectile(rdmType);

                duration -= spawnInterval;
                yield return new WaitForSeconds(spawnInterval);
            }

            player.canTakeDamage = false;
            InputManager.Instance.OnMoveMinigame.RemoveListener(Move);
            InputManager.Instance.SwitchToUI();
        }

        public void CleanProjectiles()
        {
            // Destroy all projectiles
            foreach (Transform child in projectilesParent)
                Destroy(child.gameObject);

            // Remove all the compiled data
            enemyTypes = null;
            playerEntity = null;
            spawnInterval = 1f;
        }

        #endregion

        #region Projectiles

        private float spawnInterval = 0.015f;
        private float minX = -0.6f;
        private float maxX = 0.6f;
        private float minSpawnDistance = 0.5f;
        private float lastSpawnX = 0.818f;

        private void SpawnSingleProjectile(BattleEntityType type)
        {
            float randomX;
            do
            {
                randomX = Random.Range(minX, maxX);
            } while (Mathf.Abs(randomX - lastSpawnX) < minSpawnDistance);

            lastSpawnX = randomX;

            GameObject newProjectile = Instantiate(projectile);
            newProjectile.transform.SetParent(projectilesParent);
            newProjectile.transform.localPosition = new(randomX, 0, 0);

            if (newProjectile.TryGetComponent(out MinigameProjectile minigameProjectile))
                minigameProjectile.SetColor(BattleEntity.BattleEntity.GetTypeColor(type));
        }

        #endregion

        #region Animations

        public IEnumerator ShowIn()
        {
            animator.SetBool("show", true);
            yield return new WaitForSeconds(1f + 0.2f);
        }

        public IEnumerator HideOut()
        {
            animator.SetBool("show", false);
            yield return new WaitForSeconds(0.5f + 0.2f);
        }

        #endregion

        #region Inputs

        public void Move(Vector2 dir) => player.Move(dir);

        #endregion
    }
}