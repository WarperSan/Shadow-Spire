using System.Collections;

namespace GridEntities.Interfaces
{
	/// <summary>
	/// Defines an entity that can act on a turn
	/// </summary>
	public interface ITurnable
	{
		/// <summary>
		/// Asks this entity for it's next movement
		/// </summary>
		public abstract IEnumerator Think();

		/// <summary>
		/// Called when this entity's turn starts
		/// </summary>
		public virtual void OnTurnStarted() { }

		/// <summary>
		/// Called when this entity's turn ends
		/// </summary>
		public virtual void OnTurnEnded() { }
	}
}