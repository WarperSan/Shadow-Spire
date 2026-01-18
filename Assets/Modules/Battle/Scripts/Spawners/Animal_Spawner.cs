using System.Collections;
using Battle.Projectiles;
using Enemies;
using UnityEngine;

namespace Battle.Spawners
{
	public class AnimalSpawner : Spawner
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private GameObject shrimpPrefab;

		[SerializeField]
		private Transform projectileParent;

		#endregion

		#region Data

		private float _spawnDelay = -1;

		#endregion

		#region Spawner

		private const float SPAWN_X = -3;
		private const float MIN_SPAWN_Y = -2.25f;
		private const float MAX_SPAWN_Y = 2.25f;

		/// <inheritdoc/>
		public override Type HandledType => Type.Animal;

		/// <inheritdoc/>
		public override void Clean()
		{
			foreach (Transform item in projectileParent)
				Destroy(item.gameObject);

			_spawnDelay = -1;
		}

		/// <inheritdoc/>
		public override void Setup(int strength)
		{
			_spawnDelay = Mathf.Max(1.25f / strength, 0.01f);
		}

		/// <inheritdoc/>
		public override IEnumerator StartSpawn(float duration)
		{
			while (duration > 0)
			{
				GameObject newProjectile = Instantiate(shrimpPrefab, projectileParent);

				newProjectile.transform.localPosition = new Vector2(
					SPAWN_X,
					Random.Range(MIN_SPAWN_Y, MAX_SPAWN_Y)
				);

				if (newProjectile.TryGetComponent(out Projectile projectile))
					projectile.SetEnemy(HandledType);

				if (duration < _spawnDelay)
					break;

				yield return new WaitForSeconds(_spawnDelay);

				duration -= _spawnDelay;
			}
		}

		#endregion
	}
}