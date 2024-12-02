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

        #endregion

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
        }

        private void SetSlot(EnemySO data)
        {
            sprite.sprite = data.FightSprite;
            sprite.enabled = true;
        }
    }
}

