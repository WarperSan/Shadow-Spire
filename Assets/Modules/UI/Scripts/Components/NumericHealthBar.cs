using System.Collections;
using TMPro;
using UI.Abstract;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Components
{
	public class NumericHealthBar : HealthBar
	{
		#region Fields

		#pragma warning disable IDE0044 // Add readonly modifier

		[FormerlySerializedAs("_text")]
		[Header("Fields")]
		[SerializeField]
		private TMP_Text text;

		[SerializeField]
		private Transform popupContainer;

		[SerializeField]
		private DamagePopup popupPrefab;

		[SerializeField]
		private bool useBlink;

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

		#region HealthBar

		/// <inheritdoc/>
		public override void SetHealth(uint health, uint maxHealth)
		{
			text.text = string.Format(_format, health, maxHealth);
		}

		/// <inheritdoc/>
		public override void HealDamage(uint amount) { }

		/// <inheritdoc/>
		public override void TakeDamage(uint amount)
		{
			if (useBlink)
				Blink(3);

			Damage(amount);
		}

		#endregion

		#region Animations

		private Coroutine _blinkCoroutine;

		private void Blink(int count)
		{
			if (_blinkCoroutine != null)
				StopCoroutine(_blinkCoroutine);

			_blinkCoroutine = StartCoroutine(BlinkCoroutine(count));
		}

		private IEnumerator BlinkCoroutine(int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return new WaitForSeconds(0.08f);

				text.enabled = false;

				yield return new WaitForSeconds(0.08f);

				text.enabled = true;
			}

			// Clear coroutine
			_blinkCoroutine = null;
		}

		private void Damage(uint amount)
		{
			StartCoroutine(DamagePopup(amount));
		}

		private IEnumerator DamagePopup(uint amount)
		{
			GameObject popup = Instantiate(popupPrefab.gameObject, popupContainer);

			yield return null; // Wait for load

			if (popup.TryGetComponent(out DamagePopup damage))
				yield return damage.StartAnimation(amount);

			yield return null;

			Destroy(popup);
		}

		#endregion
	}
}