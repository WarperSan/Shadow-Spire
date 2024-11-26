using System.Collections;
using GridModule;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GridEntity player;

    public GridEntity[] entities;

    private void Start()
    {
        StartCoroutine(ProcessTurn());
    }

    private IEnumerator ProcessTurn()
    {
        while (true)
        {
            yield return player.ExecuteTurn();

            foreach (var item in entities)
                yield return item.ExecuteTurn();
        }
    }
}
