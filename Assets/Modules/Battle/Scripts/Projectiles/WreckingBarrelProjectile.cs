using UnityEngine;
using UnityEngine.Serialization;

namespace Battle.Projectiles
{
	public class WreckingBarrelProjectile : Projectile
	{
		#region Renderer

		[FormerlySerializedAs("_barrel")]
		[Header("Renderer")]
		[SerializeField]
		private SpriteRenderer barrel;

		[FormerlySerializedAs("_chain")]
		[SerializeField]
		private SpriteRenderer chain;

		/// <inheritdoc/>
		protected override void SetColor(Color color)
		{
			barrel.color = color;

			Color chainColor = color;
			chainColor.a = 0.25f;
			chain.color = chainColor;
		}

		#endregion

		private float _nextX;
		private float _nextY;
		private Vector2 _direction;

		private void Update()
		{
			Vector3 pos = transform.localPosition;

			// Move Y
			if (_direction.y != 0)
			{
				pos.y += 6 * Time.deltaTime * _direction.y;

				if (_direction.y == -1 && pos.y <= _nextY || _direction.y == 1 && pos.y >= _nextY)
				{
					pos.y = _nextY;
					_direction.y = 0;
				}
			}

			// Move X
			if (_direction.x != 0)
			{
				pos.x += 3 * Time.deltaTime * _direction.x;

				if (_direction.x == -1 && pos.x <= _nextX || _direction.x == 1 && pos.x >= _nextX)
				{
					pos.x = _nextX;
					_direction.x = 0;
				}
			}

			transform.localPosition = pos;
		}

		public void ResetSelf(float y) => NewPosition(new Vector2(transform.localPosition.x, y));

		public void NewPosition(Vector2 position)
		{
			_nextX = position.x;
			_nextY = position.y;

			_direction = new Vector2(
				transform.localPosition.x == _nextX ? 0 : Mathf.Sign(_nextX - transform.localPosition.x),
				transform.localPosition.y == _nextY ? 0 : Mathf.Sign(_nextY - transform.localPosition.y)
			);
		}
	}
}