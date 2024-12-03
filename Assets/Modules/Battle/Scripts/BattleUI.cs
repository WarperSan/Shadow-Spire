using Enemies;
using UnityEngine;

namespace Battle
{
    public class BattleUI : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        public GameObject SPOILER;

        #endregion

        #region Slots

        [Header("Slots")]

        [SerializeField]
        private EnemySlot[] slots;

        public EnemySlot GetSlot(int index)
        {
            if (index < 0 || slots.Length <= index)
                return null;

            return slots[index];
        }

        public void SetSlot(EnemySO data, int index) => GetSlot(index)?.SetEnemy(data);

        private void ClearSlots(int index) => SetSlot(null, index);

        public void ClearAllSlots()
        {
            for (int i = 0; i < slots.Length; i++)
                ClearSlots(i);
        }

        #endregion
    }
}
