using UnityEngine;

namespace ExtensionsModule
{
    public static class TransformExtension
    {
        /// <summary>
        /// Lerps to the target's position and rotation by the given factor
        /// </summary>
        public static void LerpToTarget(this Transform transform, Transform target, float duration)
        {
            if (target == null)
                return;

            // Lerp position
            Vector3 pos = transform.position.LerpAll(target.position, duration);
            transform.position = pos;

            // Lerp angle
            Vector3 angle = transform.eulerAngles.LerpAngleAll(target.eulerAngles, duration);
            transform.eulerAngles = angle;
        }
        
        /// <summary>
        /// Distances between two transform
        /// </summary>
        /// <returns></returns>
        public static float Distance(this Transform self, Transform target)
        {
            // If either is invalid, return 0
            if (self == null || target == null)
                return 0;

            return Vector3.Distance(self.position, target.position);
        }
    }
}