using System.Collections;
using Enemies;
using UnityEngine;

namespace Battle.Spawners
{
	public abstract class Spawner : MonoBehaviour
	{
		public abstract Type HandledType { get; }
		public abstract void Setup(int strength);
		public abstract void Clean();

		public abstract IEnumerator StartSpawn(float duration);
	}
}