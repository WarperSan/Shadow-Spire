using Battle.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilsModule;

namespace Weapons.UI
{
    /// <summary>
    /// Data used on the weapon selection menu
    /// </summary>
    public class WeaponOptionData : UIOptionData
    {
        public WeaponInstance WeaponInstance;
        public string Subtext;
    }

    /// <summary>
    /// Option used on the weapon selection menu
    /// </summary>
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

        [SerializeField]
        private TextMeshProUGUI subtext;

        #endregion

        #region UIOption

        /// <inheritdoc/>
        protected override void OnLoadOption(WeaponOptionData option)
        {
            WeaponInstance instance = option.WeaponInstance;

            icon.sprite = instance.GetIcon();
            type.text = instance.GetTypes().GetIcons();
            damage.text = instance.GetDamage().ToString();
            subtext.text = option.Subtext;
        }

        /// <inheritdoc/>
        public override void Deselect()
        {
            frame.gameObject.SetActive(false);
            subtext.gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public override void Select()
        {
            frame.gameObject.SetActive(true);
            subtext.gameObject.SetActive(true);
        }

        #endregion
    }
}
