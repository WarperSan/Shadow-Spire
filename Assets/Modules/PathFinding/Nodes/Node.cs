using System;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding.Nodes
{
    public abstract class Node : IComparable<Node>
    {
        public int Id { get; }

        public Node(int id)
        {
            Id = id;
            _links = new Dictionary<int, float>();
        }

        #region Links

        private readonly Dictionary<int, float> _links;

        /// <summary>Creates a link between this node and the given node</summary>
        public void AddLink(int id, float cost) => _links[id] = cost;

        /// <summary>Gets the neighbours of this node</summary>
        public int[] GetNeighbors() => _links.Keys.ToArray();

        /// <summary>Gets the cost of the link between this node and the given node</summary>
        public float GetCost(int id)
        {
            if (!_links.TryGetValue(id, out float cost))
                throw new NullReferenceException($"The node #{Id} is not linked to the node #{id}.");

            return cost;
        }

        #endregion

        #region IComparable

        public int CompareTo(Node other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return other is null ? 1 : Id.CompareTo(other.Id);
        }

        #endregion
    }
}