using System;
using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using Utils;

namespace Dungeon.Drawers.Rooms
{
	/// <summary>
	/// Drawer meant to draw room types
	/// </summary>
	public abstract class RoomDrawer : Drawer
	{
		/// <summary>
		/// Room type handled by this drawer
		/// </summary>
		protected abstract RoomType Type { get; }

		protected RoomDrawer(DungeonResult level) : base(level)
		{
		}

		/// <summary>
		/// Draws in the given room
		/// </summary>
		protected abstract void OnDraw(Room room);
		
		/// <summary>
		/// Processes the given room
		/// </summary>
		protected abstract void OnProcess(Room room);

		#region Utils

		/// <summary>
		/// Fetches a list of every position that meet the given condition in a room
		/// </summary>
		protected List<Vector2Int> GetValidPositions(Room room, Func<int, int, bool> predicate = null)
		{
			// Find valid positions
			List<Vector2Int> positions = new();

			for (int y = room.Y; y < room.Y + room.Height; y++)
			{
				for (int x = room.X; x < room.X + room.Width; x++)
				{
					// If the tile is blocked, skip
					if (Level.IsBlocked(x, y))
						continue;

					if (predicate != null && !predicate.Invoke(x, y))
						continue;

					positions.Add(new Vector2Int(x, y));
				}
			}

			return positions;
		}

		#endregion

		#region Drawer

		/// <inheritdoc/>
		public override void Draw(Room[] rooms)
		{
			foreach (Room room in rooms)
			{
				if (room.Type != Type)
					continue;

				OnDraw(room);
			}
		}

		/// <inheritdoc/>
		public override void Process(Room[] rooms)
		{
			foreach (Room room in rooms)
			{
				if (room.Type != Type)
					continue;

				OnProcess(room);
			}
		}

		#endregion
	}
}