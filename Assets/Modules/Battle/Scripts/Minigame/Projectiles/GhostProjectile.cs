using UnityEngine;

namespace Battle.Minigame.Projectiles
{
    public class GhostProjectile : Projectile
    {
        #region Renderer

        [Header("Renderer")]
        [SerializeField]
        private SpriteRenderer _renderer;

        [SerializeField]
        private Collider2D _collider;

        /// <inheritdoc/>
        protected override void SetColor(Color color)
        {
            _renderer.color = color;
        }

        public float Speed = 0.75f;
        public float Cooldown = 1.5f;
        public Transform Player;

        #endregion

        #region Behaviour

        private float _cooldown;
        private bool onCooldown;

        private void GoOnCooldown()
        {
            var color = _renderer.color;
            color.a = 0.25f;
            _renderer.color = color;

            onCooldown = true;
            _cooldown = Cooldown;
            _collider.enabled = false;
        }

        private void RecoverFromCooldown()
        {
            var color = _renderer.color;
            color.a = 1f;
            _renderer.color = color;

            _cooldown = 0f;
            onCooldown = false;
            _collider.enabled = true;
        }

        #endregion

        #region Projectile

        /// <inheritdoc/>
        public override void OnHit() => GoOnCooldown();

        private void Update()
        {
            if (onCooldown)
            {
                _cooldown -= Time.deltaTime;

                if (_cooldown > 0)
                    return;

                RecoverFromCooldown();
            }

            transform.Translate(Speed * Time.deltaTime * (Player.position - transform.position).normalized);
            _renderer.flipX = Player.position.x > transform.position.x;
        }

        #endregion
    }
}