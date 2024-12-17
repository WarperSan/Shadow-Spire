using BattleEntity;
using Player;
using UnityEngine;
using UtilsModule;

namespace Battle.Minigame.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        #region Renderer

        protected virtual void SetColor(Color color) { }

        #endregion

        public void SetEnemy(Type type)
        {
            SetColor(type.GetColor());
        }

        public virtual void OnHit() => gameObject.SetActive(false);
        public virtual int GetDamage() => 5;

        #region Damage

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out MinigamePlayer player))
                return;

            bool success = player.HitPlayer(GetDamage());

            if (!success)
                return;

            OnHit();
        }

        #endregion
    }
}