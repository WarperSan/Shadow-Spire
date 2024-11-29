using System;
using System.Collections.Generic;

namespace Dungeon.Generation
{
    public class Room
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Room[] Children { get; private set; }
        public bool FromVerticalSlice { get; private set; }
        public int[] Doors { get; set; }

        public bool Split(Random rand, int minWidth, int minHeight)
        {
            var roomA = new Room();
            var roomB = new Room();

            bool hasSplitted = false;
            bool splitVertical = false;

            for (int i = 0; i < 10; i++)
            {
                splitVertical = rand.Next(0, 2) == 0;
                var percent = rand.NextDouble() * 0.4f + 0.3f; // 30-70%

                var newWidth = (int)Math.Floor(Width * percent);
                var newHeight = (int)Math.Floor(Height * percent);

                // If splitting vertically makes two big enough room
                if (splitVertical && newWidth >= minWidth && Width - newWidth >= minWidth)
                {
                    roomA.X = X;
                    roomA.Y = Y;
                    roomA.Width = newWidth;
                    roomA.Height = Height;

                    roomB.X = X + newWidth;
                    roomB.Y = Y;
                    roomB.Width = Width - newWidth;
                    roomB.Height = Height;

                    hasSplitted = true;
                    break;
                }
                else if (newHeight >= minHeight && Height - newHeight >= minHeight)
                {
                    roomA.X = X;
                    roomA.Y = Y;
                    roomA.Width = Width;
                    roomA.Height = newHeight;

                    roomB.X = X;
                    roomB.Y = Y + newHeight;
                    roomB.Width = Width;
                    roomB.Height = Height - newHeight;

                    splitVertical = false; // Vertical can fail
                    hasSplitted = true;
                    break;
                }
            }

            if (!hasSplitted)
                return false;

            roomA.GenerateDoors(rand, splitVertical);

            roomA.FromVerticalSlice = splitVertical;
            roomB.FromVerticalSlice = splitVertical;

            Children = new Room[] { roomA, roomB };

            return true;
        }

        private void GenerateDoors(Random rand, bool splitVertical)
        {
            // Generate doors
            var cutLength = splitVertical ? Width : Height;
            var doors = new int[cutLength / 3];

            //UnityEngine.Debug.Log(doors.Length);

            // Add possible spots
            var indexes = new List<int>();
            for (var i = 1; i < cutLength - 2; i++)
                indexes.Add(i);

            for (var i = 0; indexes.Count > 0 && i < doors.Length; i++)
            {
                var rdmIndex = rand.Next(0, indexes.Count);
                doors[i] = indexes[rdmIndex];

                // Remove after
                if (rdmIndex < indexes.Count - 1)
                    indexes.RemoveAt(rdmIndex + 1);

                // Remove self
                indexes.RemoveAt(rdmIndex);

                // Remove before
                if (rdmIndex > 0)
                    indexes.RemoveAt(rdmIndex - 1);
            }

            Doors = doors;
        }

        public override string ToString()
        {
            return $"[{X};{Y} ({Width}x{Height})]";
        }
    }
}