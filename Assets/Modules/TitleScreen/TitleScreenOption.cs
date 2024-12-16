using Battle.Options;
using TMPro;
using UnityEngine;

namespace TitleScreen
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
            optionTitle.text = loadedOption.Text;
        }

        /// <inheritdoc/>
        public override void Select()
        {
            optionTitle.text = string.Format("> {0} <", loadedOption.Text);
        }

        /// <inheritdoc/>
        public override void Deselect()
        {
            optionTitle.text = loadedOption.Text;
        }

        #endregion
    }
}
