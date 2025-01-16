using UnityEngine;

namespace UI.Abstract
{
    [RequireComponent(typeof(RectTransform))]
    public class UIComponent : MonoBehaviour
    {
        protected RectTransform Rect;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}