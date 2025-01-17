using GridEntities.Abstract;
using GridEntities.Interfaces;

namespace GridEntities.Entities
{
    public class ExitEntity : GridEntity, IEventable<PlayerEntity>
    {
        /// <inheritdoc/>
        public void OnEntityLand(PlayerEntity entity) { /* Exit can't move on another entity */ }

        /// <inheritdoc/>
        public void OnEntityLanded(PlayerEntity entity) => Managers.GameManager.Instance.EndLevel();
        
    }
}