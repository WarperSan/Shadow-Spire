using System;
using System.Collections.Generic;
using Dungeon.Drawers.Terrain;
using UnityEngine;
using Random = System.Random;

namespace Dungeon.Generation
{
	/// <summary>
	/// Class that handles to generate the dungeon
	/// </summary>
	public class DungeonGenerator
	{
		private readonly Random _random;
		private readonly DungeonSettings _settings;

		private DungeonGenerator(Random random, DungeonSettings settings)
		{
			_random = random;
			_settings = settings;
			_settings.MinimumRoomWidth++;  // Add right wall
			_settings.MinimumRoomHeight++; // Add down wall
		}

		/// <summary>
		/// Generates a dungeon based off the given settings
		/// </summary>
		public static DungeonResult Generate(Random random, DungeonSettings settings) => new DungeonGenerator(random, settings).Generate();

		private DungeonResult Generate()
		{
			Room root = new()
			{
				Width = _settings.Width,
				Height = _settings.Height
			};

			// Slices the root into smaller rooms
			SlicesRooms(root,
				_settings.SliceCount,
				_settings.MinimumRoomWidth,
				_settings.MinimumRoomHeight);

			// Compile all the rooms
			Room[] rooms = CompileRooms(root);

			Room entrance = EntranceExitDrawer.FindEntrance(rooms);
			entrance.Type = RoomType.Entrance;

			Room exit = EntranceExitDrawer.FindExit(rooms, entrance);
			exit.Type = RoomType.Exit;

			// Find adjacent rooms
			Dictionary<Room, HashSet<Room>> adjacentRooms = FindAdjacentRooms(rooms);

			if (rooms.Length > 1)
				CutConnections(rooms, adjacentRooms, entrance);

			// Find room types
			FindEnemyRooms(rooms);
			FindTreasureRooms(rooms);
			FindSpikesRooms(rooms);

			// Package results
			return new DungeonResult
			{
				Index = _settings.Index,
				Rooms = rooms,
				AdjacentRooms = adjacentRooms,
				Entrance = entrance,
				Exit = exit,
				Width = _settings.Width,
				Height = _settings.Height
			};
		}

		#region Rooms

