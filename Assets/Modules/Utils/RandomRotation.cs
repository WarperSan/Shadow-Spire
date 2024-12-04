using UnityEngine;

namespace UtilsModule
{
    public class RandomRotation : MonoBehaviour
    {
        public Vector3 minRotation;
        public Vector3 maxRotation;
        public Vector3 axis;

        private void Start()
        {
            Vector3 rotation = transform.localRotation.eulerAngles;

            if (axis.x == 1)
                rotation.x = UnityEngine.Random.Range(minRotation.x, maxRotation.x);

            if (axis.y == 1)
                rotation.y = UnityEngine.Random.Range(minRotation.y, maxRotation.y);

            if (axis.z == 1)
                rotation.z = UnityEngine.Random.Range(minRotation.z, maxRotation.z);

            transform.localRotation = Quaternion.Euler(rotation);
        }

        private void OnValidate()
        {
            if (axis.x != 1)
                axis.x = 0;

            if (axis.y != 1)
                axis.y = 0;

            if (axis.z != 1)
                axis.z = 0;
        }
    }
}
