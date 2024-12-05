using UnityEngine;

namespace CameraUtils
{
    [RequireComponent(typeof(Camera))]
    public class Camera2D : MonoBehaviour
    {
        public static bool IsIn3D = false;

        [SerializeField]
        private RenderTexture RenderTexture2D;

        [SerializeField]
        private Canvas canvas;

        private void Awake()
        {
            var cam = GetComponent<Camera>();

            if (IsIn3D)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = cam;
            }

            if (Camera.main == cam)
                return;

            if (IsIn3D)
            {
                cam.targetTexture = RenderTexture2D;
            }
        }
    }
}
