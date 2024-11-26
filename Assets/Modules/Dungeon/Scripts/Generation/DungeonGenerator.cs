using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Dungeon.Generation
{
    public class DungeonGenerator
    {
        private Random random;

        public DungeonGenerator(Random random)
        {
            this.random = random;
        }

        public DungeonResult Generate(int width, int height) => GenerateRooms(width, height, 100);

        #region Rooms

        private const int MINIMUM_ROOM_WIDTH = 3;
        private const int MINIMUM_ROOM_HEIGHT = 3;

        private DungeonResult GenerateRooms(int width, int height, int sliceCount)
        {
            var root = new Room
            {
                Width = width,
                Height = height
            };

            // Slices the root into smaller rooms
            SlicesRooms(root, sliceCount);

            // Compile all the rooms
            List<Room> allRooms = new();
            List<Room> endRooms = new();
            Stack<Room> exploreRooms = new();
            exploreRooms.Push(root);

            while (exploreRooms.Count > 0)
            {
                var room = exploreRooms.Pop();
                allRooms.Add(room);

                // If has children, keep going
                if (room.Children != null)
                {
                    foreach (var item in room.Children)
                        exploreRooms.Push(item);
                    continue;
                }

                endRooms.Add(room);
            }

            endRooms.Reverse();

            Room[] rooms = endRooms.ToArray();

            // Pushes the rooms to let a gap between the rooms
            PushRooms(rooms);

            // Package results
            var result = new DungeonResult
            {
                Rooms = rooms,
            };

            return result;
        }

        /// <summary>
        /// Slices the given room into smaller rooms
        /// </summary>
        private void SlicesRooms(Room root, int sliceCount)
        {
            var rooms = new Queue<Room>();
            rooms.Enqueue(root);

            var slicesLeft = sliceCount;

            while (rooms.Count > 0 && slicesLeft > 0)
            {
                var room = rooms.Dequeue();

                // Split
                bool sliced = room.Split(random, MINIMUM_ROOM_WIDTH, MINIMUM_ROOM_HEIGHT);

                if (sliced)
                {
                    foreach (var item in room.Children)
                        rooms.Enqueue(item);
                    slicesLeft--;
                }
            }
        }

        /// <summary>
        /// Expands the rooms to allow walls
        /// </summary>
        private void PushRooms(Room[] rooms)
        {
            var sortedRooms = rooms.OrderBy(r => r.X).ThenBy(r => r.Y).ToArray();

            // Push left
            foreach (var roomPushing in sortedRooms)
            {
                foreach (var roomToPush in rooms)
                {
                    // If pushing self, skip
                    if (roomToPush == roomPushing)
                        continue;

                    // If room is left, skip
                    if (roomToPush.X < roomPushing.X)
                        continue;

                    // If room is under, skip
                    if (roomToPush.Y >= roomPushing.Y + roomPushing.Height)
                        continue;

                    // If room is above, skip
                    if (roomToPush.Y < roomPushing.Y)
                        continue;

                    // bool isRight = false;

                    // for (int i = 0; i < roomToPush.Height; i++)
                    // {
                    //     if (roomToPush.Y + i > roomPushing.Y && roomToPush.Y + i < roomPushing.Y + roomPushing.Height)
                    //     {
                    //         isRight = true;
                    //         break;
                    //     }
                    // }

                    // if (!isRight)
                    //     continue;

                    //UnityEngine.Debug.Log($"{roomPushing} pushed {roomToPush} left");
                    roomToPush.X++;
                }
            }

            sortedRooms = rooms.OrderBy(r => r.Y).ThenBy(r => r.X).ToArray();

            // Push down
            foreach (var roomPushing in sortedRooms)
            {
                foreach (var roomToPush in rooms)
                {
                    // If pushing self, skip
                    if (roomToPush == roomPushing)
                        continue;

                    // If room is above, skip
                    if (roomToPush.Y < roomPushing.Y)
                        continue;

                    // If room is right, skip
                    if (roomToPush.X > roomPushing.X + roomPushing.Width)
                        continue;

                    if (roomToPush.X < roomPushing.X)
                        continue;

                    // bool isUnder = false;

                    // for (int i = 0; i < roomToPush.Width; i++)
                    // {
                    //     if (roomToPush.X + i >= roomPushing.X && roomToPush.X + i <= roomPushing.X + roomPushing.Width)
                    //     {
                    //         isUnder = true;
                    //         break;
                    //     }
                    // }

                    // if (!isUnder)
                    //     continue;

                    //UnityEngine.Debug.Log($"{roomPushing} pushed {roomToPush} down");
                    roomToPush.Y++;
                }
            }
        }

        #endregion
    }

    public class DungeonResult
    {
        /// <summary>
        /// All the final rooms in the dungeon
        /// </summary>
        public Room[] Rooms;

        public Random Random;

        public bool[,] WallGrid;

        public bool[,] GroundGrid;

        public bool[,] DoorGrid;

        public bool[,] EntranceExitGrid;
    }
}