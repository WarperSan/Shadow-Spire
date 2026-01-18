using BattleEntity;
using Managers;
using Player;
using UnityEngine;
using Utils;

namespace Battle.Projectiles
{
	public class Projectile : MonoBehaviour
	{
		#region Renderer

		[Header("Renderer")]
		[SerializeField]
		protected SpriteRenderer[] _renderers;

		protected Color _color;

		protected virtual void SetColor(Color color)
		{
			_color = color;

			foreach (SpriteRenderer item in _renderers)
				item.color = color;
		}

		#endregion

		public void SetEnemy(Type type)
		{
			SetColor(type.GetColor());
		}

		public virtual void OnHit()     { }
		public virtual int  GetDamage() => Mathf.Max(GameManager.Instance.Level.Index / 10, 1) * 5;

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