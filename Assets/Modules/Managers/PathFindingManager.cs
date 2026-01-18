using System;
using Dungeon.Generation;
using GridEntities.Abstract;
using GridEntities.Interfaces;
using PathFinding.Graphs;
using UnityEngine;
using Utils;

namespace Managers
{
	public class PathFindingManager : MonoBehaviour
	{
		public const int NO_NODE_ID = -1;

		#region Compute

		public static TileGraph ComputeTileGraph(DungeonResult level)
		{
			int[,] ids = new int[level.Height, level.Width];

			TileGraph graph = new(ids);

			// Generate nodes
			for (int y = 0; y < level.Height; y++)
			{
				for (int x = 0; x < level.Width; x++)
				{
					ids[y, x] = level.HasGround(x, y)
						? graph.AddNode(new Vector2(x, -y))
						: NO_NODE_ID;
				}
			}

			// Generale links between room nodes
			foreach (Room room in level.Rooms)
			{
				int maxX = room.X + room.Width + 1;
				int maxY = room.Y + room.Height + 1;

				for (int y = room.Y; y < maxY; y++)
				{
					for (int x = room.X; x < maxX; x++)
					{
						int current = graph.GetID(x, y);

						if (current == NO_NODE_ID)
							continue;

						int[] nexts = new int[]
						{
							graph.GetID(x + 1, y), // To right
							graph.GetID(x, y - 1)  // To bottom
						};

						foreach (int next in nexts)
						{
							if (next == NO_NODE_ID)
								continue;

							graph.AddLink(current,
								next,
								1f,
								true);
						}
					}
				}
			}

			// Generate links between doors
			for (int y = 0; y < level.Height; y++)
			{
				for (int x = 0; x < level.Width; x++)
				{
					// If not a door, skip
					if (!level.HasDoor(x, y))
						continue;

					int current;
					int next;

					// If on vertical wall
					if (level.HasWall(x, y + 1))
					{
						current = graph.GetID(x - 1, y);
						next = graph.GetID(x + 1, y);
					} else
					{
						current = graph.GetID(x, y - 1);
						next = graph.GetID(x, y + 1);
					}

					if (current == NO_NODE_ID || next == NO_NODE_ID)
						continue;

					graph.AddLink(current,
						next,
						3f,
						true);
				}
			}

			return graph;
		}

		#endregion

		#region Utils

		private static TileGraph GetTileGraph()
		{
			// Error checking
			if (GameManager.Instance == null)
				throw new NullReferenceException($"No '{nameof(GameManager)}' instance was instantiate.");

			if (GameManager.Instance.Level == null)
				throw new NullReferenceException($"No level was loaded.");

			return GameManager.Instance.Level.TileGraph ??
			       throw new NullReferenceException($"The level did not compute it's '{nameof(TileGraph)}' yet.");
		}

		public static Movement[] GetDirections(int[] path)
		{
			if (path == null)
				return null;

			TileGraph graph = GetTileGraph();

			Movement[] movements = new Movement[path.Length - 1]; // Exclude first one

			for (int i = 1; i < path.Length; i++)
			{
				Vector2 current = graph.GetNode(path[i - 1]).Position;
				Vector2 next = graph.GetNode(path[i]).Position;

				if (next.x < current.x)
					movements[i - 1] = Movement.LEFT;
				else if (next.y < current.y)
					movements[i - 1] = Movement.DOWN;
				else if (next.x > current.x)
					movements[i - 1] = Movement.RIGHT;
				else
					movements[i - 1] = Movement.UP;
			}

			return movements;
		}

		public static int[] FindPath(GridEntity origin, GridEntity target) => FindPath(origin, target.Position);

		public static int[] FindPath(GridEntity origin, Vector2Int target)
		{
			TileGraph graph = GetTileGraph();

			// Get start ID
			Vector2Int originPos = origin.Position;
			int start = graph.GetID(originPos.x, -originPos.y);

			if (start == NO_NODE_ID)
				return null;

			// Get end ID
			int end = graph.GetID(target.x, -target.y);

			if (end == NO_NODE_ID)
				return null;

			// Compute path
			return graph.GetPath(start, end);
		}

		#endregion

		#region Gizmos

		#if UNITY_EDITOR

		[Header("Gizmos")]
		[SerializeField]
		[Tooltip("Gradient used for the links")]
		private Gradient LinkCostColor;

		/// <inheritdoc/>
		private void OnDrawGizmos()
		{
			// If manager not instantiate, skip
			if (GameManager.Instance == null)
				return;

			DungeonResult level = GameManager.Instance.Level;

			// If level not loaded, skip
			if (level == null)
				return;

			TileGraph graph = level.TileGraph;

			// If graph not compiled, skip
			if (graph == null)
				return;

			for (int y = 0; y < level.Height; y++)
			{
				for (int x = 0; x < level.Width; x++)
				{
					int id = graph.GetID(x, y);

					if (id == NO_NODE_ID)
						continue;

					PathFinding.Nodes.Node2D n = graph.GetNode(id);

					// Draw links
					foreach (int neighbor in n.GetNeighbors())
					{
						Gizmos.color = LinkCostColor.Evaluate(n.GetCost(neighbor) / 3f);
						Gizmos.DrawLine(n.Position + new Vector2(0.5f, -0.5f), graph.GetNode(neighbor).Position + new Vector2(0.5f, -0.5f));
					}
				}
			}
		}

		#endif

		#endregion
	}
}