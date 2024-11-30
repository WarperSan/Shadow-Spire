using System.Collections;
using System.Collections.Generic;
using Dungeon.Generation;
using Entities;
using Entities.Interfaces;
using UtilsModule;

public class TurnManager : Singleton<TurnManager>, IDungeonReceive
{
    public PlayerEntity player;

    private readonly List<GridEntity> turnEntities = new();
    private readonly List<GridEntity> eventEntities = new();

    private IEnumerator ProcessTurn()
    {
        while (!Managers.GameManager.Instance.IsLevelOver)
        {
            foreach (var entity in turnEntities)
            {
                yield return entity.ExecuteTurn();

                // Level over
                if (Managers.GameManager.Instance.IsLevelOver)
                    break;

                // Check for event
                foreach (var item in eventEntities)
                {
                    if (item == entity)
                        continue;

                    if (item.Position != entity.Position)
                        continue;

                    (item as IEventable).OnEntityLand(entity);
                }
            }
        }
    }

    #region IDungeonReceive

    /// <inheritdoc/>
    public void OnLevelStart(DungeonResult level)
    {
        turnEntities.Clear();
        eventEntities.Clear();

        turnEntities.Add(player); // Make the player the first entity

        var foundEntities = FindObjectsOfType<GridEntity>();

        foreach (var item in foundEntities)
        {
            // Don't add player twice
            if (item == player)
                continue;

            if (item is IEventable)
                eventEntities.Add(item);

            if (item is not ITurnable)
                continue;

            turnEntities.Add(item);
        }

        StartCoroutine(ProcessTurn());
    }

    /// <inheritdoc/>
    public void OnLevelEnd(DungeonResult level) { }

    #endregion

    #region Singleton

    /// <inheritdoc/>
    protected override bool DestroyOnLoad => true;

    #endregion
}
