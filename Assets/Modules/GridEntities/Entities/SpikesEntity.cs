using System.Collections;
using GridEntities.Abstract;
using GridEntities.Interfaces;
using UnityEngine;

namespace GridEntities.Entities
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

		private int activeTurns;
		private bool canAttack;

		#region IEventable

		/// <inheritdoc/>
		public void OnEntityLand(PlayerEntity entity)
		{ /* Spikes can't move on another entity */
		}

		/// <inheritdoc/>
		public void OnEntityLanded(PlayerEntity entity)
		{
			if (canAttack)
			{
				entity.TakeDamage(2);
				activeTurns = ACTIVE_TURNS;
			} else
				activeTurns = ACTIVE_TURNS + 1;
		}

		#endregion

		#region ITurnable

		public IEnumerator Think()
		{
			activeTurns--;
			canAttack = activeTurns < ACTIVE_TURNS && activeTurns >= 0;
			spriteRenderer.sprite = canAttack ? onSprite : offSprite;

			yield return null;
		}

		#endregion
	}
}