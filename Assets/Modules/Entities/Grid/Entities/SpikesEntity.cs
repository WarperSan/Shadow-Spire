using System.Collections;
using Entities.Grid.Abstract;
using Entities.Grid.Interfaces;
using UnityEngine;

namespace Entities.Grid.Entities
{
	public class SpikesEntity : GridEntity, ITurnable, IEventable<PlayerEntity>
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private Sprite offSprite;

		[SerializeField]
		private Sprite onSprite;

		#endregion

		private const int ACTIVE_TURNS = 3;

		private int _activeTurns;
		private bool _canAttack;

		#region IEventable

		/// <inheritdoc/>
		public void OnEntityLand(PlayerEntity entity)
		{ /* Spikes can't move on another entity */
		}

		/// <inheritdoc/>
		public void OnEntityLanded(PlayerEntity entity)
		{
			if (_canAttack)
			{
				entity.TakeDamage(2);
				_activeTurns = ACTIVE_TURNS;
			} else
				_activeTurns = ACTIVE_TURNS + 1;
		}

		#endregion

		#region ITurnable

		public IEnumerator Think()
		{
			_activeTurns--;
			_canAttack = _activeTurns < ACTIVE_TURNS && _activeTurns >= 0;
			spriteRenderer.sprite = _canAttack ? onSprite : offSprite;

			yield return null;
		}

		#endregion
	}
}