using System.Collections;
using Battle.Minigame;
using Battle.Options;
using Player;
using UnityEngine;
using UnityEngine.UI;
using UtilsModule;

namespace Battle
{
    /// <summary>
    /// Class that serves as an intermediate between the UI and the manager
    /// </summary>
    public class BattleUI : MonoBehaviour
    {
        #region UI Options

        [Header("UI Options")]
        [SerializeField]
        private BattleOptions battleOptions;

        [SerializeField]
        private EnemyOptions enemyOptions;

        private UIOptions GetOptions<T>() where T : UIOptionData
        {
            System.Type type = typeof(T);

            if (type == typeof(EnemyOptionData))
                return enemyOptions;

            if (type == typeof(BattleOptionData))
                return battleOptions;

            return null;
        }

        public void Load<T>(params T[] options) where T : UIOptionData => GetOptions<T>()?.LoadOptions(options);
        public void Move<T>(Vector2 dir) where T : UIOptionData => GetOptions<T>()?.Move(dir);
        public void Enter<T>() where T : UIOptionData => GetOptions<T>()?.Enter();
        public void Escape<T>() where T : UIOptionData => GetOptions<T>()?.Escape();
        public void ShowSelection<T>() where T : UIOptionData => GetOptions<T>()?.ShowSelection();
        public void HideSelection<T>() where T : UIOptionData => GetOptions<T>()?.HideSelection();
        public U GetSelection<T, U>() where T : UIOptionData where U : UIOption<T> => GetOptions<T>()?.GetSelection<T, U>();

        #endregion

        #region Spoiler

        [Header("Spoiler")]
        [SerializeField]
        private Image spoiler;

        public IEnumerator DisableSpoiler() => spoiler.FadeOut(4, 0.2f);
        public IEnumerator EnableSpoiler() => spoiler.FadeIn(7, 0.2f);

        #endregion

        #region Enemy Attack

        [Header("Enemy Attack")]
        [SerializeField]
        private CanvasGroup battleOptionsGroup;

        [SerializeField]
        private CanvasGroup battleEnemiesGroup;

        [SerializeField]
        private MinigameManager minigameManager;

        public IEnumerator StartEnemyTurn(PlayerInformation playerInformation)
        {
            Coroutine[] parallel = new Coroutine[]
            {
                StartCoroutine(battleOptionsGroup.Fade(4, 0.1f, 1f, 0f)),
                StartCoroutine(battleEnemiesGroup.Fade(4, 0.1f, 1f, 0.5f))
            };

            foreach (Coroutine item in parallel)
                yield return item;

            yield return playerInformation.OpenGroup(0.5f);
            yield return minigameManager.ShowIn();
        }

        public IEnumerator EndEnemyTurn(PlayerInformation playerInformation)
        {
            yield return minigameManager.HideOut();
            yield return playerInformation.CloseGroup(0.5f);

            Coroutine[] parallel = new Coroutine[]
            {
                StartCoroutine(battleOptionsGroup.Fade(4, 0.1f, 0f, 1f)),
                StartCoroutine(battleEnemiesGroup.Fade(4, 0.1f, 0.5f, 1f))
            };

            foreach (Coroutine item in parallel)
                yield return item;
        }

        #endregion
    }
}