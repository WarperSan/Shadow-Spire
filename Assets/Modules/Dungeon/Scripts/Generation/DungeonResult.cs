using System;
using System.Collections.Generic;
using Entities.Grid.Entities;
using PathFinding.Graphs;

namespace Dungeon.Generation
{
	[Flags]
	public enum Tile
	{
		None = 0,
		Ground = 1 << 1,
		CoveredGround = 1 << 2,

		// 1 << 3
		// 1 << 4
		Wall = 1 << 5,

		// 1 << 6
		// 1 << 7
		// 1 << 8
		DoorClosed = 1 << 9,
		DoorOpened = 1 << 10,

		// 1 << 11
		// 1 << 12
		Entrance = 1 << 13,
		Exit = 1 << 14,

		// 1 << 15
		// 1 << 16
		Player = 1 << 17,
		Enemy = 1 << 18,

		// 1 << 19
		// 1 << 20
		// 1 << 21
		// 1 << 22
		Treasure = 1 << 23,
		Spikes = 1 << 24
	}

	/// <summary>
	/// Result of the dungeon generation
	/// </summary>
	public class DungeonResult
	{
		#region Rooms

		/// <summary>
		/// All the final rooms in the dungeon
		/// </summary>
		public Room[] Rooms;

		/// <summary>
		/// Every rooms that is adjacent to a room
		/// </summary>
		public Dictionary<Room, HashSet<Room>> AdjacentRooms;

		/// <summary>
		/// Room that serves as the entrance of the dungeon
		/// </summary>
		public Room Entrance;

		/// <summary>
		/// Room that serves as the exit of the dungeon
		/// </summary>
		public Room Exit;

		#endregion

		#region Level

		/// <summary>
		/// Current level of the dungeon
		/// </summary>
		public int Index;

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

		#endregion

		#region Tiles

		/// <summary>
		/// Tiles for the dungeon
		/// </summary>
		public Tile[,] Grid;

		/// <summary>
		/// Graph of all the tiles for the dungeon
		/// </summary>
		public TileGraph TileGraph;

		#endregion

		public PlayerEntity Player;
	}
}