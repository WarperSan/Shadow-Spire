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

        /// <inheritdoc/>
        public override void OnHit() { }

        private void Update()
        {
            transform.Translate(3 * Vector2.down * Time.deltaTime, Space.World);
        }
    }
}