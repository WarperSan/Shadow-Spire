using UnityEngine;

namespace PathFinding.Nodes
{
    public class Node2D : Node
    {
        public Vector2Int Position { get; }

        public Node2D(int id, Vector2Int position) : base(id)
        {
            this.Position = position;
        }
    }
}