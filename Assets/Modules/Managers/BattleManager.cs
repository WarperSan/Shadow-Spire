using System.Collections;
using Battle;
using Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        private void Start()
        {
            InputManager.Instance.OnMoveUI.AddListener(Move);
        }

        public void StartBattle()
        {
            StartCoroutine(StartBattleTransition());

        }

        public void EndBattle()
        {

        }

        private BattleUI battleUI;

        #region Battle Transition

        [Header("Battle Transition")]
        [SerializeField] Material transitionMat;
        [SerializeField] Texture[] transitionTextures;

        public EnemySO enemyTemp1;
        public EnemySO enemyTemp2;
        public EnemySO enemyTemp3;

        private IEnumerator StartBattleTransition()
        {
            yield return LoadRandomTransition();
            yield return TransitionFadeIn(0.9f);

            var loadScene = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);

            while (!loadScene.isDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            battleUI = FindObjectOfType<BattleUI>();
            battleUI.ClearAllSlots();
            battleUI.UpdateActionText(0);

            yield return null; // Wait for everything to set up

            battleUI.SetSlot(enemyTemp1, 0);
            battleUI.SetSlot(enemyTemp2, 1);
            battleUI.SetSlot(enemyTemp3, 2);

            for (int i = 0; i < 3; i++)
                StartCoroutine(battleUI.GetSlot(i).SpawnAnimation());

            yield return null;

            battleUI.SPOILER.SetActive(false);

            yield return null; // Wait for spoiler to get removed
        }

        private IEnumerator EndBattleTransition()
        {
            battleUI = null;
            yield return null;
        }

        #endregion

        #region Transition Material

        private void SetTransitionTexture(Texture texture) => transitionMat.SetTexture("_TransitionTex", texture);
        private void SetTransitionCutoff(float cutoff) => transitionMat.SetFloat("_Cutoff", cutoff);

        private IEnumerator LoadRandomTransition()
        {
            int rdmIndex = Random.Range(0, transitionTextures.Length);
            SetTransitionTexture(transitionTextures[rdmIndex]);

            yield return null; // Wait for load texture
        }

        private IEnumerator TransitionFadeIn(float duration)
        {
            float time = 0;

            while (time < duration)
            {
                SetTransitionCutoff(Mathf.Lerp(0, 1, time / duration));

                yield return null; // Wait 1 frame

                time += Time.deltaTime;
            }

            SetTransitionCutoff(1f);
            yield return null; // Wait 1 frame
        }

        private void ResetTransitionMaterial()
        {
            SetTransitionTexture(null);
            SetTransitionCutoff(0);
        }

        #endregion

        #region Inputs

        private void Move(Vector2 dir)
        {
            if (battleUI == null)
                return;

            battleUI.MoveActionCursor(dir);
        }

        #endregion

#if UNITY_EDITOR
        // For keeping consistency in editor
        private void OnApplicationQuit() => ResetTransitionMaterial();
#endif
    }

}
