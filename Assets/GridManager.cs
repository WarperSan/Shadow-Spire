using UtilsModule;

namespace GridModule
{
    public class GridManager : Singleton<GridManager>
    {
        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}