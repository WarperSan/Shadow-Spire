using Enemies;
using UnityEngine;

namespace Battle.UI
{
    public class EnemyOptions : SelectMenu
    {
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
        
        private int selectedTarget = -1;

        public override void Clear()
        {
            foreach (var item in slots)
                item?.UnTargetSlot();

            IsSelected = false;
        }

        public override void Move(Vector2 dir)
        {
            GetSlot(selectedTarget)?.UnTargetSlot();

            if (dir.x < 0)
                selectedTarget--;
            else if (dir.x > 0)
                selectedTarget++;

            if (selectedTarget < 0)
            {
                for (int i = slots.Length - 1; i >= 0 ; i--)
                {
                    if (GetSlot(i) != null)
                    {
                        selectedTarget = i;
                        break;
                    }
                }
            }
            else if (selectedTarget >= slots.Length)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (GetSlot(i) != null)
                    {
                        selectedTarget = i;
                        break;
                    }
                }
            }

            GetSlot(selectedTarget)?.TargetSlot();
        }
    }
}