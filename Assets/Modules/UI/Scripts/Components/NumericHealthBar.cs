using System.Collections;
using TMPro;
using UI.Abstract;
using UnityEngine;

namespace UI.Components
{
	public class NumericHealthBar : HealthBar
	{
		#region Fields

		#pragma warning disable IDE0044 // Add readonly modifier

		[Header("Fields")]
		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private Transform popupContainer;

		[SerializeField]
		private DamagePopup popupPrefab;

		[SerializeField]
		private bool useBlink;

		#pragma warning restore IDE0044 // Add readonly modifier

		private string format;

		#endregion

		#region UIComponent

		/// <inheritdoc/>
		protected override void OnAwake()
		{
			base.OnAwake();

			format = _text.text;
		}

		#endregion

		#region HealthBar

		/// <inheritdoc/>
		public override void SetHealth(uint health, uint maxHealth)
		{
			_text.text = string.Format(format, health, maxHealth);
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

		private Coroutine blinkCoroutine;

		private void Blink(int count)
		{
			if (blinkCoroutine != null)
				StopCoroutine(blinkCoroutine);

			blinkCoroutine = StartCoroutine(BlinkCoroutine(count));
		}

		private IEnumerator BlinkCoroutine(int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return new WaitForSeconds(0.08f);

				_text.enabled = false;

				yield return new WaitForSeconds(0.08f);

				_text.enabled = true;
			}

			// Clear coroutine
			blinkCoroutine = null;
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