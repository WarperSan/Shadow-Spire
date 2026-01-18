using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
	/// <summary>
	/// Data of a weapon
	/// </summary>
	[CreateAssetMenu(fileName = "WeaponSO", menuName = "Weapon", order = 0)]
	public class WeaponSo : ScriptableObject
	{
		[FormerlySerializedAs("Icon")]
		[Tooltip("Sprite used for the icon of this weapon")]
		public Sprite icon;

		[FormerlySerializedAs("BaseType")]
		[Tooltip("Base type of this weapon")]
		public Enemies.Type baseType;

		[FormerlySerializedAs("BaseDamage")]
		[Min(1)]
		[Tooltip("Base damage of this weapon")]
		public int baseDamage;

		[FormerlySerializedAs("UnlockLevel")]
		[Tooltip("At which level this weapon is unlockable")]
		public int unlockLevel;
	}
}