using System.Collections;
using TMPro;
using UI.Battle;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Managers
{
	public class UIManager : MonoBehaviour
	{
		#region Blackout

		private const int BLACKOUT_TICKS = 4;

		[Space]
		[SerializeField]
		private Graphic blackout;

		public IEnumerator FadeInBlackout(int  ticks = BLACKOUT_TICKS, float delay = 0.2f) => blackout.FadeIn(ticks, delay);
		public IEnumerator FadeOutBlackout(int ticks = BLACKOUT_TICKS, float delay = 0.2f) => blackout.FadeOut(ticks, delay);

		#endregion

		#region Next Level

		private const string NEXT_LEVEL_TEXT = "Level\n{0}";

		[Space]
		[SerializeField]
		private TextMeshProUGUI nextLevel;

		public IEnumerator ShowNextLevel(int current, int next)
		{
			yield return new WaitForSeconds(0.6f);

			nextLevel.gameObject.SetActive(true);
			nextLevel.text = string.Format(NEXT_LEVEL_TEXT, current);

			yield return new WaitForSeconds(1.5f);

			nextLevel.text = string.Format(NEXT_LEVEL_TEXT, next);

			yield return new WaitForSeconds(2f);

			nextLevel.gameObject.SetActive(false);

			yield return new WaitForSeconds(0.2f);
		}

		#endregion

		#region Level

		private const string CURRENT_LEVEL_TEXT = "Level\t{0}";

		[Space]
		[SerializeField]
		private TextMeshProUGUI currentLevel;

		public void SetLevel(int level) => currentLevel.text = string.Format(CURRENT_LEVEL_TEXT, level);

		#endregion

		#region Death

		private const string DEATH_TEXT = "YOU DIED\nAT LEVEL {0}";

		[Space]
		[SerializeField]
		private TextMeshProUGUI deathText;

		public IEnumerator DeathSequence(int level, bool fromOverworld)
		{
			deathText.text = string.Format(DEATH_TEXT, level);

			if (fromOverworld)
			{
				yield return new WaitForSeconds(0.5f);
				yield return FadeInBlackout();
			}

			yield return new WaitForSeconds(1f);

			yield return deathText.FadeIn(4, 0.2f);

			yield return new WaitForSeconds(4f);

			yield return deathText.FadeOut(4, 0.2f);

			yield return new WaitForSeconds(1f);
		}

		#endregion

		#region Battle

		[Header("Battle")]
		[SerializeField]
		private Material battleTransitionMaterial;

		[SerializeField]
		private Texture[] battleTransitionTextures;

		public IEnumerator StartBattleTransition()
		{
			yield return BattleTransition.ExecuteTransition(battleTransitionMaterial, battleTransitionTextures, 0.9f);

			AsyncOperation loadScene = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);

			// Wait until loaded
			while (!loadScene.isDone)
				yield return null;

			BattleTransition.ResetMaterial(battleTransitionMaterial);

			yield return new WaitForSeconds(1f); // Dramatic effect
		}

		public IEnumerator EndBattleTransition()
		{
			yield return FadeInBlackout(1, 0);
		}

		#if UNITY_EDITOR
		// For keeping consistency in editor
		private void OnApplicationQuit() => BattleTransition.ResetMaterial(battleTransitionMaterial);
		#endif

		#endregion
	}
}