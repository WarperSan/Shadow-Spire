using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilsModule;
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
        private GameObject healthPopupPrefab;

        [SerializeField]
        private Transform healthPopupContainer;

        private Coroutine healthBlinkCoroutine;

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
            if (healthBlinkCoroutine != null)
                StopCoroutine(healthBlinkCoroutine);

            healthBlinkCoroutine = StartCoroutine(HealthBlink());
            StartCoroutine(HealthPopup(amount));
        }

        private IEnumerator HealthBlink(int count = 3)
        {
            healthText.enabled = true;

            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(5f / 60f); // Wait 5 frames

                healthText.enabled = false;

                yield return new WaitForSeconds(5f / 60f); // Wait 5 frames

                healthText.enabled = true;
            }

            // Clear coroutine
            healthBlinkCoroutine = null;
        }

        private IEnumerator HealthPopup_Position(RectTransform rect)
        {
            yield return rect.TranslateLocal(32, 40f / 60f / 32, new Vector3(0, 0, 0), new Vector3(0, 35, 0));
        }

        private IEnumerator HealthPopup_Alpha(TextMeshProUGUI text)
        {
            yield return new WaitForSeconds(30f / 60f); // Wait 30 frames
            yield return text.FadeOut(32, 10f /60f / 32);
        }

        private IEnumerator HealthPopup(int amount)
        {
            var popup = Instantiate(healthPopupPrefab, healthPopupContainer);

            yield return null; // Wait for load

            var text = popup.GetComponent<TextMeshProUGUI>();
            text.text = string.Format("-{0}", amount);

            var rect = popup.GetComponent<RectTransform>();

            Coroutine[] parallel = new Coroutine[]
            {
                StartCoroutine(HealthPopup_Position(rect)),
                StartCoroutine(HealthPopup_Alpha(text)),
            };

            foreach (Coroutine item in parallel)
                yield return item;

            yield return null;
            Destroy(popup);
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
