using Enemies;

namespace BattleEntity
{
    public class BattleEnemyEntity : BattleEntity
    {
        public EnemySO Enemy { get; private set; }

        public BattleEnemyEntity(EnemySO enemy)
        {
            this.Enemy = enemy;
            this.Health = enemy.BaseHealth;
            this.Attack = enemy.BaseAttack;
            this.Type = enemy.Type;
        }
    }
}
