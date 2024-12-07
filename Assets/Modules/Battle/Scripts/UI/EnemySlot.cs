using System.Collections;
using Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.UI
{
    public class EnemySlot : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private RectTransform enemy;

        [SerializeField]
        private Image sprite;

        [SerializeField]
        private Image shadow;

        [SerializeField]
        private GameObject target;

        #endregion

        #region Data

        public void SetEnemy(EnemySO data)
        {
            if (data == null)
                ClearSlot();
            else
                SetSlot(data);
        }

        private void ClearSlot()
        {
            sprite.sprite = null;
            sprite.enabled = false;
            shadow.sprite = null;
            shadow.enabled = false;
            UnTargetSlot();
        }

        private void SetSlot(EnemySO data)
        {
            sprite.sprite = data.FightSprite;
            sprite.enabled = true;
            shadow.sprite = data.FightShadowSprite;
            shadow.enabled = data.FightShadowSprite != null;
            UnTargetSlot();
        }

        #endregion

        #region Animations

        public IEnumerator SpawnAnimation()
        {
            const float DURATION = 1.5f;
            const float TICKS = 8;
            const float START_Y = 80f;
            const float ALPHA_SPRITE = 1f;
            const float ALPHA_SHADOW = 26f / 255f;

            var pos = enemy.anchoredPosition;
            var spriteColor = Color.black;
            var shadowColor = Color.black;
            pos.y = START_Y;

            yield return null; // Wait 1 frame

            for (int i = 0; i <= TICKS; i++)
            {
                pos.y = START_Y - START_Y / TICKS * i;

                spriteColor.r = spriteColor.g = spriteColor.b = ALPHA_SPRITE / TICKS * i;
                shadowColor.r = shadowColor.g = shadowColor.b = ALPHA_SHADOW / TICKS * i;

                enemy.anchoredPosition = pos;
                sprite.color = spriteColor;
                shadow.color = shadowColor;
                yield return new WaitForSeconds(DURATION / TICKS);
            }
        }

        #endregion

        #region Target

        public void TargetSlot() => target.SetActive(true);
        public void UnTargetSlot() => target.SetActive(false);

        #endregion
    }
}

