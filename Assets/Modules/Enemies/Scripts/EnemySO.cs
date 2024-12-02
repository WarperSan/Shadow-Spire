using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy", order = 0)]
    public class EnemySO : ScriptableObject
    {
        public Sprite OverworldSprite;
        public Sprite FightSprite;
    }
}
