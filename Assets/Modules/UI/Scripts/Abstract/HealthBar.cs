namespace UI.Abstract
{
	/// <summary>
	/// Component that represents an health bar
	/// </summary>
	public abstract class HealthBar : UIComponent
	{
		/// <summary>
		/// Sets the health bar to a value
		/// </summary>
		public abstract void SetHealth(uint health, uint maxHealth);

		/// <summary>
		/// Displays a hit of the given amount
		/// </summary>
		public abstract void TakeDamage(uint amount);

		/// <summary>
		/// Displays a heal of the given amount 
		/// </summary>
		public abstract void HealDamage(uint amount);
	}
}