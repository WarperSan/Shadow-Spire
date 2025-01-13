using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace BattleEntity
{
    [Flags]
    public enum Type
    {
        NONE = 0,
        NORMAL = 1 << 1,
        UNDEAD = 1 << 2,
        GHOST = 1 << 3,
        GIANT = 1 << 4,
        ANIMAL = 1 << 5,
        AIR = 1 << 6,
    }

    public abstract class BattleEntity
    {
        public int Attack { get; protected set; }

        #region Health

        public int Health { get; protected set; }
        public bool IsDead { get; private set; }

        public void TakeAttack(WeaponInstance weapon)
        {
            int damage = Mathf.RoundToInt(weapon.GetDamage() * CalculateEffectiveness(weapon.GetTypes()) / 100f);
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

        public Type Type { get; protected set; }

        // x0.5 = NOT EFFECTIVE
        // x1   = NORMAL
        // x1.5 = VERY EFFECTIVE
        // x3   = SUPER EFFECTIVE

        // private static float[,] TYPE_CHART = {
        //     // ATK \ DEF   NONE NORMAL UNDEAD GHOST GIANT ANIMAL AIR
        //     /* NONE */   {  1f,    1f,     1f,   1f,   1f,   1f,   1f }, // NEUTRAL
        //     /* NORMAL */ {  1f,    1f,   0.5f, 0.5f,   1f, 1.5f,   1f }, // VERY WEAK
        //     /* UNDEAD */ {  1f,    3f,   0.5f, 0.5f, 0.5f,   3f, 1.5f }, // GLASS CANNON
        //     /* GHOST */  {  1f,    1f,     1f,   3f, 0.5f, 1.5f,   3f }, // SAFER GLASS CANNON
        //     /* GIANT */  {  1f,  1.5f,     3f, 0.5f,   1f, 0.5f, 0.5f }, // BALANCED
        //     /* ANIMAL */ {  1f,  1.5f,   0.5f,   1f,   3f,   1f,   1f }, // WEAK
        //     /* AIR */    {  1f,  0.5f,     3f,   1f, 1.5f, 0.5f,   1f }, // ???
        // };

        // x0.5 = RESISTED
        // x1   = NORMAL
        // x2   = EFFECTIVE

        private static float[,] TYPE_CHART = {
            // ATK \ DEF   NONE NORMAL UNDEAD GHOST GIANT ANIMAL AIR
            /* NONE */   {  1f,    1f,     1f,   1f,   1f,   1f,   1f },
            /* NORMAL */ {  1f,    1f,     1f,   1f, 0.5f,   1f,   2f },
            /* UNDEAD */ {  1f,    1f,     1f,   1f,   2f,   2f,   1f },
            /* GHOST */  {  1f,    1f,     2f, 0.5f,   1f,   1f,   1f },
            /* GIANT */  {  1f,    2f,     1f,   1f,   1f, 0.5f, 0.5f },
            /* ANIMAL */ {  1f,  0.5f,   0.5f,   2f,   1f,   1f,   2f },
            /* AIR */    {  1f,    1f,     2f,   1f,   1f, 0.5f,   1f },
        };

        public float CalculateEffectiveness(Type attackType)
        {
            // Find index of attack type
            int[] attackIndexes = GetTypeIndexes(attackType);

            // Find index of defence type
            int[] defenceIndexes = GetTypeIndexes(Type);

            // Get multiplier
            float percent = 100f;

            foreach (int attackIndex in attackIndexes)
            {
                foreach (int defenceIndex in defenceIndexes)
                    percent *= TYPE_CHART[attackIndex, defenceIndex];
            }

            // Limit between 50% and 500%
            if (percent < 50f)
                percent = 50f;
            else if (percent > 500f)
                percent = 500f;

            return percent;
        }

        private static int[] GetTypeIndexes(Type type)
        {
            Array types = Enum.GetValues(typeof(Type));
            List<int> indexes = new List<int>();

            for (int i = 0; i < types.Length; i++)
            {
                Type item = (Type)types.GetValue(i);

                if ((type & item) != 0)
                    indexes.Add(i);
            }

            return indexes.ToArray();
        }

        #endregion
    }
}