using Enemies;
using UnityEngine.Events;

namespace BattleEntity
{
    public class BattleEnemyEntity : BattleEntity
    {
        public EnemyInstance Enemy { get; private set; }
        public UnityEvent<int> Hit { get; private set; } = new();
        public UnityEvent<int> Death { get; private set; } = new();

        public BattleEnemyEntity(EnemyInstance enemy)
        {
            Enemy = enemy;
            Health = enemy.GetHealth();
            Attack = enemy.GetAttack();
            Type = enemy.GetTypes();
        }

        /// <inheritdoc/>
        protected override void OnHit(int damage) => Hit?.Invoke(damage);

        /// <inheritdoc/>
        protected override void OnDeath(int damage) => Death?.Invoke(damage);
    }
}
