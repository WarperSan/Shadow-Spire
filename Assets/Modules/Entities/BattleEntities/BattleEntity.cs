using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

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
        public int Attack { get; protected set; }
        
        #region Health

        public int Health { get; protected set; }
        public bool IsDead { get; private set; }
        
        public void TakeAttack(WeaponSO weapon)
        {
            int damage = Mathf.RoundToInt(2 * CalculateEffectiveness(weapon.AttackType) / 100f);
            TakeDamage(damage);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                IsDead = true;
                OnDeath(damage);
                return;
            }

            OnHit(damage);
        }

        /// <summary>
        /// Called when this entity gets hit
        /// </summary>
        protected virtual void OnHit(int damage) { }

        /// <summary>
        /// Called when this entity dies
        /// </summary>
        protected virtual void OnDeath(int damage) { }

        #endregion

        #region Type

        public BattleEntityType Type { get; protected set; }

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
            // Find index of attack type
            int[] attackIndexes = GetTypeIndexes(attackType);

            // Find index of defence type
            int[] defenceIndexes = GetTypeIndexes(Type);

            // Get multiplier
            float percent = 100f;

            foreach (var attackIndex in attackIndexes)
            {
                foreach (var defenceIndex in defenceIndexes)
                    percent *= TYPE_CHART[attackIndex, defenceIndex];
            }


            return percent;
        }

        private static int[] GetTypeIndexes(BattleEntityType type)
        {
            var types = Enum.GetValues(typeof(BattleEntityType));
            var indexes = new List<int>();

            for (int i = 0; i < types.Length; i++)
            {
                BattleEntityType item = (BattleEntityType)types.GetValue(i);

                if ((type & item) != 0)
                    indexes.Add(i);
            }

            return indexes.ToArray();
        }

        #endregion
    }

}
