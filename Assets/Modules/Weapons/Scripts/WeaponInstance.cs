using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Utils;
using Type = Enemies.Type;

namespace Weapons
{
	/// <summary>
	/// Instance of a weapon. Used to store and evaluate weapons
	/// </summary>
	public class WeaponInstance
	{
		public static WeaponSo[] Weapons;

		#region Data

		private readonly WeaponSo _data;
		private Type _types;
		private readonly int _level;
		private readonly float _rdmDamageBoost;

		#endregion

		#region Constructors

		public WeaponInstance(WeaponSo weapon, int level)
		{
			_data = weapon;
			_level = level / 5 + 1;
			_types = weapon.baseType;
			_rdmDamageBoost = (float)GameManager.Instance.Level.Random.NextDouble() * 0.25f + 0.8f; // [0.8; 1.05]
		}

		#endregion

		#region Getters

		public Type GetTypes() => _types;

		public int GetDamage()
		{
			float damage = _data.baseDamage;

			damage += _data.baseDamage * 0.125f * _level * _rdmDamageBoost;
			damage = Mathf.Max(damage, 0);

			return Mathf.FloorToInt(damage);
		}

		public Sprite GetIcon() => _data.icon;

		#endregion

		#region Static

		public static WeaponInstance CreateRandom(int level)
		{
			System.Random random = GameManager.Instance.Level.Random;

			List<WeaponSo> allWeapons = new();

			foreach (WeaponSo item in Weapons)
			{
				if (item.unlockLevel <= level)
					allWeapons.Add(item);
			}

			WeaponSo rdmWeapon = allWeapons[random.Next(0, allWeapons.Count)];
			WeaponInstance weapon = new(rdmWeapon, level);

			if (level >= 10 && random.NextDouble() < 0.3f)
				AddRandomType(weapon);

			return weapon;
		}

		private static void AddRandomType(WeaponInstance weapon)
		{
			System.Random random = GameManager.Instance.Level.Random;

			Array allTypes = Enum.GetValues(typeof(Type));

			List<Type> types = weapon._types.GetTypes().ToList();
			types.Add((Type)allTypes.GetValue(random.Next(0, allTypes.Length)));

			// Remove extra types (max 2)
			for (int i = types.Count; i > 2; i--)
				types.RemoveAt(random.Next(0, types.Count));

			// Assign new types
			weapon._types = Type.None;

			foreach (Type t in types)
				weapon._types |= t;
		}

		#endregion
	}
}