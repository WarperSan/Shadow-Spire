using System.Collections.Generic;
using Dungeon.Drawers;
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
            Room entrance = EntranceExitDrawer.FindEntrance(rooms);
            Room exit = EntranceExitDrawer.FindExit(rooms, entrance);

            // Find adjacent rooms
            Dictionary<Room, List<Room>> adjacentRooms = FindAdjacentRooms(rooms);
            CutConnections(rooms, adjacentRooms, entrance);

            // Package results
            return new DungeonResult
            {
                Rooms = rooms,
                AdjacentRooms = adjacentRooms,
                Entrance = entrance,
                Exit = exit,
                Width = width,
                Height = height,
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

        /// <summary>
        /// Removes some doors to make a path of rooms
        /// </summary>
        private void CutConnections(Room[] rooms, Dictionary<Room, List<Room>> adjacentRooms, Room entrance)
        {
            // Initialize all the depths to -1
            foreach (var room in rooms)
                room.Depth = -1;

            // Set the start depth to 0
            entrance.Depth = 0;

            Stack<Room> roomsToExplore = new();
            roomsToExplore.Push(entrance);

            while (roomsToExplore.Count > 0)
            {
                var currentRoom = roomsToExplore.Peek();

                // Find all the neighbors that have not been explored
                var neighbors = new List<Room>();

                foreach (var adjacentRoom in adjacentRooms[currentRoom])
                {
                    if (adjacentRoom.Depth != -1)
                        continue;

                    neighbors.Add(adjacentRoom);
                }

                // If no unexplored neighbor, continue
                if (neighbors.Count == 0)
                {
                    roomsToExplore.Pop();
                    continue;
                }

                // Pick a random unexplored neighbor
                var nextRoom = neighbors[random.Next(0, neighbors.Count)];

                // Increase it's depth by 1
                nextRoom.Depth = currentRoom.Depth + 1;

                // Add next room to the stack
                roomsToExplore.Push(nextRoom);
            }

            // Find the room with the highest depth
            Room furthestRoom = rooms[0];

            for (int i = 1; i < rooms.Length; i++)
            {
                if (rooms[i].Depth > furthestRoom.Depth)
                    furthestRoom = rooms[i];
            }

            // Find the neighbor with the lowest depth
            List<Room> furthestRoomNeighbors = adjacentRooms[furthestRoom];
            Room lowestNeighbor = furthestRoomNeighbors[0];

            for (int i = 1; i < furthestRoomNeighbors.Count; i++)
            {
                if (furthestRoomNeighbors[i].Depth < lowestNeighbor.Depth)
                    lowestNeighbor = furthestRoomNeighbors[i];
            }

            // Remove extra doors
            foreach (var (room, otherRooms) in adjacentRooms)
            {
                for (int i = otherRooms.Count - 1; i >= 0; i--)
                {
                    // If the difference is higher than 1
                    if (Mathf.Abs(room.Depth - otherRooms[i].Depth) > 1)
                        otherRooms.RemoveAt(i);
                }
            }

            // Create a door between them
            adjacentRooms[furthestRoom].Add(lowestNeighbor);
            adjacentRooms[lowestNeighbor].Add(furthestRoom);
        }

        #endregion
    }
}