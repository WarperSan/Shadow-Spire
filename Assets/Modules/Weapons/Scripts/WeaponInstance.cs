using Managers;
using UnityEngine;

namespace Weapons
{
    public class WeaponInstance
    {
        private WeaponSO _data;
        private int _level;

        public WeaponSO GetBaseData() => _data;
        public int GetDamage() => Mathf.RoundToInt(_data.BaseDamage + _data.BaseDamage * _level * 0.4f);

        public WeaponInstance(WeaponSO weapon, int level)
        {
            _data = weapon;
            _level = level;
        }

        public static WeaponInstance CreateRandom(int level)
        {
            var allWeapons = GameManager.Instance.allWeapons;
            var random = GameManager.Instance.Level.Random;

            var rdmWeapon = allWeapons[random.Next(0, allWeapons.Length)];
            return new WeaponInstance(rdmWeapon, level);
        }
    }
}