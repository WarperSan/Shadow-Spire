using System.Collections;
using Battle.Options;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UtilsModule;

namespace TitleScreen
{
    /// <summary>
    /// Menu used for the title screen
    /// </summary>
    public class TitleScreenOptions : UIOptions<TitleScreenOption, TitleScreenOptionData>
    {
        public Graphic blackout;

        private void Start()
        {
            LoadOptions(new TitleScreenOptionData[] {
                new() {
                    Text = "Play",
                    OnEnter = OnPlay
                },
                new() {
                    Text = "Quit",
                    OnEnter = OnQuit
                }
            });

            ShowSelection();

            InputManager.Instance.OnMoveUI.AddListener(Move);
            InputManager.Instance.OnEnterUI.AddListener(Enter);
        }


        #region Play Sequence

        private bool isRunningPlaySequence;

        public void OnPlay()
        {
            if (isRunningPlaySequence)
                return;

            StartCoroutine(PlaySequence());
            isRunningPlaySequence = true;
        }

        private IEnumerator PlaySequence()
        {
            yield return blackout.FadeIn(4, 0.2f);
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        #endregion

        #region Quit Sequence

        private bool isRunningQuitSequence;

        public void OnQuit()
        {
            if (isRunningQuitSequence)
                return;

            StartCoroutine(QuitSequence());
            isRunningQuitSequence = true;
        }

        private IEnumerator QuitSequence()
        {
            yield return blackout.FadeIn(4, 0.2f);
            Application.Quit();
        }

        #endregion

        #region UIOptions

        /// <inheritdoc/>
        protected override void LoadOptions(TitleScreenOptionData[] options)
        {
            DestroyOptions();

            loadedOptions = new TitleScreenOption[options.Length];

            float singleY = -rectTransform.rect.size.y / options.Length;
            float startY = (options.Length - 1) / 2f * -singleY;

            for (int i = 0; i < options.Length; i++)
            {
                var newOption = Instantiate(optionPrefab.gameObject, transform);

                if (newOption.TryGetComponent(out RectTransform rectTransform))
                    rectTransform.anchoredPosition = new Vector2(0, startY + singleY * i);

                if (newOption.TryGetComponent(out TitleScreenOption titleOption))
                {
                    titleOption.SetParent(this);
                    titleOption.LoadOption(options[i]);
                    loadedOptions[i] = titleOption;
                }
            }

            selectedIndex = 0;
        }

        /// <inheritdoc/>
        protected override void OnMoveSelected(Vector2 dir) => base.OnMoveSelected(new(dir.y, dir.x));

        #endregion
    }
}