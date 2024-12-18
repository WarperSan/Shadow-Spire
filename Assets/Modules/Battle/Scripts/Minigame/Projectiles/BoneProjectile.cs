using UnityEngine;

namespace Battle.Minigame.Projectiles
{
    public class BoneProjectile : Projectile
    {
        #region Renderer

        [Header("Renderer")]
        [SerializeField]
        private SpriteRenderer _renderer;

        /// <inheritdoc/>
        protected override void SetColor(Color color)
        {
            _renderer.color = color;
        }

        #endregion

        #region Projectile

        private void Update() => transform.Translate(3 * Time.deltaTime * Vector2.down, Space.World);

        #endregion
    }
}