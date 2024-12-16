using System.Collections;
using Battle.Minigame.Projectiles;
using BattleEntity;
using UnityEngine;
using UtilsModule;

namespace Battle.Minigame.Spawners
{
    public class Normal_Spawner : Spawner
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private SpikeWallProjectile leftWall;

        [SerializeField]
        private SpikeWallProjectile rightWall;

        [SerializeField]
        private SpikeWallProjectile topWall;

        [SerializeField]
        private SpikeWallProjectile bottomWall;

        #endregion

        #region Spawner

        private const float OFF_WALL = 3.5f;
        private const float ON_WALL = 2.5f;

        /// <inheritdoc/>
        public override Type HandledType => Type.NORMAL;

        /// <inheritdoc/>
        public override void Clean()
        {
            leftWall.gameObject.SetActive(false);
            var leftPos = leftWall.transform.localPosition;
            leftPos.x = -OFF_WALL;
            leftWall.transform.localPosition = leftPos;

            rightWall.gameObject.SetActive(false);
            var rightPos = rightWall.transform.localPosition;
            rightPos.x = OFF_WALL;
            rightWall.transform.localPosition = rightPos;

            topWall.gameObject.SetActive(false);
            var topPos = topWall.transform.localPosition;
            topPos.y = OFF_WALL;
            topWall.transform.localPosition = topPos;

            bottomWall.gameObject.SetActive(false);
            var bottomPos = bottomWall.transform.localPosition;
            bottomPos.y = -OFF_WALL;
            bottomWall.transform.localPosition = bottomPos;
        }

        /// <inheritdoc/>
        public override void Setup(int strength)
        {
            bottomWall.gameObject.SetActive(strength >= 1);
            bottomWall.SetEnemy(Type.NORMAL);

            topWall.gameObject.SetActive(strength >= 2);
            topWall.SetEnemy(Type.NORMAL);
            
            leftWall.gameObject.SetActive(strength >= 3);
            leftWall.SetEnemy(Type.NORMAL);

            rightWall.gameObject.SetActive(strength >= 3);
            rightWall.SetEnemy(Type.NORMAL);
        }

        /// <inheritdoc/>
        public override IEnumerator StartSpawn(float duration)
        {
            var parallel = new Coroutine[]
            {
                StartCoroutine(TranslateTo(bottomWall.transform, new(0, -1))),
                StartCoroutine(TranslateTo(topWall.transform, new(0, 1))),
                StartCoroutine(TranslateTo(leftWall.transform, new(-1, 0))),
                StartCoroutine(TranslateTo(rightWall.transform, new(1, 0))),
            };

            foreach (var item in parallel)
                yield return item;
        }

        private IEnumerator TranslateTo(Transform transform, Vector2 matrix)
        {
            var pos = transform.localPosition;

            if (matrix.x != 0)
                pos.x = matrix.x * ON_WALL;

            if (matrix.y != 0)
                pos.y = matrix.y * ON_WALL;

            return transform.TranslateLocal(4, 0.2f, transform.localPosition, pos);
        }

        /// <inheritdoc/>
        public override void StopSpawn() { }

        #endregion
    }
}