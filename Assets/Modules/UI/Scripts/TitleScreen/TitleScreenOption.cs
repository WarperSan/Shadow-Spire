using TMPro;
using UI.Abstract;
using UnityEngine;

namespace UI.TitleScreen
{
	/// <summary>
	/// Data used for the title screen menu
	/// </summary>
	public class TitleScreenOptionData : UIOptionData
	{
		public string Text;
	}

	/// <summary>
	/// Option used for the title screen menu
	/// </summary>
	public class TitleScreenOption : UIOption<TitleScreenOptionData>
	{
		[SerializeField]
		private TextMeshProUGUI optionTitle;

		#region UIOption

		/// <inheritdoc/>
		protected override void OnLoadOption(TitleScreenOptionData option)
		{
			optionTitle.text = LoadedOption.Text;
		}

		/// <inheritdoc/>
		public override void Select()
		{
			optionTitle.text = string.Format("> {0} <", LoadedOption.Text);
		}

		/// <inheritdoc/>
		public override void Deselect()
		{
			optionTitle.text = LoadedOption.Text;
		}

		#endregion
	}
}