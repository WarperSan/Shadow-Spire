using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
	public enum EnemyPathing
	{
		Direct,
		Random
	}

	public enum EnemyMovementSpeed
	{
		VerySlow,
		Slow,
		Normal,
		Fast,
		VeryFast
	}
	
	[Flags]
	public enum Type
	{
		None = 0,
		Normal = 1 << 1,
		Undead = 1 << 2,
		Ghost = 1 << 3,
		Giant = 1 << 4,
		Animal = 1 << 5,
		Air = 1 << 6
	}

	/// <summary>
	/// Data of an enemy
	/// </summary>
	[CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy", order = 0)]
	public class EnemySo : ScriptableObject
	{
		[FormerlySerializedAs("OverworldSprite")]
		[Header("Overworld")]
		[Tooltip("Sprite used when displaying this enemy in the overworld")]
		public Sprite overworldSprite;

		[FormerlySerializedAs("MovementSpeed")]
		[Tooltip("Determines how fast this enemy moves in the overworld")]
		public EnemyMovementSpeed movementSpeed;

		[FormerlySerializedAs("Pathing")]
		[Tooltip("Determines the pathing used by this enemy")]
		public EnemyPathing pathing;

		[FormerlySerializedAs("FightSprite")]
		[Header("Fight")]
		[Tooltip("Sprite used when displaying this enemy in the fight")]
		public Sprite fightSprite;

		[FormerlySerializedAs("FightShadowSprite")]
		[Tooltip("Sprite used when display this enemy's shadow in the fight")]
		public Sprite fightShadowSprite;

		[FormerlySerializedAs("BaseHealth")]
		[Min(1)]
		public int baseHealth = 1;

		[FormerlySerializedAs("BaseType")]
		[Tooltip("Base type of this enemy")]
		public Type baseType;
	}
}