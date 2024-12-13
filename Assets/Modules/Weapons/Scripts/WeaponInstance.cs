using Dungeon.Generation;
using Managers;
using UnityEngine;

namespace Weapons
{
    public class WeaponInstance
    {
        private WeaponSO _data;
        private int _level;
        private int _rdmDamageBoost;

        public WeaponSO GetBaseData() => _data;
        public int GetDamage() => Mathf.RoundToInt(_data.BaseDamage + _data.BaseDamage * 0.5f * (_level + _rdmDamageBoost * 0.4f));

        public WeaponInstance(WeaponSO weapon, int level)
        {
            _data = weapon;
            _level = level;
            _rdmDamageBoost = GameManager.Instance.Level.Random.Next(0, GameManager.Instance.Level.Index);
        }

        public static WeaponInstance CreateRandom(int level)
        {
            var allWeapons = GameManager.Instance.allWeapons;
            var random = GameManager.Instance.Level.Random;

            var rdmWeapon = allWeapons[random.Next(0, allWeapons.Length)];
            return new WeaponInstance(rdmWeapon, level - DungeonGenerator.WEAPON_INDEX);
        }
    }
}