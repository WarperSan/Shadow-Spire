using System.Collections;
using Entities.Interfaces;
using Managers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Entities
{
    public class PlayerEntity : GridEntity, ITurnable, IMovable
    {
        private void Start()
        {
            InputManager.Instance.OnMovePlayer.AddListener(Move);
            SetWeapon(new WeaponInstance(startWeapon, 0)); // Update UI
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
    
        #region Weapon

        [Header("Weapon")]
        [SerializeField]
        private WeaponInstance weapon;

        [SerializeField]
        private WeaponSO startWeapon;

        [SerializeField]
        private WeaponOption weaponOption;

        public WeaponInstance GetWeapon() => weapon;

        public void SetWeapon(WeaponInstance weapon)
        {
            this.weapon = weapon;
            weaponOption.LoadOption(new WeaponOptionData() { WeaponInstance = weapon });
        }

        #endregion
    }
}