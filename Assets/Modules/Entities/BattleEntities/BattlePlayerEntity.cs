using GridEntities.Entities;

namespace BattleEntity
{
	/// <summary>
	/// Battle entity that represents the <see cref="PlayerEntity"/>
	/// </summary>
	public class BattlePlayerEntity : BattleEntity
	{
		private readonly PlayerEntity _playerEntity;

		public BattlePlayerEntity(PlayerEntity playerEntity)
		{
			Health = playerEntity.Health;
			_playerEntity = playerEntity;
			Type = Enemies.Type.None;
		}

		/// <inheritdoc/>
		protected override void OnHit(int damage)
		{
			_playerEntity.TakeDamage(damage);
		}

		/// <inheritdoc/>
		protected override void OnDeath(int damage)
		{
			_playerEntity.TakeDamage(damage);
		}

		public void Heal(int amount)
		{
			Health += amount;
			_playerEntity.Heal(amount);
		}
	}
}