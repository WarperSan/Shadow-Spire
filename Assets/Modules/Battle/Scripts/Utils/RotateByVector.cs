using UnityEngine;

namespace Battle.Utils
{
    public class RotateByVector : MonoBehaviour
    {
        [Tooltip("Direction of the rotation")]
        [SerializeField]
        private Vector3 direction = Vector3.forward;

        [Tooltip("By how much the rotation is sped up")]
        [SerializeField]
        private float speed = 1f;

        #region MonoBehaviour

        /// <inheritdoc/>
        private void FixedUpdate() => transform.Rotate(speed * Time.fixedDeltaTime * direction.normalized);

        #endregion
    }
}