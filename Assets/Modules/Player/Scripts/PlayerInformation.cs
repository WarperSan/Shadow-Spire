using System.Collections;
using TMPro;
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
        private TextMeshProUGUI healthText;

        [SerializeField]
        private TextMeshProUGUI healthPopupText;

        [SerializeField]
        private Animator healthAnimator;

        public void SetHealth(int health, int maxHealth)
        {
            healthText.text = string.Format(
                "<sprite name=icon_heart> {0} / {1}",
                health,
                maxHealth
            );

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        public void HitHealth(int amount)
        {
            healthPopupText.text = string.Format("-{0}", amount);
            healthAnimator.SetTrigger("hit");
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
