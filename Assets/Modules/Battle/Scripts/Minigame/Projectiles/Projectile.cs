using BattleEntity;
using Managers;
using Player;
using UnityEngine;
using Utils;

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

        public virtual void OnHit() { }
        public virtual int GetDamage() => Mathf.Max(GameManager.Instance.Level.Index / 10, 1) * 5;

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