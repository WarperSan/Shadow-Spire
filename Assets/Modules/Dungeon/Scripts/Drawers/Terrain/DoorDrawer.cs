using System;
using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Dungeon.Drawers.Terrain
{
	public class DoorDrawer : Drawer
	{
		private readonly Tilemap wallMap;
		private readonly TileBase openedDoorTile;
		private readonly TileBase closedDoorTile;

		#region Drawer

		public DoorDrawer(
			DungeonResult level,
			Tilemap       wallMap,
			TileBase      openedDoorTile,
			TileBase      closedDoorTile
		) : base(level)
		{
			this.wallMap = wallMap;
			this.openedDoorTile = openedDoorTile;
			this.closedDoorTile = closedDoorTile;
		}

		/// <inheritdoc/>
		public override void Draw(Room[] rooms)
		{
			int height = Level.Grid.GetLength(0);
			int width = Level.Grid.GetLength(1);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (!Level.HasDoor(x, y))
						continue;

					wallMap.SetTile(new Vector3Int(x, -y, 0),
						Level.Has(x, y, Generation.Tile.DOOR_OPENED) ? openedDoorTile : closedDoorTile);
				}
			}
		}

		/// <inheritdoc/>
		public override void Process(Room[] rooms)
		{
			System.Random random = Level.Random;

			List<(Room, Room)> processedLinks = new();

			foreach ((Room room, HashSet<Room> adjacents) in Level.AdjacentRooms)
			{
				foreach (Room adjacent in adjacents)
				{
					// If already processed, skip
					if (processedLinks.Contains((adjacent, room)))
						continue;

					if (adjacent.X < room.X + room.Width && adjacent.Y < room.Y + room.Height)
						continue;

					processedLinks.Add((room, adjacent));

					bool isDoorClosed = false; //Mathf.Abs(room.Depth - adjacent.Depth) > 1;
					bool isVerticalWall = adjacent.X >= room.X + room.Width;

					int min = isVerticalWall
						? Mathf.Max(room.Y, adjacent.Y)
						: Mathf.Max(room.X, adjacent.X);

					int max = isVerticalWall
						? Mathf.Min(room.Y + room.Height - 1, adjacent.Y + adjacent.Height - 1)
						: Mathf.Min(room.X + room.Width - 1, adjacent.X + adjacent.Width - 1);

					if (CanRoomsCombine(room.Type, adjacent.Type) && random.NextDouble() < 0.25f)
					{
						RemoveWall(room, adjacent);
						continue;
					}

					if (max - min >= 2)
					{
						min++;
						max--;
					}

					int x = adjacent.X - 1;
					int y = adjacent.Y - 1;

					if (isVerticalWall)
						y = random.Next(min, max);
					else
						x = random.Next(min, max);

					Level.Add(x, y, isDoorClosed ? Generation.Tile.DOOR_CLOSED : Generation.Tile.DOOR_OPENED);
				}
			}
		}

		/// <inheritdoc/>
		public override void Clear() => wallMap.ClearAllTiles();

		#endregion

		#region Door

		private void RemoveWall(Room room, Room adjacent)
		{
			if (adjacent.X >= room.X + room.Width)
			{
				int minY = Mathf.Max(room.Y, adjacent.Y);
				int maxY = Mathf.Min(room.Y + room.Height - 1, adjacent.Y + adjacent.Height - 1);

				for (int y = minY; y <= maxY; y++)
					Level.Remove(adjacent.X - 1, y, Generation.Tile.WALL);
			} else
			{
				int minX = Mathf.Max(room.X, adjacent.X);
				int maxX = Mathf.Min(room.X + room.Width - 1, adjacent.X + adjacent.Width - 1);

				for (int x = minX; x <= maxX; x++)
					Level.Remove(x, adjacent.Y - 1, Generation.Tile.WALL);
			}
		}

		private bool CanRoomsCombine(RoomType typeA, RoomType typeB)
		{
			RoomType[] validTypes = new RoomType[]
			{
				RoomType.NORMAL,
				RoomType.ENEMY,
				RoomType.TREASURE
			};

			return Array.IndexOf(validTypes, typeA) != -1 && Array.IndexOf(validTypes, typeB) != -1;
		}

		#endregion
	}
}