using System;

namespace Dungeon.Generation
{
    public class DungeonResult
    {
        /// <summary>
        /// All the final rooms in the dungeon
        /// </summary>
        public Room[] Rooms;

        /// <summary>
        /// The element used for random generation
        /// </summary>
        public Random Random;

        /// <summary>
        /// Width of the entire dungeon
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the entire dungeon
        /// </summary>
        public int Height;

        /// <summary>
        /// </summary>
        public bool[,] WallGrid;

        public bool[,] GroundGrid;

        public bool[,] DoorGrid;

        public bool[,] EntranceExitGrid;
    }
}