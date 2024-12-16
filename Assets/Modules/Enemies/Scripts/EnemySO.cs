using BattleEntity;
using UnityEngine;

namespace Enemies
{
    public enum EnemyPathing
    {
        DIRECT,
        RANDOM
    }

    public enum EnemyMovementSpeed
    {
        VERY_SLOW,
        SLOW,
        NORMAL,
        FAST,
        VERY_FAST
    }

    /// <summary>
    /// Data of an enemy
    /// </summary>
    [CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy", order = 0)]
    public class EnemySO : ScriptableObject
    {
        [Header("Overworld")]
        [Tooltip("Sprite used when displaying this enemy in the overworld")]
        public Sprite OverworldSprite;

        [Tooltip("Determines how fast this enemy moves in the overworld")]
        public EnemyMovementSpeed MovementSpeed;

        [Tooltip("Determines the pathing used by this enemy")]
        public EnemyPathing Pathing;

        [Header("Fight")]
        [Tooltip("Sprite used when displaying this enemy in the fight")]
        public Sprite FightSprite;

        [Tooltip("Sprite used when display this enemy's shadow in the fight")]
        public Sprite FightShadowSprite;

        [Min(1)]
        public int BaseHealth = 1;

        [Min(1)]
        public int BaseAttack = 1;

        public BattleEntityType Type = BattleEntityType.NORMAL;
    }
}
