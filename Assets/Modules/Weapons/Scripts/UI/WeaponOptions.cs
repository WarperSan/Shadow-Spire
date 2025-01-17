using UI.Abstract;
using UnityEngine;
using Utils;

namespace Weapons.UI
{
    /// <summary>
    /// Menu used for the weapon selection
    /// </summary>
    public class WeaponOptions : UIOptions<WeaponOption, WeaponOptionData>
    {
        /// <inheritdoc/>
        protected override void AlignOptions(Transform[] elements) => elements.AlignHorizontally(Rect);
    }
}