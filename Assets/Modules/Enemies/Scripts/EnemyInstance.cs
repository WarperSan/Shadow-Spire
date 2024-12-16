using BattleEntity;
using Dungeon.Generation;
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
            _level = level;
        }

        public static EnemyInstance CreateRandom(int level)
        {
            level -= DungeonGenerator.ENEMY_ROOM_INDEX;

            var random = GameManager.Instance.Level.Random;

            var rdmEnemy = ENEMIES[random.Next(0, ENEMIES.Length)];
            var enemy = new EnemyInstance(rdmEnemy, level);

            return enemy;
        }

        #endregion

        #region Getters

        public EnemySO GetRaw() => _data;
        public int GetHealth() => Mathf.RoundToInt(_data.BaseHealth + _data.BaseHealth * 0.5f * _level * 0.5f);
        public int GetAttack() => 5;
        public Type GetTypes() => _data.Type;

        #endregion
    }
}