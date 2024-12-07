using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arcade
{
    /// <summary>
    /// Script for the interaction between the player and the arcade
    /// </summary>
    public class Arcade : MonoBehaviour, IInteractable
    {
        [SerializeField] Camera ArcadeCamera;
        [SerializeField] AudioLowPassFilter AudioFilter;

        /// <inheritdoc/>
        public void OnClick() => StartCoroutine(Start2DGame());

        public IEnumerator Start2DGame()
        {
            Debug.Log("activated");
            //AudioFilter.enabled = true;
            var loadScene = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

            while (!loadScene.isDone)
                yield return null;
        }

        private void Start()
        {
            OnClick();
        }
    }
}
