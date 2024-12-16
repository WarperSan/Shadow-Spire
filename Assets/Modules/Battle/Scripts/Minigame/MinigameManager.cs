using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Minigame.Spawners;
using BattleEntity;
using Managers;
using Player;
using UnityEngine;

namespace Battle.Minigame
{
    public class MinigameManager : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private GameObject projectile;

        [SerializeField]
        private MinigamePlayer player;

        [SerializeField]
        private Spawner[] spawners;

        [SerializeField]
        private Transform projectilesParent;

        [SerializeField]
        private Animator animator;

        #endregion

        #region Data

        private Dictionary<BattleEntity.Type, int> typesCount = new();
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
            foreach (BattleEntity.Type item in Enum.GetValues(typeof(BattleEntity.Type)))
                typesCount[item] = 0;

            foreach (var enemy in battleEnemyEntities)
            {
                // Ignore dead enemies
                if (enemy.IsDead)
                    continue;

                foreach (var uniqueType in BattleEntity.BattleEntity.GetTypes(enemy.Type))
                    typesCount[uniqueType]++;

                enemyCount++;
            }

            // Set up spawners
            foreach (var spawner in spawners)
            {
                var strength = typesCount[spawner.HandledType];

                if (strength > 0)
                {
                    spawner.enabled = true;
                    spawner.Setup(3);
                }
                else
                    spawner.enabled = false;
            }

            // Calculate how many projectiles to spawn per seconds
            spawnInterval = 0.3f / enemyCount;
        }

        public IEnumerator SpawnProjectiles(float duration)
        {
            InputManager.Instance.SwitchToMiniGame();
            InputManager.Instance.OnMoveMinigame.AddListener(Move);
            player.canTakeDamage = true;

            // while (duration > 0 && !playerEntity.IsDead)
            // {
            //     var rdmType = BattleEntity.Type.NORMAL;
            //     SpawnSingleProjectile(rdmType);

            //     duration -= spawnInterval;
            //     yield return new WaitForSeconds(spawnInterval);
            // }

            var spawnerCoroutines = new List<Coroutine>();

            foreach (var spawner in spawners)
            {
                if (!spawner.enabled)
                    continue;

                spawnerCoroutines.Add(StartCoroutine(spawner.StartSpawn(duration)));
            }

            yield return new WaitForSeconds(duration);

            foreach (var item in spawnerCoroutines)
                yield return item;

            player.canTakeDamage = false;
            InputManager.Instance.OnMoveMinigame.RemoveListener(Move);
            InputManager.Instance.SwitchToUI();
        }

        public void CleanProjectiles()
        {
            foreach (var spawner in spawners)
            {
                if (!spawner.enabled)
                    continue;

                spawner.Clean();
                spawner.enabled = false;
            }

            // Destroy all projectiles
            //foreach (Transform child in projectilesParent)
            //    Destroy(child.gameObject);

            // Remove all the compiled data
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

        private void SpawnSingleProjectile(BattleEntity.Type type)
        {
            float randomX;
            do
            {
                randomX = UnityEngine.Random.Range(minX, maxX);
            } while (Mathf.Abs(randomX - lastSpawnX) < minSpawnDistance);

            lastSpawnX = randomX;

            GameObject newProjectile = Instantiate(projectile);
            newProjectile.transform.SetParent(projectilesParent);
            newProjectile.transform.localPosition = new(randomX, 0, 0);

            if (newProjectile.TryGetComponent(out Projectiles.Projectile minigameProjectile))
                minigameProjectile.SetEnemy(type);
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