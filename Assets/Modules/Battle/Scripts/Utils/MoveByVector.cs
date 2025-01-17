using UnityEngine;

namespace Battle.Utils
{
    public class MoveByVector : MonoBehaviour
    {
        [Tooltip("Direction of the movement")]
        [SerializeField]
        private Vector3 direction;

        [Tooltip("By how much the movement is sped up")]
        [SerializeField]
        private float speed;

        [Tooltip("Moves in local space")]
        [SerializeField]
        private bool moveInLocal;

        #region MonoBehaviour

        /// <inheritdoc/>
        private void FixedUpdate() => transform.Translate(speed * Time.fixedDeltaTime * direction.normalized, moveInLocal ? Space.Self : Space.World);

        #endregion
    }
}