using Weapons;

namespace BattleEntity
{
    internal class BattlePlayerEntity : BattleEntity
    {
        private WeaponSO Weapon;

        public BattlePlayerEntity(WeaponSO weapon)
        {
            this.Health = 20;
            this.Type = BattleEntityType.NONE;
            this.Weapon = weapon;
        }
    }
}
