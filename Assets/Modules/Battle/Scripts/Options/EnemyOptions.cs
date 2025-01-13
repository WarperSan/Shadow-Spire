using UnityEngine;
using UtilsModule;

namespace Battle.Options
{
    public class EnemyOptions : UIOptions<EnemyOption, EnemyOptionData>
    {

        /// <inheritdoc/>
        protected override void AlignOptions(Transform[] elements) => elements.AlignHorizontally(rectTransform);

        
        /// <inheritdoc/>
        protected override void OnMoveSelected(Vector2 dir)
        {
            int startIndex = selectedIndex;

            do
            {
                base.OnMoveSelected(dir); // Move once

                var data = loadedOptions[selectedIndex].GetOption();

                // Check if selected is alive
                if (!data.Entity.IsDead)
                    break;
            } while (selectedIndex != startIndex);
        }

        public void FindNextValid(Vector2 dir) => OnMoveSelected(dir);
    }
}