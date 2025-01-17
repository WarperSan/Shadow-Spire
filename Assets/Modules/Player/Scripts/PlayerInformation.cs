using System.Collections;
using TMPro;
using UI.Abstract;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using Weapons.UI;

namespace Player
{
    public class PlayerInformation : MonoBehaviour
    {
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        #region Weapon

        [Header("Weapon")]
        [SerializeField]
        private WeaponOption weaponOption;

        public void SetWeapon(WeaponInstance weapon) => weaponOption.LoadOption(new WeaponOptionData() { WeaponInstance = weapon });

        #endregion

        #region Health

        [Header("Health")]
        [SerializeField]
        private HealthBar playerHealthBar;

        public void SetHealth(uint health, uint maxHealth)
        {
            playerHealthBar.SetHealth(health, maxHealth);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        public void HitHealth(uint amount)
        {
            playerHealthBar.TakeDamage(amount);
        }

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

        public IEnumerator OpenGroup(float time)
        {
            const int TICKS = 16;
            const float SPACING = 225;

            for (int i = 1; i <= TICKS; i++)
            {
                groupUI.spacing = SPACING / TICKS * i;
                yield return new WaitForSeconds(time / TICKS);
            }

            groupUI.spacing = SPACING;
        }

        public IEnumerator CloseGroup(float time)
        {
            const int TICKS = 16;
            const float SPACING = 0;

            float startSpacing = groupUI.spacing;

            for (int i = 1; i <= TICKS; i++)
            {
                groupUI.spacing = startSpacing - startSpacing / TICKS * i;
                yield return new WaitForSeconds(time / TICKS);
            }

            groupUI.spacing = SPACING;
        }

        #endregion
    }
}
