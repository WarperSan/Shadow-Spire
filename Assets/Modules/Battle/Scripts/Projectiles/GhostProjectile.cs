using UnityEngine;

namespace Battle.Projectiles
{
    public class GhostProjectile : Projectile
    {
        [SerializeField]
        private Collider2D _collider;

        public float Speed = 0.75f;
        public float Cooldown = 1.5f;
        public Transform Player;

        #region Behaviour

        private float _cooldown;
        private bool onCooldown;

        private void GoOnCooldown()
        {
            Color color = _color;
            color.a = 0.25f;
            SetColor(color);

            onCooldown = true;
            _cooldown = Cooldown;
            _collider.enabled = false;
        }

        private void RecoverFromCooldown()
        {
            Color color = _color;
            color.a = 1f;
            SetColor(color);

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

            foreach (var item in _renderers)
                item.flipX = Player.position.x > transform.position.x;
        }

        #endregion
    }
}