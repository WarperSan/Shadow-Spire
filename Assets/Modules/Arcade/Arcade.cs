using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

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
        CameraUtils.Camera2D.IsIn3D = true;
        OnClick();
    }

}
