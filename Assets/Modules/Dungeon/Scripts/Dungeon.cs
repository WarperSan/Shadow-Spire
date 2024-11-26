using Dungeon.Drawers;
using Dungeon.Generation;
using UnityEngine;

namespace Dungeon
{
    public class Dungeon : MonoBehaviour
    {
        public int overSeed;
        public bool useSeed;

        #region Drawers

        [Header("Drawers")]
        [SerializeField]
        private WallDrawer wallDrawer;

        [SerializeField]
        private DoorDrawer doorDrawer;

        [SerializeField]
        private GroundDrawer groundDrawer;

        #endregion

        private void Start()
        {
            var seed = Random.Range(int.MinValue, int.MaxValue);

            if (useSeed)
                seed = overSeed;

            var random = new System.Random(seed);

            Debug.Log("Seed: " + seed);

            var result = new DungeonGenerator(random).Generate(12,12);

            foreach (var room in result.rooms)
                Debug.Log(room.X + ";" + room.Y + " (" + room.Width + "x" + room.Height + ")");

            wallDrawer.ProcessAndDraw(result.rooms);
            groundDrawer.ProcessAndDraw(result.rooms, out bool[,] groundGrid);

            bool[,] doorGrid = doorDrawer.Process(result.rooms, groundGrid, random);
            doorDrawer.Draw(doorGrid, result.rooms);
        }
    }
}