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
    }
}
