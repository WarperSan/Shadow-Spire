using UnityEngine;

namespace Battle.Utils
{
    public class TimeToLive : MonoBehaviour
    {
        [SerializeField] float lifetime = 10.0f;
        float initTime;

        private void OnEnable()
        {
            initTime = Time.time;
        }

        private void OnDisable()
        {
            Destroy(gameObject);
        }

        void Update()
        {
            if (Time.time > initTime + lifetime)
                enabled = false;
        }
    }

}