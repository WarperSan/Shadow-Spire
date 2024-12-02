using System.Collections;
using System.Collections.Generic;
using Dungeon.Generation;
using Entities;
using Entities.Interfaces;
using UnityEngine;

namespace Managers
{
    public class TurnManager : MonoBehaviour, IDungeonReceive
    {
        private readonly List<GridEntity> turnEntities = new();
        private GridEntity[] foundEntities;

        private IEnumerator ProcessTurn()
        {
            while (!GameManager.Instance.IsLevelOver)
            {
                foreach (var entity in turnEntities)
                {
                    // Level over
                    if (GameManager.Instance.IsLevelOver)
                        break;

                    yield return entity.ExecuteTurn();

                    // Check for event
                    foreach (var item in foundEntities)
                    {
                        // If checking self, skip
                        if (item == entity)
                            continue;

                        // If not on the same position, skip
                        if (item.Position != entity.Position)
                            continue;

                        if (entity is IEventable landOn)
                            landOn.OnEntityLand(item);

                        if (item is IEventable landedOn)
                            landedOn.OnEntityLanded(entity);
                    }
                }
            }
        }

        #region IDungeonReceive

        /// <inheritdoc/>
        public void OnLevelStart(DungeonResult level)
        {
            var player = level.Player;

            turnEntities.Clear();
            turnEntities.Add(player); // Make the player the first entity

            foundEntities = FindObjectsOfType<GridEntity>();

            foreach (var item in foundEntities)
            {
                // Don't add player twice
                if (item == player)
                    continue;

                if (item is not ITurnable)
                    continue;

                turnEntities.Add(item);
            }

            StartCoroutine(ProcessTurn());
        }

        /// <inheritdoc/>
        public void OnLevelEnd(DungeonResult level) { }

        #endregion
    }
}
