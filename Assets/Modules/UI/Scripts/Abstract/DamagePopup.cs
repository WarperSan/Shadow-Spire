using System.Collections;

namespace UI.Abstract
{
	/// <summary>
	/// Component that represents a damage popup
	/// </summary>
	public abstract class DamagePopup : UIComponent
	{
		/// <summary>
		/// Displays the given amount of damage
		/// </summary>
		protected abstract void SetDamage(uint amount);

		/// <summary>
		/// Starts the animation for this popup
		/// </summary>
		public IEnumerator StartAnimation(uint amount)
		{
			SetDamage(amount);

			yield return OnAnimation();

			yield return null;

			Destroy(gameObject);
		}

		/// <summary>
		/// Executes the animation for this popup
		/// </summary>
		protected abstract IEnumerator OnAnimation();
	}
}