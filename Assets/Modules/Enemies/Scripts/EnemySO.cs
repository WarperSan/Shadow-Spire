using BattleEntity;
using UnityEngine;

namespace Enemies
{
    public enum EnemyMovementSpeed
    {
        VERY_SLOW,
        SLOW,
        NORMAL,
        FAST,
        VERY_FAST
    }

    [CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy", order = 0)]
    public class EnemySO : ScriptableObject
    {
        [Header("Overworld")]
        public Sprite OverworldSprite;
        public EnemyMovementSpeed MovementSpeed;

        [Header("Fight")]
        public Sprite FightSprite;
        public Sprite FightShadowSprite;

        public int BaseHealth = 1;
        public int BaseAttack = 1;
        public BattleEntityType Type = BattleEntityType.NORMAL;

    }
}
