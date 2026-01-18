using UnityEngine;
using UnityEngine.Serialization;

namespace Battle.Projectiles
{
	public class GhostProjectile : Projectile
	{
		[FormerlySerializedAs("_collider")]
		[SerializeField]
		private Collider2D collider;

		[FormerlySerializedAs("Speed")]
		public float speed = 0.75f;
		[FormerlySerializedAs("Cooldown")]
		public float cooldown = 1.5f;
		[FormerlySerializedAs("Player")]
		public Transform player;

		#region Behaviour

		private float _cooldown;
		private bool _onCooldown;

		private void GoOnCooldown()
		{
			Color color = Color;
			color.a = 0.25f;
			SetColor(color);

			_onCooldown = true;
			_cooldown = cooldown;
			collider.enabled = false;
		}

		private void RecoverFromCooldown()
		{
			Color color = Color;
			color.a = 1f;
			SetColor(color);

			_cooldown = 0f;
			_onCooldown = false;
			collider.enabled = true;
		}

		#endregion

		#region Projectile

		/// <inheritdoc/>
		public override void OnHit() => GoOnCooldown();

		private void Update()
		{
			if (_onCooldown)
			{
				_cooldown -= Time.deltaTime;

				if (_cooldown > 0)
					return;

				RecoverFromCooldown();
			}

			transform.Translate(speed * Time.deltaTime * (player.position - transform.position).normalized);

			foreach (SpriteRenderer item in renderers)
				item.flipX = player.position.x > transform.position.x;
		}

		#endregion
	}
}