using System;
using UnityEngine;

namespace Dungeon.Generation
{
	/// <summary>
	/// Types of rooms that compose a dungeon
	/// </summary>
	public enum RoomType
	{
		Normal,
		Entrance, // Room that is the entrance
		Exit,     // Room that is the exit
		Enemy,    // Room that spawns monsters
		Treasure, // Room that spawns a treasure
		Spikes    // Room that spawns spikes
	}

	/// <summary>
	/// Class holding the information for a single room
	/// </summary>
	public class Room
	{
		public int X { get; set; }
		public int Y { get; set; }

		public int Width  { get; set; }
		public int Height { get; set; }

		public int      Depth { get; set; }
		public RoomType Type  { get; set; } = RoomType.Normal;

		/// <summary>
		/// List of rooms that compose this room
		/// </summary>
		public Room[] Children { get; private set; }

		/// <summary>
		/// Splits this room into two smaller rooms
		/// </summary>
		public bool Split(System.Random rand, int minWidth, int minHeight)
		{
			Room roomA = new();
			Room roomB = new();

			bool hasSplit = false;

			for (int i = 0; i < 10; i++)
			{
				bool splitVertical = rand.Next(0, 2) == 0;
				double percent = rand.NextDouble() * 0.4f + 0.3f; // 30-70%

				int newWidth = (int)Math.Floor(Width * percent);
				int newHeight = (int)Math.Floor(Height * percent);

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

					hasSplit = true;
					break;
				}

				if (newHeight >= minHeight && Height - newHeight >= minHeight)
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
					hasSplit = true;
					break;
				}
			}

			if (!hasSplit)
				return false;

			Children = new[] { roomA, roomB };

			return true;
		}

		/// <summary>
		/// Checks if the given room is adjacent to this room
		/// </summary>
		public bool IsAdjacent(Room other) => IsUnder(other) || IsBeside(other);

		/// <summary>
		/// Checks if the given room is under this room
		/// </summary>
		public bool IsUnder(Room other)
		{
			Vector2Int selfMax = new(X + Width - 1, Y + Height - 1);
			Vector2Int otherMax = new(other.X + other.Width - 1, other.Y + other.Height - 1);

			return other.Y - selfMax.y == 1 &&            // The rooms only are 1 tile apart 
			       otherMax.x > X && other.X < selfMax.x; // The rooms have a common X point
		}

		/// <summary>
		/// Checks if the given room is beside this room
		/// </summary>
		public bool IsBeside(Room other)
		{
			Vector2Int selfMax = new(X + Width - 1, Y + Height - 1);
			Vector2Int otherMax = new(other.X + other.Width - 1, other.Y + other.Height - 1);

			return other.X - selfMax.x == 1 &&            // The rooms only are 1 tile apart
			       otherMax.y > Y && other.Y < selfMax.y; // The rooms have a common Y point
		}

		/// <inheritdoc/>
		public override string ToString() => $"[{X};{Y} ({Width}x{Height})]";
	}
}