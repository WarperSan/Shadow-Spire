using System.Collections;
using Battle.Projectiles;
using Enemies;
using UnityEngine;
using Utils;

namespace Battle.Spawners
{
	public class NormalSpawner : Spawner
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private Projectile leftWall;

		[SerializeField]
		private Projectile rightWall;

		[SerializeField]
		private Projectile topWall;

		[SerializeField]
		private Projectile bottomWall;

		#endregion

		#region Spawner

		private const float OFF_WALL = 3.5f;
		private const float ON_WALL = 2.5f;

		/// <inheritdoc/>
		public override Type HandledType => Type.Normal;

		/// <inheritdoc/>
		public override void Clean()
		{
			leftWall.gameObject.SetActive(false);
			Vector3 leftPos = leftWall.transform.localPosition;
			leftPos.x = -OFF_WALL;
			leftWall.transform.localPosition = leftPos;

			rightWall.gameObject.SetActive(false);
			Vector3 rightPos = rightWall.transform.localPosition;
			rightPos.x = OFF_WALL;
			rightWall.transform.localPosition = rightPos;

			topWall.gameObject.SetActive(false);
			Vector3 topPos = topWall.transform.localPosition;
			topPos.y = OFF_WALL;
			topWall.transform.localPosition = topPos;

			bottomWall.gameObject.SetActive(false);
			Vector3 bottomPos = bottomWall.transform.localPosition;
			bottomPos.y = -OFF_WALL;
			bottomWall.transform.localPosition = bottomPos;
		}

		/// <inheritdoc/>
		public override void Setup(int strength)
		{
			bottomWall.SetEnemy(HandledType);
			topWall.SetEnemy(HandledType);
			leftWall.SetEnemy(HandledType);
			rightWall.SetEnemy(HandledType);

			bottomWall.gameObject.SetActive(strength >= 1);
			topWall.gameObject.SetActive(strength >= 2);
			leftWall.gameObject.SetActive(strength >= 3);
			rightWall.gameObject.SetActive(strength >= 3);
		}

		/// <inheritdoc/>
		public override IEnumerator StartSpawn(float duration)
		{
			Coroutine[] parallel = new Coroutine[]
			{
				StartCoroutine(TranslateTo(bottomWall.transform, new Vector2(0, -1))),
				StartCoroutine(TranslateTo(topWall.transform, new Vector2(0, 1))),
				StartCoroutine(TranslateTo(leftWall.transform, new Vector2(-1, 0))),
				StartCoroutine(TranslateTo(rightWall.transform, new Vector2(1, 0)))
			};

			foreach (Coroutine item in parallel)
				yield return item;
		}

		private IEnumerator TranslateTo(Transform transform, Vector2 matrix)
		{
			Vector3 pos = transform.localPosition;

			if (matrix.x != 0)
				pos.x = matrix.x * ON_WALL;

			if (matrix.y != 0)
				pos.y = matrix.y * ON_WALL;

			return transform.TranslateLocal(4,
				0.2f,
				transform.localPosition,
				pos);
		}

		#endregion
	}
}