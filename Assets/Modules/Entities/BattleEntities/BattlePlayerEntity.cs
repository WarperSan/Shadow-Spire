using Entities;
using Weapons;

namespace BattleEntity
{
    internal class BattlePlayerEntity : BattleEntity
    {
        private PlayerEntity playerEntity;        
        public BattlePlayerEntity(PlayerEntity playerEntity)
        {
            this.Health = playerEntity.Health;
            this.playerEntity = playerEntity;
            this.Type = BattleEntityType.NONE;
        }
        protected override void OnHit(int damage)
        {
            playerEntity.TakeDamage(damage);
        }

        protected override void OnDeath(int damage)
        {
            playerEntity.TakeDamage(damage);
        }
    }
}
