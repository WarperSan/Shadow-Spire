using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon.Generation;
using Managers;
using UnityEngine;
using UtilsModule;

namespace Weapons
{
    /// <summary>
    /// Instance of a weapon. Used to store and evaluate weapons
    /// </summary>
    public class WeaponInstance
    {
        public static WeaponSO[] WEAPONS;

        #region Data

        private WeaponSO _data;
        private BattleEntity.Type _types;
        private int _level;
        private int _rdmDamageBoost;

        #endregion

        #region Constructors

        public WeaponInstance(WeaponSO weapon, int level)
        {
            _data = weapon;
            _level = level;
            _types = weapon.BaseType;
            _rdmDamageBoost = GameManager.Instance.Level.Random.Next(0, GameManager.Instance.Level.Index);
        }

        public static WeaponInstance CreateRandom(int level)
        {
            var random = GameManager.Instance.Level.Random;

            var allWeapons = new List<WeaponSO>();

            foreach (var item in WEAPONS)
            {
                if (item.UnlockLevel <= level)
                    allWeapons.Add(item);
            }

            var rdmWeapon = allWeapons[random.Next(0, allWeapons.Count)];
            var weapon = new WeaponInstance(rdmWeapon, level);

            if (level >= 3 && random.NextDouble() < 0.3f)
            {
                // Add random type
                var allTypes = Enum.GetValues(typeof(BattleEntity.Type));

                var types = weapon._types.GetTypes().ToList();
                types.Add((BattleEntity.Type)allTypes.GetValue(random.Next(0, allTypes.Length)));

                // Remove extra types (max 2)
                for (int i = types.Count; i > 2; i--)
                    types.RemoveAt(random.Next(0, types.Count));

                // Assign new types
                weapon._types = BattleEntity.Type.NONE;

                for (int i = 0; i < types.Count; i++)
                    weapon._types |= types[i];
            }

            return weapon;
        }

        #endregion

        #region Getters

        public BattleEntity.Type GetTypes() => _types;

        public int GetDamage()
        {
            float damage = _data.BaseDamage;

            damage += _data.BaseDamage * 0.5f * (_level + _rdmDamageBoost * 0.4f) * _data.DamageRate;

            damage = Mathf.Max(damage, 0);

            return Mathf.FloorToInt(damage);
        }

        public Sprite GetIcon() => _data.Icon;

        #endregion

        public void Update(DungeonResult level)
        {
            if (_data.UpdateLevel)
                _level = level.Index;
        }
    }
}