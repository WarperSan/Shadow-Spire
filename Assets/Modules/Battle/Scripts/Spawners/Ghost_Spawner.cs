using System.Collections;
using Battle.Projectiles;
using BattleEntity;
using UnityEngine;

namespace Battle.Spawners
{
    public class Ghost_Spawner : Spawner
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private GameObject ghostPrefab;

        [SerializeField]
        private Transform projectileParent;

        [SerializeField]
        private Transform player;

        #endregion

        #region Data

        private float cooldown;
        private float speed;
        private int count;

        #endregion

        #region Spawner

        private const float MIN_X = -3f;
        private const float MIN_Y = -3f;
        private const float MAX_X = 3f;
        private const float MAX_Y = 3f;

        /// <inheritdoc/>
        public override Type HandledType => Type.GHOST;

        /// <inheritdoc/>
        public override void Clean()
        {
            foreach (Transform item in projectileParent)
                Destroy(item.gameObject);
        }

        /// <inheritdoc/>
        public override void Setup(int strength)
        {
            cooldown = 2f / strength;
            speed = 0.75f * strength;
            count = strength / 3 + 1;
        }

        /// <inheritdoc/>
        public override IEnumerator StartSpawn(float duration)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject ghost = Instantiate(ghostPrefab, projectileParent);
                ghost.transform.localPosition = new Vector2(
                    Random.value <= 0.5f ? MIN_X : MAX_X,
                    Random.value <= 0.5f ? MIN_Y : MAX_Y
                );

                if (ghost.TryGetComponent(out GhostProjectile projectile))
                {
                    projectile.SetEnemy(HandledType);
                    projectile.Player = player;
                    projectile.Cooldown = cooldown;
                    projectile.Speed = speed;
                }

                yield return new WaitForSeconds(duration / count);
            }
        }

        #endregion
    }
}