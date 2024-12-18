using System.Collections;
using Battle.Minigame.Projectiles;
using BattleEntity;
using UnityEngine;

namespace Battle.Minigame.Spawners
{
    public class Animal_Spawner : Spawner
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private GameObject shrimpPrefab;

        [SerializeField]
        private Transform projectileParent;

        #endregion

        #region Data

        private float spawnDelay = -1;

        #endregion

        #region Spawner

        private const float SPAWN_X = -3;
        private const float MIN_SPAWN_Y = -2.25f;
        private const float MAX_SPAWN_Y = 2.25f;

        /// <inheritdoc/>
        public override Type HandledType => Type.ANIMAL;

        /// <inheritdoc/>
        public override void Clean()
        {
            foreach (Transform item in projectileParent)
                Destroy(item.gameObject);

            spawnDelay = -1;
        }

        /// <inheritdoc/>
        public override void Setup(int strength)
        {
            spawnDelay = Mathf.Max(1.25f / strength, 0.01f);
        }

        /// <inheritdoc/>
        public override IEnumerator StartSpawn(float duration)
        {
            while (duration > 0)
            {
                var newProjectile = Instantiate(shrimpPrefab, projectileParent);
                newProjectile.transform.localPosition = new Vector2(
                    SPAWN_X,
                    Random.Range(MIN_SPAWN_Y, MAX_SPAWN_Y)
                );

                if (newProjectile.TryGetComponent(out Projectile projectile))
                    projectile.SetEnemy(HandledType);

                yield return new WaitForSeconds(spawnDelay);

                duration -= spawnDelay;
            }
        }

        #endregion
    }
}