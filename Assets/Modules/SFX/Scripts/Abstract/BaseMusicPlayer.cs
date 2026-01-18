using System.Collections;
using UnityEngine;
using Utils;

namespace SFX.Abstract
{
	/// <summary>
	/// Component that handles a <see cref="AudioSource"/>
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class BaseMusicPlayer : MonoBehaviour
	{
		private AudioSource _audioSource;

		protected virtual void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		public IEnumerator FadeIn()
		{
			_audioSource.volume = 0;
			_audioSource.Play();

			yield return _audioSource.Fade(10,
				0.2f,
				0,
				0.2f);
		}

		public IEnumerator FadeOut()
		{
			float startVolume = _audioSource.volume;

			yield return _audioSource.Fade(10,
				0.2f,
				startVolume,
				0);
		}
	}
}