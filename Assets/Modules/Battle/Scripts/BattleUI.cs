using System.Text;
using Enemies;
using TMPro;
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

        #region Actions

        private const char SELECTED_ACTION_CHAR = '>';

        [Header("Actions")]
        [SerializeField]
        private TextMeshProUGUI actionsText;
        private int actionIndex = 0;
        private string[] ACTIONS;

        public void LoadActions(string[] actions)
        {
            ACTIONS = actions;
            UpdateActionText(0);
        }

        public void MoveActionCursor(Vector2 dir)
        {
            // If moving up
            if (dir.y > 0)
                actionIndex--;
            else if (dir.y < 0)
                actionIndex++;

            if (actionIndex < 0)
                actionIndex = ACTIONS.Length - 1;
            else if (actionIndex >= ACTIONS.Length)
                actionIndex = 0;

            UpdateActionText(actionIndex);
        }

        public void UpdateActionText(int index)
        {
            actionIndex = index;

            StringBuilder text = new();

            for (int i = 0; i < ACTIONS.Length; i++)
            {
                bool selected = i == actionIndex;

                text.Append(
                    string.Format(
                        "<color={0}>{1}</color> {2}",
                        selected ? "#FFFFFFFF" : "#FFFFFF00",
                        SELECTED_ACTION_CHAR,
                        ACTIONS[i]
                    )
                );

                if (i != ACTIONS.Length - 1)
                {
                    text.AppendLine();
                    text.AppendLine();
                }
            }

            actionsText.text = text.ToString();
        }

        public string GetAction() => ACTIONS[actionIndex];

        #endregion
    }
}
