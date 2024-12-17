using Managers;
using UnityEngine;

namespace Player
{
    public class MinigamePlayer : MonoBehaviour
    {
        [SerializeField] float movementSpeed;

        private Vector2 direction;
        public BattleManager battleManager;

        public bool canTakeDamage;

        void Update()
        {
            Vector3 movement = movementSpeed * Time.deltaTime * direction;
            Vector3 nextPosition = transform.localPosition + movement;

            nextPosition.x = Mathf.Clamp(nextPosition.x, -2.5f, 2.5f);
            nextPosition.y = Mathf.Clamp(nextPosition.y, -2.5f, 2.5f);

            transform.localPosition = nextPosition;
        }

        public bool HitPlayer(int damage)
        {
            if (!canTakeDamage)
                return false;

            battleManager.DamagePlayer(damage);
            return true;
        }

        public void ResetSelf()
        {
            transform.localPosition = Vector3.zero;
            direction = Vector2.zero;
        }

        #region Inputs

        public void Move(Vector2 direction) => this.direction = direction;

        #endregion
    }
}