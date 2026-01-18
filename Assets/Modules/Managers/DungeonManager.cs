using System.Linq;
using Dungeon.Drawers;
using Dungeon.Drawers.Rooms;
using Dungeon.Drawers.Terrain;
using Dungeon.Generation;
using GridEntities.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
	public class DungeonManager : MonoBehaviour
	{
		#region Ground

		[Header("Ground")]
		[SerializeField]
		private Tilemap groundMap;

		[SerializeField]
		private TileBase groundTile;

		#endregion

		#region Walls

		[Header("Walls")]
		[SerializeField]
		private Tilemap wallMap;

		// TOP    = 0b0001 = 1
		// RIGHT  = 0b0010 = 2
		// BOTTOM = 0b0100 = 4
		// LEFT   = 0b1000 = 8
		[SerializeField]
		private TileBase[] wallTiles;

		#endregion

		#region Doors

		[Header("Doors")]
		[SerializeField]
		private TileBase openedDoorTile;

		[SerializeField]
		private TileBase closedDoorTile;

		#endregion

		#region Entities

		[Header("Entities")]
		[SerializeField]
		private EntranceEntity entrance;

		[SerializeField]
		private ExitEntity exit;

		#endregion

		#region Spawn Items

		[Header("Spawn Items")]
		[SerializeField]
		private Transform spawnItemsParent;

		#endregion

		#region Enemies

		[Header("Enemies")]
		[SerializeField]
		private GameObject enemyPrefab;

		[SerializeField]
		private GameObject spikesPrefab;

		#endregion

		#region Treasures

		[Header("Treasures")]
		[SerializeField]
		private GameObject potionPrefab;

		#endregion

		#region Generation

		private Drawer[] DrawerPipeline;
		private IDungeonReceive[] Receivers;

		public DungeonResult GenerateLevel(DungeonSettings settings, PlayerEntity player)
		{
			// Generate level
			System.Random random = new(settings.Seed);
			DungeonResult lvl = DungeonGenerator.Generate(random, settings);

			// Create drawers
			DrawerPipeline = new Drawer[]
			{
				// Terrain
				new WallDrawer(lvl, wallMap, wallTiles),
				new DoorDrawer(lvl,
					wallMap,
					openedDoorTile,
					closedDoorTile),
				new EntranceExitDrawer(lvl,
					entrance,
					exit,
					player),
				new GroundDrawer(lvl, groundMap, groundTile),

				// Rooms
				new EnemyRoomDrawer(lvl,
					enemyPrefab,
					Enemies.EnemyInstance.ENEMIES,
					spawnItemsParent),
				new TreasureRoomDrawer(lvl, potionPrefab, spawnItemsParent),
				new SpikesRoomDrawer(lvl, spikesPrefab, spawnItemsParent)
			};

			// Process the level
			lvl.Player = player;
			lvl.Random = random;
			lvl.Grid = Drawer.CreateEmpty(lvl.Rooms);
			lvl.Height = lvl.Grid.GetLength(0);
			lvl.Width = lvl.Grid.GetLength(1);

			foreach (Drawer drawer in DrawerPipeline)
				drawer.Process(lvl.Rooms);

			// Compute graphs
			lvl.TileGraph = PathFindingManager.ComputeTileGraph(lvl);

			return lvl;
		}

		public void StartLevel(DungeonResult lvl)
		{
			// Draw the level
			foreach (Drawer drawer in DrawerPipeline)
				drawer.Draw(lvl.Rooms);

			// Notify all receivers
			Receivers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDungeonReceive>().ToArray();

			foreach (IDungeonReceive receiver in Receivers)
				receiver.OnLevelStart(lvl);
		}

		public void ClearDungeon()
		{
			// Clear all drawers
			foreach (Drawer drawer in DrawerPipeline)
				drawer.Clear();
		}

		#endregion
	}
}