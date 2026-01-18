using System.Collections;
using Battle.Projectiles;
using Enemies;
using UnityEngine;

namespace Battle.Spawners
{
	public class UndeadSpawner : Spawner
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private GameObject bonePrefab;

		[SerializeField]
		private Transform projectileParent;

		#endregion

		#region Data

		private float _spawnDelay = -1;

		#endregion

		#region Spawner

		private const float SPAWN_Y = 3;
		private const float MIN_SPAWN_X = -2.25f;
		private const float MAX_SPAWN_X = 2.25f;

		/// <inheritdoc/>
		public override Type HandledType => Type.Undead;

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
			_spawnDelay = Mathf.Max(1.2f / strength, 0.01f);
		}

		/// <inheritdoc/>
		public override IEnumerator StartSpawn(float duration)
		{
			yield return new WaitForSeconds(0.5f);

			duration -= 0.5f;

			while (duration > 0)
			{
				GameObject newProjectile = Instantiate(bonePrefab, projectileParent);

				newProjectile.transform.localPosition = new Vector2(
					Random.Range(MIN_SPAWN_X, MAX_SPAWN_X),
					SPAWN_Y
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