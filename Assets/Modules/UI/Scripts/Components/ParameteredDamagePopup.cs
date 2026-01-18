using System.Collections;
using TMPro;
using UI.Abstract;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace UI.Components
{
	public class ParameteredDamagePopup : DamagePopup
	{
		#region Fields

		#pragma warning disable IDE0044 // Add readonly modifier

		[FormerlySerializedAs("_text")]
		[Header("Fields")]
		[SerializeField]
		private TMP_Text text;

		[SerializeField]
		private Vector3 startPos;

		[SerializeField]
		private Vector3 endPos;

		#pragma warning restore IDE0044 // Add readonly modifier

		private string _format;

		#endregion

		#region UIComponent

		/// <inheritdoc/>
		protected override void OnAwake()
		{
			base.OnAwake();
			_format = text.text;
		}

		#endregion

		#region DamagePopup

		/// <inheritdoc/>
		protected override void SetDamage(uint amount)
		{
			text.text = string.Format(_format, amount);
		}

		/// <inheritdoc/>
		protected override IEnumerator OnAnimation()
		{
			Coroutine[] parallel = new Coroutine[]
			{
				StartCoroutine(HealthPopup_Position()),
				StartCoroutine(HealthPopup_Alpha())
			};

			foreach (Coroutine item in parallel)
				yield return item;
		}

		#endregion

		#region Animations

		private IEnumerator HealthPopup_Position()
		{
			yield return Rect.TranslateLocal(32,
				40f / 60f / 32,
				startPos,
				endPos);
		}

		private IEnumerator HealthPopup_Alpha()
		{
			yield return new WaitForSeconds(0.5f);
			yield return text.FadeOut(32, 10f / 60f / 32);
		}

		#endregion
	}
}