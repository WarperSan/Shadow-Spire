using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace Arcade
{
    /// <summary>
    /// Script for the interaction between the player and the arcade
    /// </summary>
    public class Arcade : MonoBehaviour, IInteractable
    {
        [SerializeField] CinemachineVirtualCamera ArcadeCamera;
        [SerializeField] PlayerController player;
        [SerializeField] AudioLowPassFilter AudioFilter;

        /// <inheritdoc/>
        public void OnClick() => StartCoroutine(Start2DGame());

        public IEnumerator Start2DGame()
        {
            player.enabled = false;

            var camPlayer = player.GetComponentInChildren<CinemachineVirtualCamera>();
            ArcadeCamera.Priority = camPlayer.Priority + 1;

            AudioFilter.enabled = true;
            var loadScene = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);

            while (!loadScene.isDone)
                yield return null;
        }
    }
}
