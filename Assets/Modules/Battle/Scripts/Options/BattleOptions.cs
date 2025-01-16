using UnityEngine;
using Utils;

namespace Battle.Options
{
    public class BattleOptions : UIOptions<BattleOption, BattleOptionData>
    {
        /// <inheritdoc/>
        protected override void AlignOptions(Transform[] elements) => elements.AlignHorizontally(rectTransform);
    }
}