using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player
{
    public class PlayerInformation : MonoBehaviour
    {
        #region Weapon

        [Header("Weapon")]
        [SerializeField]
        private WeaponOption weaponOption;

        public void SetWeapon(WeaponInstance weapon) => weaponOption.LoadOption(new WeaponOptionData() { WeaponInstance = weapon });

        #endregion

        #region Health

        [Header("Health")]
        [SerializeField]
        private TextMeshProUGUI healthText;

        public void SetHealth(int health, int maxHealth) => healthText.text = string.Format(
            "<sprite name=icon_heart> {0} / {1}",
            health,
            maxHealth
        );

        #endregion

        #region Potion

        [Header("Potion")]
        [SerializeField]
        private TextMeshProUGUI potionCountText;

        public void SetPotionCount(int count) => potionCountText.text = string.Format("x{0}", count);

        #endregion
    
        #region Group

        [Header("Group")]
        [SerializeField] 
        private HorizontalLayoutGroup groupUI;

        public IEnumerator OpenGroup()
        {
            groupUI.spacing = 450;
            groupUI.padding.left = -40;
            yield return null;
        }

        public IEnumerator CloseGroup()
        {
            groupUI.spacing = 0;
            groupUI.padding.left = 0;
            yield return null;
        }

        #endregion
    }
}
