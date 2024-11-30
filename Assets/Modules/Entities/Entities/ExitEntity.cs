using Entities.Interfaces;

namespace Entities
{
    public class ExitEntity : GridEntity, IEventable
    {
        public void OnEntityLand(GridEntity entity)
        {
            if (entity is not PlayerEntity)
                return;

            Managers.GameManager.Instance.EndLevel();
        }
    }
}