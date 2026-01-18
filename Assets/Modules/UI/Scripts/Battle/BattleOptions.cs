using UI.Abstract;
using UnityEngine;
using Utils;

namespace UI.Battle
{
	public class BattleOptions : UIOptions<BattleOption, BattleOptionData>
	{
		/// <inheritdoc/>
		protected override void AlignOptions(Transform[] elements) => elements.AlignHorizontally(Rect);
	}
}