		/// <summary>
		/// Slices the given room into smaller rooms
		/// </summary>
		private void SlicesRooms(
			Room root,
			int  sliceCount,
			int  minRoomWidth,
			int  minRoomHeight
		)
		{
			Queue<Room> rooms = new();
			rooms.Enqueue(root);

			int slicesLeft = sliceCount;

			while (rooms.Count > 0 && slicesLeft > 0)
			{
				Room room = rooms.Dequeue();

				// Split
				bool sliced = room.Split(_random, minRoomWidth, minRoomHeight);

				if (sliced)
				{
					foreach (Room item in room.Children)
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
				Room room = roomsToExplore.Pop();

				// If the room has children, keep going
				if (room.Children != null)
				{
					foreach (Room item in room.Children)
						roomsToExplore.Push(item);
					continue;
				}

				rooms.Add(room);
			}

			rooms.Reverse();

			return rooms.ToArray();
		}

		#endregion

		#region Doors

		/// <summary>
		/// Creates a map of all the rooms and all the rooms they are connected to
		/// </summary>
		private Dictionary<Room, HashSet<Room>> FindAdjacentRooms(Room[] rooms)
		{
			Dictionary<Room, HashSet<Room>> roomLinks = new();

			foreach (Room firstRoom in rooms)
			{
				foreach (Room secondRoom in rooms)
				{
					// If same room, skip
					if (firstRoom == secondRoom)
						continue;

					if (!firstRoom.IsAdjacent(secondRoom))
						continue;

					if (!roomLinks.ContainsKey(firstRoom))
						roomLinks.Add(firstRoom, new HashSet<Room>());

					if (!roomLinks.ContainsKey(secondRoom))
						roomLinks.Add(secondRoom, new HashSet<Room>());

					roomLinks[firstRoom].Add(secondRoom);
					roomLinks[secondRoom].Add(firstRoom);
				}
			}

			return roomLinks;
		}

		/// <summary>
		/// Removes some doors to make a path of rooms
		/// </summary>
		private void CutConnections(Room[] rooms, Dictionary<Room, HashSet<Room>> adjacentRooms, Room entrance)
		{
			// Initialize all the depths to -1
			foreach (Room room in rooms)
				room.Depth = -1;

			// Set the start depth to 0
			entrance.Depth = 0;

			Stack<Room> roomsToExplore = new();
			roomsToExplore.Push(entrance);

			while (roomsToExplore.Count > 0)
			{
				Room currentRoom = roomsToExplore.Peek();

				// Find all the neighbors that have not been explored
				List<Room> neighbors = new();

				foreach (Room adjacentRoom in adjacentRooms[currentRoom])
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
				Room nextRoom = neighbors[_random.Next(0, neighbors.Count)];

				// Increase its depth by 1
				nextRoom.Depth = currentRoom.Depth + 1;

				// Add next room to the stack
				roomsToExplore.Push(nextRoom);
			}

			(Room, Room) highLoop = CreateLoop(
				rooms,
				adjacentRooms,
				(c, f) => c.Depth > f.Depth,
				(c, f) => c.Depth < f.Depth
			);

			(Room, Room) lowLoop = CreateLoop(
				rooms,
				adjacentRooms,
				(c, f) => c.Depth < f.Depth,
				(c, f) => c.Depth > f.Depth
			);

			// Remove extra doors
			foreach ((Room room, HashSet<Room> otherRooms) in adjacentRooms)
			{
				IEnumerator<Room> currentRooms = otherRooms.GetEnumerator();
				List<Room> roomsToRemove = new();

				while (currentRooms.MoveNext() && currentRooms.Current != null)
				{
					// If the difference is higher than 1
					if (Mathf.Abs(room.Depth - currentRooms.Current.Depth) > 1)
						roomsToRemove.Add(currentRooms.Current);
				}
				currentRooms.Dispose();

				foreach (Room item in roomsToRemove)
					otherRooms.Remove(item);
			}

			// Create the door between the loops
			if (_settings.AddHighLoop)
			{
				adjacentRooms[highLoop.Item1].Add(highLoop.Item2);
				adjacentRooms[highLoop.Item2].Add(highLoop.Item1);
			}

			if (_settings.AddLowLoop)
			{
				adjacentRooms[lowLoop.Item1].Add(lowLoop.Item2);
				adjacentRooms[lowLoop.Item2].Add(lowLoop.Item1);
			}
		}

		private (Room, Room) CreateLoop(
			Room[]                          rooms,
			Dictionary<Room, HashSet<Room>> adjacentRooms,
			Func<Room, Room, bool>          roomToStartFrom,
			Func<Room, Room, bool>          neighborToChoose
		)
		{
			// Find the room with the highest depth
			Room furthestRoom = rooms[0];

			for (int i = 1; i < rooms.Length; i++)
			{
				if (roomToStartFrom.Invoke(rooms[i], furthestRoom))
					furthestRoom = rooms[i];
			}

			// Find the neighbor with the lowest depth
			IEnumerator<Room> furthestRoomNeighbors = adjacentRooms.GetValueOrDefault(furthestRoom).GetEnumerator();
			furthestRoomNeighbors.MoveNext();

			Room lowestNeighbor = furthestRoomNeighbors.Current;

			while (furthestRoomNeighbors.MoveNext())
			{
				if (neighborToChoose.Invoke(furthestRoomNeighbors.Current, lowestNeighbor))
					lowestNeighbor = furthestRoomNeighbors.Current;
			}
			furthestRoomNeighbors.Dispose();

			return (furthestRoom, lowestNeighbor);
		}

		#endregion

		#region Type

		public const int ENEMY_ROOM_INDEX = 3;    // Level 3+
		public const int TREASURE_ROOM_INDEX = 6; // Level 6+
		public const int SPIKES_ROOM_INDEX = 4;   // Level 4+

		private void FindEnemyRooms(Room[] rooms)
		{
			if (_settings.Index < ENEMY_ROOM_INDEX)
				return;

			List<Room> validRooms = new();

			foreach (Room room in rooms)
			{
				// If too close from the entrance, skip
				if (room.Depth <= 1)
					continue;

				validRooms.Add(room);
			}

			// If not valid room, skip
			if (validRooms.Count == 0)
				return;

			int count = rooms.Length / 3;

			if (count >= validRooms.Count)
				return;

			for (int i = 0; i < count; i++)
			{
				int rdmIndex = _random.Next(0, validRooms.Count);
				(validRooms[^1], validRooms[rdmIndex]) = (validRooms[rdmIndex], validRooms[^1]);
			}

			for (int i = 0; i < count; i++)
				validRooms[^(count + 1)].Type = RoomType.Enemy;
		}

		private void FindTreasureRooms(Room[] rooms)
		{
			if (_settings.Index < TREASURE_ROOM_INDEX)
				return;

			// Find deepest
			Room deepest = null;

			foreach (Room room in rooms)
			{
				if (room.Type != RoomType.Normal)
					continue;

				if (deepest == null || room.Depth > deepest.Depth)
					deepest = room;
			}

			if (deepest == null)
				return;

			deepest.Type = RoomType.Treasure;
		}

		private void FindSpikesRooms(Room[] rooms)
		{
			if (_settings.Index < SPIKES_ROOM_INDEX)
				return;

			// Find random
			Room randomRoom = null;

			foreach (Room room in rooms)
			{
				if (room.Type != RoomType.Normal)
					continue;

				if (randomRoom == null)
				{
					randomRoom = room;
					continue;
				}

				// 50% replace 50% keep
				if (_random.Next(0, 2) == 0)
					randomRoom = room;
			}

			if (randomRoom == null)
				return;

			randomRoom.Type = RoomType.Spikes;
		}

		#endregion
	}
}