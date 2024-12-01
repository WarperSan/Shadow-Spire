using System.Collections.Generic;
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

        #region Rooms

        public DungeonResult Generate(int width, int height, int sliceCount, int minRoomWidth, int minRoomHeight)
        {
            var root = new Room
            {
                Width = width,
                Height = height
            };

            // Slices the root into smaller rooms
            SlicesRooms(root, sliceCount, minRoomWidth, minRoomHeight);

            // Compile all the rooms
            Room[] rooms = CompileRooms(root);

            // Find adjacent rooms
            Dictionary<Room, List<Room>> adjacentRoom = FindAdjacentRooms(rooms);

            // Package results
            return new DungeonResult
            {
                Rooms = rooms,
                Width = width,
                Height = height,
                AdjacentRooms = adjacentRoom
            };
        }

        /// <summary>
        /// Slices the given room into smaller rooms
        /// </summary>
        private void SlicesRooms(Room root, int sliceCount, int minRoomWidth, int minRoomHeight)
        {
            var rooms = new Queue<Room>();
            rooms.Enqueue(root);

            var slicesLeft = sliceCount;

            while (rooms.Count > 0 && slicesLeft > 0)
            {
                var room = rooms.Dequeue();

                // Split
                bool sliced = room.Split(random, minRoomWidth, minRoomHeight);

                if (sliced)
                {
                    foreach (var item in room.Children)
                        rooms.Enqueue(item);
                    slicesLeft--;
                }
            }
        }

        /// <summary>
        /// Finds all the rooms from the given root
        /// </summary>
        private Room[] CompileRooms(Room root)
        {
            Stack<Room> roomsToExplore = new();
            roomsToExplore.Push(root);

            List<Room> rooms = new();

            while (roomsToExplore.Count > 0)
            {
                var room = roomsToExplore.Pop();

                // If has children, keep going
                if (room.Children != null)
                {
                    foreach (var item in room.Children)
                        roomsToExplore.Push(item);
                    continue;
                }

                rooms.Add(room);
            }

            rooms.Reverse();

            return rooms.ToArray();
        }

        /// <summary>
        /// Creates a map of all the rooms and all the rooms they are connected to
        /// </summary>
        private Dictionary<Room, List<Room>> FindAdjacentRooms(Room[] rooms)
        {
            var roomLinks = new Dictionary<Room, List<Room>>();

            foreach (var firstRoom in rooms)
            {
                foreach (var secondRoom in rooms)
                {
                    // If same room, skip
                    if (firstRoom == secondRoom)
                        continue;

                    if (!firstRoom.IsAdjacent(secondRoom))
                        continue;

                    if (!roomLinks.ContainsKey(firstRoom))
                        roomLinks.Add(firstRoom, new());

                    if (!roomLinks.ContainsKey(secondRoom))
                        roomLinks.Add(secondRoom, new());

                    roomLinks[firstRoom].Add(secondRoom);
                    roomLinks[secondRoom].Add(firstRoom);
                }
            }

            return roomLinks;
        }

        #endregion
    }
}