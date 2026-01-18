using Managers;
using UnityEngine;

namespace Enemies
{
	/// <summary>
	/// Class used to represent an enemy instance
	/// </summary>
	public class EnemyInstance
	{
		public static EnemySo[] Enemies;

		#region Data

		private readonly EnemySo _data;
		private readonly int _level;

		#endregion

		#region Constructors

		public EnemyInstance(EnemySo enemy, int level)
		{
			_data = enemy;
			_level = level / 10;
		}

		public static EnemyInstance CreateRandom(int level)
		{
			System.Random random = GameManager.Instance.Level.Random;

			EnemySo rdmEnemy = Enemies[random.Next(0, Enemies.Length)];
			EnemyInstance enemy = new(rdmEnemy, level);

			return enemy;
		}

		#endregion

		#region Getters

		public EnemySo GetRaw() => _data;

		public int GetHealth()
		{
			float health = _data.baseHealth;

			health += _data.baseHealth * 0.15f * _level;
			health = Mathf.Max(health, 0);

			return Mathf.FloorToInt(health);
		}

		public int  GetAttack() => 5;
		public Type GetTypes()  => _data.baseType;

		#endregion
	}
}