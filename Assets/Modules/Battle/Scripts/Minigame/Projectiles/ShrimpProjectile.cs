using UnityEngine;

namespace Battle.Minigame.Projectiles
{
    public class ShrimpProjectile : Projectile
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

        /// <inheritdoc/>
        public override void OnHit() { }

        private void Update() => transform.Translate(4.5f * Time.deltaTime * Vector2.right, Space.World);

        #endregion
    }
}