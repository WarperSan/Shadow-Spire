using System.Collections;
using Battle.Projectiles;
using Enemies;
using Player;
using UnityEngine;

namespace Battle.Spawners
{
	public class AirSpawner : Spawner
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private GameObject leafPrefab;

		[SerializeField]
		private Transform projectileParent;

		[SerializeField]
		private MinigamePlayer player;

		#endregion

		#region Data

		private Vector2 _pushingDirection;
		private float _pushingForce;
		private float _spawnDelay;

		#endregion

		#region Spawner

		private const float MIN_SPAWN_X = -2f;
		private const float MAX_SPAWN_X = 1f;
		private const float SPAWN_Y = 3.5f;

		/// <inheritdoc/>
		public override Type HandledType => Type.Air;

		/// <inheritdoc/>
		public override void Clean()
		{
			foreach (Transform item in projectileParent)
				Destroy(item.gameObject);

			player.pushingDirection -= _pushingDirection;
			_pushingDirection = Vector2.zero;
			_pushingForce = 0;
			_spawnDelay = 0;
		}

		/// <inheritdoc/>
		public override void Setup(int strength)
		{
			strength = 3;

			float angle = Random.Range(-10 * strength, 10 * strength) - 90;
			_pushingForce = Random.Range(0.01f, 0.2f) * strength;

			_pushingDirection = new Vector2(
				Mathf.Cos(angle * Mathf.Deg2Rad),
				Mathf.Sign(angle * Mathf.Deg2Rad)
			) * _pushingForce;

			_spawnDelay = 3.5f / strength;
		}

		/// <inheritdoc/>
		public override IEnumerator StartSpawn(float duration)
		{
			player.pushingDirection += _pushingDirection;

			yield return null;

			while (duration > 0)
			{
				GameObject newProjectile = Instantiate(leafPrefab, projectileParent);

				newProjectile.transform.localPosition = new Vector2(
					Random.Range(MIN_SPAWN_X, MAX_SPAWN_X),
					SPAWN_Y
				);

				if (newProjectile.TryGetComponent(out LeafProjectile projectile))
				{
					projectile.SetEnemy(HandledType);
					projectile.push = _pushingDirection;
				}

				yield return new WaitForSeconds(_spawnDelay);

				duration -= _spawnDelay;
			}
		}

		#endregion
	}
}