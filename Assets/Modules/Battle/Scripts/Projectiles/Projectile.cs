using Enemies;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Battle.Projectiles
{
	public class Projectile : MonoBehaviour
	{
		#region Renderer

		[FormerlySerializedAs("_renderers")]
		[Header("Renderer")]
		[SerializeField]
		protected SpriteRenderer[] renderers;

		protected Color Color;

		protected virtual void SetColor(Color color)
		{
			Color = color;

			foreach (SpriteRenderer item in renderers)
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