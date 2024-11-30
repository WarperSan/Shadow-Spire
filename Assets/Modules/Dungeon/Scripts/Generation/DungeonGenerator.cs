using System.Collections.Generic;
using System.Linq;
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

        public DungeonResult Generate(int width, int height, int sliceCount) => GenerateRooms(width, height, sliceCount);

        #region Rooms

        private const int MINIMUM_ROOM_WIDTH = 5;
        private const int MINIMUM_ROOM_HEIGHT = 5;

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
            //PushRooms(rooms);

            // Package results
            var result = new DungeonResult
            {
                Rooms = rooms,
                Width = width,
                Height = height
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
            Dictionary<Room, int> push = new();

            // Push right
            for (int i = 0; i < rooms.Length; i++)
            {
                var roomPushing = rooms[i];

                for (int j = i + 1; j < rooms.Length; j++)
                {
                    var roomToPush = rooms[j];

                    var pushingMin = roomPushing.Y;
                    var pushingMax = roomPushing.Y + roomPushing.Height;

                    var pushedMin = roomToPush.Y;
                    var pushedMax = roomToPush.Y + roomToPush.Height;

                    if ((pushedMax > pushingMin) && (pushedMin < pushingMax))
                    {
                        if (push.TryGetValue(roomToPush, out int x) && x >= roomPushing.X)
                            continue;

                        roomToPush.X++;
                        push[roomToPush] = roomPushing.X;
                    }
                }
            }

            push.Clear();

            var sortedRooms = rooms.OrderBy(r => r.Y).ThenBy(r => r.X).ToArray();

            // Push down
            for (int i = 0; i < sortedRooms.Length; i++)
            {
                var roomPushing = rooms[i];

                for (int j = i + 1; j < sortedRooms.Length; j++)
                {
                    var roomToPush = rooms[j];

                    var pushingMin = roomPushing.X;
                    var pushingMax = roomPushing.X + roomPushing.Width;

                    var pushedMin = roomToPush.X;
                    var pushedMax = roomToPush.X + roomToPush.Width;

                    if ((pushedMax > pushingMin) && (pushedMin < pushingMax))
                    {
                        if (push.TryGetValue(roomToPush, out int y) && y >= roomPushing.Y)
                            continue;

                        roomToPush.Y++;
                        push[roomToPush] = roomPushing.Y;
                    }
                }
            }
        }

        #endregion
    }
}