using System.Collections;
using System.Net;
using Entities.Interfaces;
using Managers;
using TMPro;
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
            SetWeapon(weapon); // Update UI
        }

        public GameObject icon;

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
            icon.SetActive(true);
        }

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            while (requestMove == null)
                yield return null;

            icon.SetActive(false);
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
        private WeaponSO weapon;

        [SerializeField]
        private Image weaponIcon;

        [SerializeField]
        private TextMeshProUGUI weaponType;

        public WeaponSO GetWeapon() => weapon;

        public void SetWeapon(WeaponSO weapon)
        {
            this.weapon = weapon;
            weaponIcon.sprite = weapon.Icon;
            weaponType.text = BattleEntity.BattleEntity.GetIcons(weapon.AttackType);
        }

        #endregion
    }
}