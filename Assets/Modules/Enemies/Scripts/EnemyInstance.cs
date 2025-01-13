using BattleEntity;
using Managers;
using UnityEngine;

namespace Enemies
{
    public class EnemyInstance
    {
        public static EnemySO[] ENEMIES;

        #region Data

        private EnemySO _data;
        private int _level;

        #endregion

        #region Constructors

        public EnemyInstance(EnemySO enemy, int level)
        {
            _data = enemy;
            _level = level / 10;
        }

        public static EnemyInstance CreateRandom(int level)
        {
            System.Random random = GameManager.Instance.Level.Random;

            EnemySO rdmEnemy = ENEMIES[random.Next(0, ENEMIES.Length)];
            EnemyInstance enemy = new(rdmEnemy, level);

            return enemy;
        }

        #endregion

        #region Getters

        public EnemySO GetRaw() => _data;

        public int GetHealth()
        {
            float health = _data.BaseHealth;

            health += _data.BaseHealth * 0.15f * _level;
            health = Mathf.Max(health, 0);

            return Mathf.FloorToInt(health);
        }
        public int GetAttack() => 5;
        public Type GetTypes() => _data.BaseType;

        #endregion
    }
}