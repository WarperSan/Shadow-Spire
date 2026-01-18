using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Spawners;
using BattleEntity;
using Player;
using UnityEngine;
using Utils;

namespace Managers
{
	public class MinigameManager : MonoBehaviour
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private GameObject projectile;

		[SerializeField]
		private MinigamePlayer player;

		[SerializeField]
		private Spawner[] spawners;

		[SerializeField]
		private Transform projectilesParent;

		[SerializeField]
		private Animator animator;

		#endregion

		#region Data

		private Dictionary<BattleEntity.Type, int> typesCount = new();
		private float duration;

		#endregion

		#region Manager

		public void SetupProjectiles(BattleEnemyEntity[] battleEnemyEntities, BattlePlayerEntity playerEntity, BattleManager battleManager)
		{
			player.battleManager = battleManager;
			player.ResetSelf();

			if (battleEnemyEntities == null || battleEnemyEntities.Length == 0)
				return;

			int enemyCount = 0;

			// Compile types
			foreach (BattleEntity.Type item in Enum.GetValues(typeof(BattleEntity.Type)))
				typesCount[item] = 0;

			foreach (BattleEnemyEntity enemy in battleEnemyEntities)
			{
				// Ignore dead enemies
				if (enemy.IsDead)
					continue;

				foreach (BattleEntity.Type uniqueType in enemy.Type.GetTypes())
					typesCount[uniqueType]++;

				enemyCount++;
			}

			// Set up spawners
			foreach (Spawner spawner in spawners)
			{
				int strength = typesCount[spawner.HandledType];

				spawner.Setup(strength);
				spawner.enabled = strength > 0;
			}

			duration = 3.5f * enemyCount;
		}

		public IEnumerator SpawnProjectiles()
		{
			InputManager.Instance.SwitchToMiniGame();
			InputManager.Instance.OnMoveMinigame.AddListener(Move);
			player.canTakeDamage = true;

			List<Coroutine> spawnerCoroutines = new();

			foreach (Spawner spawner in spawners)
			{
				if (!spawner.enabled)
					continue;

				spawnerCoroutines.Add(StartCoroutine(spawner.StartSpawn(duration)));
			}

			yield return new WaitForSeconds(duration);

			foreach (Coroutine item in spawnerCoroutines)
				yield return item;

			player.canTakeDamage = false;
			InputManager.Instance.OnMoveMinigame.RemoveListener(Move);
			InputManager.Instance.SwitchToUI();
		}

		public void CleanProjectiles()
		{
			// Destroy all projectiles
			foreach (Spawner spawner in spawners)
			{
				if (!spawner.enabled)
					continue;

				spawner.Clean();
				spawner.enabled = false;
			}

			// Remove all the compiled data
			duration = -1;
		}

		#endregion

		#region Animations

		public IEnumerator Appear()
		{
			yield return Appear_Spin();
		}

		private IEnumerator Appear_Spin()
		{
			Coroutine[] parallel = new Coroutine[]
			{
				StartCoroutine(AnimationsUtils.Animate(
					values =>
					{
						transform.localScale = new Vector3(values[0], values[1], values[2]);
					},
					AnimationsUtils.EaseInOut(0,
						0,
						30 * 0.017f,
						1.1f), // x
					AnimationsUtils.EaseInOut(0,
						0,
						30 * 0.017f,
						1.1f), // y
					AnimationsUtils.EaseInOut(0,
						0,
						30 * 0.017f,
						1.1f) // z
				)),
				StartCoroutine(AnimationsUtils.Animate(
					values =>
					{
						transform.localRotation = Quaternion.Euler(values[0], values[1], values[2]);
					},
					AnimationsUtils.EaseInOut(0,
						0,
						30 * 0.017f,
						0), // x
					AnimationsUtils.EaseInOut(0,
						0,
						30 * 0.017f,
						0), // y
					AnimationsUtils.EaseInOut(0,
						0,
						30 * 0.017f,
						720) // z
				))
			};

			foreach (Coroutine item in parallel)
				yield return item;

			yield return AnimationsUtils.Animate(
				values =>
				{
					transform.localScale = new Vector3(values[0], values[1], values[2]);
				},
				AnimationsUtils.EaseInOut(0,
					1.1f,
					15 * 0.017f,
					1f), // x
				AnimationsUtils.EaseInOut(0,
					1.1f,
					15 * 0.017f,
					1f), // y
				AnimationsUtils.EaseInOut(0,
					1.1f,
					15 * 0.017f,
					1f) // z
			);
			//yield return transform.ScaleLocal(30, 0.017f, Vector3.zero, Vector3.one);
		}

		public IEnumerator Disappear()
		{
			yield return Disappear_ScaleDown();
		}

		private IEnumerator Disappear_ScaleDown()
		{
			yield return transform.ScaleLocal(15,
				0.017f,
				Vector3.one,
				Vector3.zero);
		}

		#endregion

		#region Inputs

		public void Move(Vector2 dir) => player.Move(dir);

		#endregion
	}
}