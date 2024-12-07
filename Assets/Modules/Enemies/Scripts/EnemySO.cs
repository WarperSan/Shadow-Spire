using BattleEntity;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy", order = 0)]
    public class EnemySO : ScriptableObject
    {
        public Sprite OverworldSprite;

        [Header("Fight")]
        public Sprite FightSprite;
        public Sprite FightShadowSprite;

        public int BaseHealth = 1;
        public int BaseAttack = 1;
        public BattleEntityType Type = BattleEntityType.NORMAL;

    }
}
