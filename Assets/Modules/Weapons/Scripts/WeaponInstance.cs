using System;
using System.Collections.Generic;
using System.Linq;
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
        private float _rdmDamageBoost;

        #endregion

        #region Constructors

        public WeaponInstance(WeaponSO weapon, int level)
        {
            _data = weapon;
            _level = level / 5 + 1;
            _types = weapon.BaseType;
            _rdmDamageBoost = (float)GameManager.Instance.Level.Random.NextDouble() * 0.25f + 0.8f; // [0.8; 1.05]
        }

        #endregion

        #region Getters

        public BattleEntity.Type GetTypes() => _types;

        public int GetDamage()
        {
            float damage = _data.BaseDamage;

            damage += _data.BaseDamage * 0.125f * _level * _rdmDamageBoost;
            damage = Mathf.Max(damage, 0);

            return Mathf.FloorToInt(damage);
        }

        public Sprite GetIcon() => _data.Icon;

        #endregion

        #region Static

        public static WeaponInstance CreateRandom(int level)
        {
            System.Random random = GameManager.Instance.Level.Random;

            List<WeaponSO> allWeapons = new List<WeaponSO>();

            foreach (WeaponSO item in WEAPONS)
            {
                if (item.UnlockLevel <= level)
                    allWeapons.Add(item);
            }

            WeaponSO rdmWeapon = allWeapons[random.Next(0, allWeapons.Count)];
            WeaponInstance weapon = new WeaponInstance(rdmWeapon, level);

            if (level >= 10 && random.NextDouble() < 0.3f)
                AddRandomType(weapon);

            return weapon;
        }

        private static void AddRandomType(WeaponInstance weapon)
        {
            System.Random random = GameManager.Instance.Level.Random;

            Array allTypes = Enum.GetValues(typeof(BattleEntity.Type));

            List<BattleEntity.Type> types = weapon._types.GetTypes().ToList();
            types.Add((BattleEntity.Type)allTypes.GetValue(random.Next(0, allTypes.Length)));

            // Remove extra types (max 2)
            for (int i = types.Count; i > 2; i--)
                types.RemoveAt(random.Next(0, types.Count));

            // Assign new types
            weapon._types = BattleEntity.Type.NONE;

            for (int i = 0; i < types.Count; i++)
                weapon._types |= types[i];
        }

        #endregion
    }
}