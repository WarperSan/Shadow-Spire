using System;
using TMPro;
using UI.Abstract;
using UnityEngine;

namespace UI.Battle
{
	public class BattleOptionData : UIOptionData
	{
		public string Text;
		public Func<bool> IsValid = () => true;
	}

	/// <summary>
	/// Script that handles to display a single option
	/// </summary>
	public class BattleOption : UIOption<BattleOptionData>
	{
		#region Fields

		[Header("Fields")]
		[SerializeField]
		private TextMeshProUGUI text;

		#endregion

		#region API

		/// <inheritdoc/>
		protected override void OnLoadOption(BattleOptionData option)
		{
			text.text = option.Text;
			text.color = option.IsValid.Invoke() ? Color.white : Color.gray;
		}

		/// <inheritdoc/>
		public override void Select()
		{
			text.text = string.Format("> {0} <", LoadedOption.Text);
		}

		/// <inheritdoc/>
		public override void Deselect()
		{
			text.text = LoadedOption.Text;
		}

		#endregion
	}
}