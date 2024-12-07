using System;

namespace BattleEntity
{
    [Flags]
    public enum BattleEntityType 
    { 
        NONE = 0,
        NORMAL = 1, 
        UNDEAD = 2, 
        GHOST = 4, 
        GIANT = 8,
        ANIMAL = 16,
    }
    public abstract class BattleEntity
    {
        public int Health { get; protected set; }
        public int Attack { get; protected set; }
        public BattleEntityType Type { get; protected set; }
    }

}
