using System;
using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using Utils;

namespace Dungeon.Drawers.Rooms
{
    public abstract class RoomDrawer : Drawer
    {
        public abstract RoomType Type { get; }

        protected RoomDrawer(DungeonResult level) : base(level)
        {
        }

        protected abstract void OnDraw(Room room);
        protected abstract void OnProcess(Room room);

        #region Utils

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

                    positions.Add(new(x, y));
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