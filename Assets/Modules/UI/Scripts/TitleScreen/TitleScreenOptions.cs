using System.Collections;
using Managers;
using UI.Abstract;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI.TitleScreen
{
	/// <summary>
	/// Menu used for the title screen
	/// </summary>
	public class TitleScreenOptions : UIOptions<TitleScreenOption, TitleScreenOptionData>
	{
		public Graphic blackout;

		private void Start()
		{
			LoadOptions(new TitleScreenOptionData[]
			{
				new()
				{
					Text = "Play",
					OnEnter = OnPlay
				},
				new()
				{
					Text = "Quit",
					OnEnter = OnQuit
				}
			});

			ShowSelection();

			InputManager.Instance.onMoveUI.AddListener(Move);
			InputManager.Instance.onEnterUI.AddListener(Enter);

			Cursor.visible = false;
		}

		#region Play Sequence

		private bool _isRunningPlaySequence;

		public void OnPlay()
		{
			if (_isRunningPlaySequence)
				return;

			StartCoroutine(PlaySequence());
			_isRunningPlaySequence = true;
		}

		private IEnumerator PlaySequence()
		{
			yield return blackout.FadeIn(4, 0.2f);
			yield return new WaitForSeconds(1f);

			SceneManager.LoadScene("Game", LoadSceneMode.Single);
		}

		#endregion

		#region Quit Sequence

		private bool _isRunningQuitSequence;

		public void OnQuit()
		{
			if (_isRunningQuitSequence)
				return;

			StartCoroutine(QuitSequence());
			_isRunningQuitSequence = true;
		}

		private IEnumerator QuitSequence()
		{
			yield return blackout.FadeIn(4, 0.2f);

			Application.Quit();
		}

		#endregion

		#region UIOptions

		/// <inheritdoc/>
		protected override void AlignOptions(Transform[] elements) => elements.AlignVertically(Rect);

		/// <inheritdoc/>
		protected override void OnMoveSelected(Vector2 dir) => base.OnMoveSelected(new Vector2(dir.y, dir.x));

		#endregion
	}
}