using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CameraUtils
{
    [RequireComponent(typeof(Canvas))]
    public class FollowHighestCamera : MonoBehaviour
    {
        private Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var roots = scene.GetRootGameObjects();
            canvas.worldCamera = roots.First(r => r.activeInHierarchy && r.CompareTag("MainCamera")).GetComponent<Camera>();
        }

        void OnSceneUnloaded(Scene scene)
        {
            Debug.Log("OnSceneUnloaded: " + scene.name);
            canvas.worldCamera = Camera.main;
        }
    }
}
