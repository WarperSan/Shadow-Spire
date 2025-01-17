using UnityEngine;

namespace Battle.Projectiles
{
    public class WreckingBarrelProjectile : Projectile
    {
        #region Renderer

        [Header("Renderer")]
        [SerializeField]
        private SpriteRenderer _barrel;

        [SerializeField]
        private SpriteRenderer _chain;

        /// <inheritdoc/>
        protected override void SetColor(Color color)
        {
            _barrel.color = color;

            Color chainColor = color;
            chainColor.a = 0.25f;
            _chain.color = chainColor;
        }

        #endregion

        private float nextX;
        private float nextY;
        private Vector2 direction;

        private void Update()
        {
            Vector3 pos = transform.localPosition;

            // Move Y
            if (direction.y != 0)
            {
                pos.y += 6 * Time.deltaTime * direction.y;

                if ((direction.y == -1 && pos.y <= nextY) || (direction.y == 1 && pos.y >= nextY))
                {
                    pos.y = nextY;
                    direction.y = 0;
                }
            }

            // Move X
            if (direction.x != 0)
            {
                pos.x += 3 * Time.deltaTime * direction.x;

                if ((direction.x == -1 && pos.x <= nextX) || (direction.x == 1 && pos.x >= nextX))
                {
                    pos.x = nextX;
                    direction.x = 0;
                }
            }

            transform.localPosition = pos;
        }

        public void ResetSelf(float y) => NewPosition(new Vector2(transform.localPosition.x, y));
        public void NewPosition(Vector2 position)
        {
            nextX = position.x;
            nextY = position.y;

            direction = new Vector2(
                transform.localPosition.x == nextX ? 0 : Mathf.Sign(nextX - transform.localPosition.x),
                transform.localPosition.y == nextY ? 0 : Mathf.Sign(nextY - transform.localPosition.y)
            );
        }
    }
}
