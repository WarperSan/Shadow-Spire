using Enemies;
using UnityEngine;

namespace Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        public GameObject SPOILER;

        #endregion

        #region Inputs

        public void Move(Vector2 dir)
        {
            var menus = new SelectMenu[] { battleOptions, enemyOptions };

            foreach (var item in menus)
            {
                if (item.IsSelected)
                {
                    item.Move(dir);
                    break;
                }
            }
        }

        public void Enter()
        {
            if (battleOptions.IsSelected)
            {
                var option = battleOptions.GetSelectedOption();

                if (option == "ATTACK")
                {
                    battleOptions.Clear();
                    enemyOptions.IsSelected = true;
                    Move(Vector2.zero);
                    return;
                }

                return;
            }
        }

        public void Escape()
        {
            if (enemyOptions.IsSelected)
            {
                enemyOptions.Clear();
                battleOptions.IsSelected = true;
                battleOptions.Move(Vector2.zero);
                return;
            }
        }

        #endregion

        #region Menus

        [Header("Menus")]
        [SerializeField]
        private BattleOptions battleOptions;

        [SerializeField]
        private EnemyOptions enemyOptions;

        public void SetBattleOptions(params string[] options)
        {
            if (battleOptions == null)
                return;

            battleOptions.SetOptions(options);
            battleOptions.IsSelected = true;
        }

        public void StartBattle(EnemySO left, EnemySO middle, EnemySO right)
        {
            if (enemyOptions == null)
                return;

            enemyOptions.SetSlot(left, 0);
            enemyOptions.SetSlot(middle, 1);
            enemyOptions.SetSlot(right, 2);

            for (int i = 0; i < 3; i++)
            {
                var slot = enemyOptions.GetSlot(i);

                if (slot == null)
                    return;

                StartCoroutine(slot.SpawnAnimation());
            }
        }

        #endregion
    }
}
