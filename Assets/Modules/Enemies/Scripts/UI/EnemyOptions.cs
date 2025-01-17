using UI.Abstract;
using UnityEngine;
using Utils;

namespace Enemies.UI
{
    /// <summary>
    /// Menu used for the enemy selection
    /// </summary>
    public class EnemyOptions : UIOptions<EnemyOption, EnemyOptionData>
    {
        /// <inheritdoc/>
        protected override void AlignOptions(Transform[] elements) => elements.AlignHorizontally(Rect);
        
        /// <inheritdoc/>
        protected override void OnMoveSelected(Vector2 dir)
        {
            int startIndex = selectedIndex;

            do
            {
                base.OnMoveSelected(dir); // Move once

                EnemyOptionData data = loadedOptions[selectedIndex].GetOption();

                // Check if selected is alive
                if (!data.Entity.IsDead)
                    break;
            } while (selectedIndex != startIndex);
        }

        public void FindNextValid(Vector2 dir) => OnMoveSelected(dir);
    }
}