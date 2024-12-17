using System.Collections;
using Entities.Interfaces;
using UnityEngine;

namespace Entities
{
    public class SpikesEntity : GridEntity, ITurnable, IEventable<PlayerEntity>
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Sprite offSprite;

        [SerializeField]
        private Sprite onSprite;

        #endregion

        private int activeTurns;
        private bool canAttack;

        #region IEventable

        /// <inheritdoc/>
        public void OnEntityLand(PlayerEntity entity) { /* Spikes can't move on another entity */ }

        /// <inheritdoc/>
        public void OnEntityLanded(PlayerEntity entity)
        {
            if (canAttack)
            {
                entity.TakeDamage(2);
                activeTurns = 2;
            }
            else
                activeTurns = 3;
        }

        #endregion

        #region ITurnable

        public IEnumerator Think()
        {
            activeTurns--;
            canAttack = activeTurns < 2 && activeTurns >= 0; // 0 and 1 
            spriteRenderer.sprite = canAttack ? onSprite : offSprite;

            yield return null;
        }

        #endregion
    }
}