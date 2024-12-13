using Battle.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Weapons
{
    public class WeaponOptionData : UIOptionData
    {
        public WeaponInstance WeaponInstance;
    }

    public class WeaponOption : UIOption<WeaponOptionData>
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Image icon;

        [SerializeField]
        private TextMeshProUGUI type;

        [SerializeField]
        private TextMeshProUGUI damage;

        [SerializeField]
        private Image frame;

        #endregion

        #region UIOption

        /// <inheritdoc/>
        protected override void OnLoadOption(WeaponOptionData option)
        {
            var instance = option.WeaponInstance;
            var data = instance.GetBaseData();

            icon.sprite = data.Icon;
            type.text = BattleEntity.BattleEntity.GetIcons(data.AttackType);
            damage.text = instance.GetDamage().ToString();
        }

        /// <inheritdoc/>
        public override void Deselect()
        {
            frame.enabled = false;
        }

        /// <inheritdoc/>
        public override void Select()
        {
            frame.enabled = true;
        }

        #endregion
    }
}
