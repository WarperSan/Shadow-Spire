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
            foreach (SpriteRenderer item in renderers)
                item.color = color;
        }

        #endregion
    }
}