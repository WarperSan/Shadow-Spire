using System.Collections;
using Battle.Minigame.Projectiles;
using BattleEntity;
using UnityEngine;
using UtilsModule;

namespace Battle.Minigame.Spawners
{
    public class Giant_Spawner : Spawner
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private WreckingBarrelProjectile barrel;

        #endregion

        #region Data


        #endregion

        #region Spawner

        private const float MIN_X = -2f;
        private const float MIN_Y = 3f;
        private const float MAX_X = 2f;
        private const float MAX_Y = 6.75f;

        private const float OFF_Y = 8.5f;

        /// <inheritdoc/>
        public override Type HandledType => Type.GIANT;

        /// <inheritdoc/>
        public override void Clean()
        {
            barrel.transform.localPosition = new Vector3(0, OFF_Y, 0);
            barrel.gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public override void Setup(int strength)
        {
            // Pick a random X
            Vector3 pos = barrel.transform.localPosition;
            pos.x = MIN_X;
            barrel.transform.localPosition = pos;

            barrel.SetEnemy(HandledType);
            barrel.ResetSelf(Random.Range(MIN_Y, MAX_Y));
        }

        /// <inheritdoc/>
        public override IEnumerator StartSpawn(float duration)
        {
            barrel.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.4f);
            duration -= 0.4f;

            bool goingRight = true;

            while (duration > 0)
            {
                Vector3 pos = barrel.transform.localPosition;
                pos.x = goingRight ? MAX_X : MIN_X;

                barrel.NewPosition(pos);

                yield return new WaitForSeconds(1.4f);
                duration -= 1.4f;

                goingRight = !goingRight;

                if (duration > 0.3f && Random.value <= 0.8f)
                {
                    pos.y = Random.Range(MIN_Y, MAX_Y);
                    barrel.NewPosition(pos);

                    yield return new WaitForSeconds(0.3f);
                    duration -= 0.3f;
                }
            }
        }

        #endregion
    }
}