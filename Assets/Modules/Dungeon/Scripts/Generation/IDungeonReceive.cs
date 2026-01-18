namespace Dungeon.Generation
{
	/// <summary>
	/// Interface used to notify classes about a dungeon's generation
	/// </summary>
	public interface IDungeonReceive
	{
		/// <summary>
		/// Called when a level starts
		/// </summary>
		public void OnLevelStart(DungeonResult level);

		/// <summary>
		/// Called when a level ends
		/// </summary>
		public void OnLevelEnd(DungeonResult   level);
	}
}