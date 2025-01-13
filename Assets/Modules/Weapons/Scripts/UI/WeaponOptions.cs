using Battle.Options;
using UnityEngine;
using UtilsModule;

namespace Weapons.UI
{
    /// <summary>
    /// Menu used for the weapon selection
    /// </summary>
    public class WeaponOptions : UIOptions<WeaponOption, WeaponOptionData>
    {
        /// <inheritdoc/>
        protected override void AlignOptions(Transform[] elements) => elements.AlignHorizontally(rectTransform);
    }
}