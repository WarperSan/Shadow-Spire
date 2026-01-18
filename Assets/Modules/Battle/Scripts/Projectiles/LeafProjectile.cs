using UnityEngine;

namespace Battle.Projectiles
{
	public class LeafProjectile : Projectile
	{
		#region Projectile

		public Vector3 push;

		private void Update()
		{
			transform.localPosition += 8 * Time.deltaTime * push;
			transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time) / 4f * 180, 0, 0);
		}

		/// <inheritdoc/>
		protected override void SetColor(Color color)
		{
			color.a = 0.15f;
			base.SetColor(color);
		}

		#endregion
	}
}