using System.Collections;
using Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.UI
{
    public class EnemySlot : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Image sprite;

        [SerializeField]
        private Image shadow;

        [SerializeField]
        private GameObject target;

        [SerializeField]
        private TextMeshProUGUI health;

        [SerializeField]
        private TextMeshProUGUI healthPopup;

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

        [Header("Animations")]
        [SerializeField]
        private Animator animator;

        public void SpawnAnimation() => animator.SetTrigger("spawnIn");
        public void HitAnimation(int damage)
        {
            animator.SetTrigger("hit");
            healthPopup.text = string.Format("-{0}", damage);
        }
        
        #endregion

        #region Target

        public void TargetSlot() => target.SetActive(true);
        public void UnTargetSlot() => target.SetActive(false);

        #endregion

        #region Health

        public void SetHealth(int health) => this.health.text = string.Format("<sprite name=icon_heart> {0}", health);

        #endregion
    }
}

