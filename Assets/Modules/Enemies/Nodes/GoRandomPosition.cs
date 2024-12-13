using BehaviourModule.Nodes;
using Entities;
using Managers;
using UnityEngine;

namespace Enemies.Node
{
    internal class GoRandomPosition : BehaviourModule.Nodes.Node
    {
        private GridEntity self;
        private int[] path;
        private Movement[] movements;
        private Vector2Int rdmPosition;

        public GoRandomPosition(GridEntity self)
        {
            this.self = self;
            FindRandomPosition();
        }

        protected override NodeState OnEvaluate()
        {
            if (self.Position == rdmPosition)
                FindRandomPosition();

            path = PathFindingManager.FindPath(self, rdmPosition);
            movements = PathFindingManager.GetDirections(path);

            // If no path found or on the same tile
            if (movements == null || movements.Length == 0)
            {
                SetData("NextMovement", null, -1);
                return NodeState.FAILURE;
            }

            SetData("NextMovement", movements[0], -1);

            return NodeState.SUCCESS;
        }

        private void FindRandomPosition()
        {
            var level = GameManager.Instance.Level;
            var rdmRoom = level.Rooms[level.Random.Next(0, level.Rooms.Length)];

            var rdmX = level.Random.Next(rdmRoom.X, rdmRoom.X + rdmRoom.Width - 1);
            var rdmY = level.Random.Next(rdmRoom.Y, rdmRoom.Y + rdmRoom.Height - 1);

            rdmPosition = new Vector2Int(rdmX, rdmY);
        }
    }
}
