using UnityEngine;
 using TMPro;

namespace OptimizationModule
{
    public class FPSCounter : MonoBehaviour
    {
        public TextMeshProUGUI fpsText;

        private float deltaTime;

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            float fps = 1.0f / deltaTime;
            fpsText.text = $"{fps:0.} FPS";
        }
    }
}