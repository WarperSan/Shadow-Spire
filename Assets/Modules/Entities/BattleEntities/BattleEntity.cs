using System;

namespace BattleEntity
{
    [Flags]
    public enum BattleEntityType
    {
        NONE = 0,
        NORMAL = 1 << 1,
        UNDEAD = 1 << 2,
        GHOST = 1 << 3,
        GIANT = 1 << 4,
        ANIMAL = 1 << 5,
    }

    public abstract class BattleEntity
    {
        public int Health { get; protected set; }
        public int Attack { get; protected set; }
        public BattleEntityType Type { get; protected set; }

        #region Effectiveness

        // x0   = IMMUNE
        // x0.5 = NOT EFFECTIVE
        // x1   = NORMAL
        // x1.5 = VERY EFFECTIVE
        // x3   = SUPER EFFECTIVE
        private static float[,] TYPE_CHART = {
            // ATK \ DEF   NONE NORMAL UNDEAD GHOST GIANT ANIMAL
            /* NONE */   {  1f,    1f,     1f,   1f,   1f,   1f },
            /* NORMAL */ {  1f,    1f,   0.5f, 0.5f,   1f, 1.5f }, // VERY WEAK
            /* UNDEAD */ {  1f,    3f,   0.5f,   1f, 0.5f,   3f }, // GLASS CANNON
            /* GHOST */  {  1f,  1.5f,   0.5f,   3f, 0.5f, 1.5f }, // SAFER GLASS CANNON
            /* GIANT */  {  1f,  1.5f,   1.5f, 0.5f,   1f, 1.5f }, // BALANCED
            /* ANIMAL */ {  1f,  1.5f,   0.5f,   1f, 0.5f,   3f }, // WEAK
        };

        public float CalculateEffectiveness(BattleEntityType attackType)
        {
            var types = Enum.GetValues(typeof(BattleEntityType));

            // Find index of attack type
            int attackIndex = Array.IndexOf(types, attackType);

            // Find index of defence type
            int defenceIndex = Array.IndexOf(types, Type);

            // Get multiplier
            float percent = 100f;

            percent *= TYPE_CHART[attackIndex, defenceIndex];

            return percent;
        }

        #endregion
    }

}
