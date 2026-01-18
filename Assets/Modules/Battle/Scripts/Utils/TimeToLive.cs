using UnityEngine;

namespace Battle.Utils
{
	public class TimeToLive : MonoBehaviour
	{
		[SerializeField]
		private float lifetime = 10.0f;

		private float _initTime;

		private void OnEnable()
		{
			_initTime = Time.time;
		}

		private void OnDisable()
		{
			Destroy(gameObject);
		}

		private void Update()
		{
			if (Time.time > _initTime + lifetime)
				enabled = false;
		}
	}
}