using System.Collections;
using BattleEntity;
using UnityEngine;

namespace Battle.Minigame.Spawners
{
    public abstract class Spawner : MonoBehaviour
    {
        public abstract Type HandledType { get; }
        public abstract void Setup(int strength);
        public abstract void Clean();

        public abstract IEnumerator StartSpawn(float duration);
        public abstract void StopSpawn();
    }
}