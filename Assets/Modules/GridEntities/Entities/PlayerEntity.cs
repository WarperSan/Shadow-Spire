using System.Collections;
using Dungeon.Generation;
using GridEntities.Abstract;
using GridEntities.Interfaces;
using Managers;
using Player;
using UnityEngine;
using Weapons;

namespace GridEntities.Entities
{
	public class PlayerEntity : GridEntity, ITurnable, IMovable, IDungeonReceive
	{
		public PlayerInformation playerInformation;

		private void Start()
		{
			InputManager.Instance.onMovePlayer.AddListener(Move);
		}

		#region Inputs

		private Movement? _requestMove = null;

		private void Move(Vector2 dir)
		{
			Movement movement;

			if (dir.x > 0)
				movement = Movement.Right;
			else if (dir.x < 0)
				movement = Movement.Left;
			else if (dir.y > 0)
				movement = Movement.Up;
			else
				movement = Movement.Down;

			// If can apply movement, register
			if ((this as IMovable).CanMove(movement))
				_requestMove = movement;
		}

		#endregion

		#region ITurnable

		/// <inheritdoc/>
		public void OnTurnStarted()
		{
			_requestMove = null; // Clear previous moves
		}

		/// <inheritdoc/>
		IEnumerator ITurnable.Think()
		{
			while (_requestMove == null)
				yield return null;

			yield return _requestMove.Value;
		}

		#endregion

		#region IMovable

		/// <inheritdoc/>
		void IMovable.OnMoveStart(Movement movement)
		{
			FlipByMovement(movement);
		}

		#endregion

		#region Health

		public int MaxHealth { get; set; }
		public int Health    { get; set; }

		private void SetHealth(int health)
		{
			Health = Mathf.Min(Mathf.Max(health, 0), MaxHealth);
			playerInformation.SetHealth((uint)Health, (uint)MaxHealth);
			GameManager.Instance.isPlayerDead = Health <= 0;
		}

		public void TakeDamage(int damage)
		{
			SetHealth(Health - damage);
			playerInformation.HitHealth((uint)damage);
		}

		public void Heal(int amount) => SetHealth(Health + amount);

		#endregion

		#region Weapon

		[Header("Weapon")]
		[SerializeField]
		private WeaponInstance _weapon;

		public WeaponInstance Weapon
		{
			get => _weapon;
			set
			{
				playerInformation.SetWeapon(value);
				_weapon = value;
			}
		}

		#endregion

		#region Potions

		[Header("Potions")]
		[SerializeField]
		private int potionCount = 0;

		private void SetPotionCount(int amount)
		{
			potionCount = amount;
			playerInformation.SetPotionCount(potionCount);
		}

		public void CollectPotion() => SetPotionCount(potionCount + 1);
		public void ConsumePotion() => SetPotionCount(potionCount - 1);
		public bool HasPotions()    => potionCount > 0;

		#endregion

		#region IDungeonReceive

		private bool _initialized = false;

		/// <inheritdoc/>
		public void OnLevelStart(DungeonResult level)
		{
			if (!_initialized)
			{
				MaxHealth = 150;

				// Update UI
				SetHealth(MaxHealth);
				Weapon = WeaponInstance.CreateRandom(1);
				SetPotionCount(0);
				_initialized = true;
			}
		}

		/// <inheritdoc/>
		public void OnLevelEnd(DungeonResult level)
		{
		}

		#endregion
	}
}