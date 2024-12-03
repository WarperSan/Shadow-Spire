using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        #region Battle Transition

        [Header("Battle Transition")]
        [SerializeField] Material transitionMat;
        [SerializeField] Texture[] transitionTextures;

        public void StartBattle()
        {
            StartCoroutine(StartBattleTransition());
        }

        public void EndBattle()
        {

        }

        private IEnumerator StartBattleTransition()
        {
            yield return LoadRandomTransition();
            yield return TransitionFadeIn(1.5f);
            yield return new WaitForSeconds(10.5f);
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        }

        private void EndBattleTransition()
        {

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

#if UNITY_EDITOR
        // For keeping consistency in editor
        private void OnApplicationQuit() => ResetTransitionMaterial();
#endif
    }

}
