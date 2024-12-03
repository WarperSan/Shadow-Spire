using System.Collections;
using Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class EnemySlot : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Image sprite;

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
            UnTargetSlot();
        }

        private void SetSlot(EnemySO data)
        {
            sprite.sprite = data.FightSprite;
            sprite.enabled = true;
            UnTargetSlot();
        }

        #endregion

        #region Animations

        public IEnumerator SpawnAnimation()
        {
            const float DURATION = 1.5f;
            const float TICKS = 8;
            const float START_Y = 80f;

            var pos = sprite.rectTransform.anchoredPosition;
            var color = Color.black;
            pos.y = START_Y;

            yield return null; // Wait 1 frame

            for (int i = 0; i <= TICKS; i++)
            {
                pos.y = START_Y - START_Y / TICKS * i;
                color.r = color.g = color.b = 1f / TICKS * i;

                sprite.rectTransform.anchoredPosition = pos;
                sprite.color = color;
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

