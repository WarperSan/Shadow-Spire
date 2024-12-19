using UnityEngine;

namespace Battle.Minigame.Projectiles
{
    public class LeafProjectile : Projectile
    {
        #region Renderer

        [Header("Renderer")]
        [SerializeField]
        private SpriteRenderer _renderer;

        /// <inheritdoc/>
        protected override void SetColor(Color color)
        {
            color.a = 0.15f;
            _renderer.color = color;
        }

        #endregion

        #region Projectile

        public Vector3 push;

        private void Update()
        {
            transform.localPosition += 8 * Time.deltaTime * push;
            transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time) / 4f * 180, 0, 0);
        }

        #endregion
    }
}