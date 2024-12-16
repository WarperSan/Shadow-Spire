using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// Data of a weapon
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "Weapon", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        [Tooltip("Sprite used for the icon of this weapon")]
        public Sprite Icon;

        [Tooltip("Base type of this weapon")]
        public BattleEntity.Type BaseType;
        
        [Min(1), Tooltip("Base damage of this weapon")]
        public int BaseDamage;

        [Min(0), Tooltip("How much the level affects the damage")]
        public float DamageRate = 1f;
    }
}
