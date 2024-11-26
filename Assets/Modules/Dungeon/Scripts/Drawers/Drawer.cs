using System;
using Dungeon.Generation;
using UnityEngine;

namespace Dungeon.Drawers
{
    public abstract class Drawer : MonoBehaviour
    {
        public abstract bool[,] Process(Room[] rooms);
        public abstract void Draw(bool[,] grid, Room[] rooms);

        /// <summary>
        /// Creates an empty grid with the given rooms
        /// </summary>
        protected bool[,] CreateEmpty(Room[] rooms)
        {
            // Calculate size
            int minX = int.MaxValue;
            int maxX = int.MinValue;

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (var room in rooms)
            {
                minX = Math.Min(minX, room.X);
                maxX = Math.Max(maxX, room.X + room.Width);

                minY = Math.Min(minY, room.Y);
                maxY = Math.Max(maxY, room.Y + room.Height);
            }

            return new bool[maxY - minY + 2, maxX - minX + 2];
        }
    }
}