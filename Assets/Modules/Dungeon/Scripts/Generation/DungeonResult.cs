using System;
using System.Collections.Generic;

namespace Dungeon.Generation
{
    [Flags]
    public enum Tile
    {
        NONE = 0,
        GROUND = 1 << 1,
        // 1 << 2
        // 1 << 3
        // 1 << 4
        WALL = 1 << 5,
        // 1 << 6
        // 1 << 7
        // 1 << 8
        DOOR_CLOSED = 1 << 9,
        DOOR_OPENED = 1 << 10,
        // 1 << 11
        // 1 << 12
        ENTRANCE = 1 << 13,
        EXIT = 1 << 14
    }

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

        /// <summary>
        /// Tiles for the dungeon
        /// </summary>
        public Tile[,] Grid;
    }
}