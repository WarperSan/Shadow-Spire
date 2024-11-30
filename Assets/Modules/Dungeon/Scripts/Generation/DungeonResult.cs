using System;
using System.Collections.Generic;

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
        /// Every rooms that is adjacent to a room
        /// </summary>
        public Dictionary<Room, List<Room>> AdjacentRooms;

        public bool[,] WallGrid;

        public bool[,] GroundGrid;

        public bool[,] DoorGrid;

        public bool[,] EntranceExitGrid;
    }
}