using System.Collections;
using System.Collections.Generic;
using Dungeon.Generation;
using Entities;
using UtilsModule;

public class TurnManager : Singleton<TurnManager>, IDungeonReceive
{
    public PlayerEntity player;

    private List<GridEntity> entities = new();

    private List<GridEntity>[,] entityEvents;

    private IEnumerator ProcessTurn()
    {
        while (true)
        {
            yield return player.ExecuteTurn();

            foreach (var item in entities)
                yield return item.ExecuteTurn();
        }
    }

    #region Enemies

    public void RegisterEntity(GridEntity entity) => entities.Add(entity);
    public void RemoveEntity(GridEntity entity) => entities.Remove(entity);

    #endregion

    #region Events

    public void RegisterEvent(GridEntity entity)
    {
        // Vector2Int pos = entity.Position;

        // entityEvents[pos.y, pos.x] ??= new();
        // entityEvents[pos.y, pos.x].Add(entity);
    }

    public void MoveEvent(GridEntity entity)
    {
        DungeonResult level = Dungeon.Dungeon.Instance.Level;

        for (int y = 0; y < level.Height; y++)
        {
            for (int x = 0; x < level.Width; x++)
            {
                if (entityEvents[y, x] == null)
                    continue;

                if (!entityEvents[y, x].Contains(entity))
                    continue;

                entityEvents[y, x].Remove(entity);
            }
        }

        RegisterEvent(entity);
    }

    #endregion

    #region IDungeonReceive

    /// <inheritdoc/>
    public void OnLevelStart(DungeonResult level)
    {
        entityEvents = new List<GridEntity>[level.Height, level.Width];
        entities.Clear();

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
