using UnityEngine;

namespace Battle.Minigame.Projectiles
{
    public class SpikeWallProjectile : Projectile
    {
        #region Renderer

        [Header("Renderer")]
        [SerializeField]
        private SpriteRenderer[] renderers;

        /// <inheritdoc/>
        protected override void SetColor(Color color)
        {
            foreach (var item in renderers)
                item.color = color;
        }

        #endregion

        /// <inheritdoc/>
        public override void OnHit() { }
    }
}