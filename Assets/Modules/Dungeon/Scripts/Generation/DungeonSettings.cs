namespace Dungeon.Generation
{
    public struct DungeonSettings
    {
        /// <summary>
        /// Seed used for the random elements
        /// </summary>
        public int Seed;

        /// <summary>
        /// Total width of the dungeon
        /// </summary>
        public int Width;

        /// <summary>
        /// Total height of the dungeon
        /// </summary>
        public int Height;

        /// <summary>
        /// Minimum width of a room
        /// </summary>
        public int MinimumRoomWidth;

        /// <summary>
        /// Minimum height of a room
        /// </summary>
        public int MinimumRoomHeight;

        /// <summary>
        /// Maximum amount of slices of the dungeon
        /// </summary>
        public int SliceCount;
    }
}