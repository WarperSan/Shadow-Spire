using System.Collections;
using Dungeon.Generation;
using Entities.Interfaces;
using Managers;
using Player;
using UnityEngine;
using Weapons;

namespace Entities
{
    public class PlayerEntity : GridEntity, ITurnable, IMovable, IDungeonReceive
    {
        public PlayerInformation playerInformation;

        private void Start()
        {
            InputManager.Instance.OnMovePlayer.AddListener(Move);
        }

        #region Inputs

        private Movement? requestMove = null;

        private void Move(Vector2 dir)
        {
            Movement movement;

            if (dir.x > 0)
                movement = Movement.RIGHT;
            else if (dir.x < 0)
                movement = Movement.LEFT;
            else if (dir.y > 0)
                movement = Movement.UP;
            else
                movement = Movement.DOWN;

            // If can apply movement, register
            if ((this as IMovable).CanMove(movement))
                requestMove = movement;
        }

        #endregion

        #region ITurnable

        /// <inheritdoc/>
        public void OnTurnStarted()
        {
            requestMove = null; // Clear previous moves
        }

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            while (requestMove == null)
                yield return null;

            yield return requestMove.Value;
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
        public int Health { get; set; }
        private void SetHealth(int health)
        {
            Health = Mathf.Min(Mathf.Max(health, 0), MaxHealth);
            playerInformation.SetHealth((uint)Health, (uint)MaxHealth);
            GameManager.Instance.IsPlayerDead = Health <= 0;
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
        private WeaponInstance weapon;

        public WeaponInstance Weapon
        {
            get => weapon;
            set
            {
                playerInformation.SetWeapon(value);
                weapon = value;
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
        public bool HasPotions() => potionCount > 0;

        #endregion

        #region IDungeonReceive

        private bool initialized = false;

        /// <inheritdoc/>
        public void OnLevelStart(DungeonResult level)
        {
            if (!initialized)
            {
                MaxHealth = 150;

                // Update UI
                SetHealth(MaxHealth);
                Weapon = WeaponInstance.CreateRandom(1);
                SetPotionCount(0);
                initialized = true;
            }
        }

        /// <inheritdoc/>
        public void OnLevelEnd(DungeonResult level)
        {

        }

        #endregion
    }
}