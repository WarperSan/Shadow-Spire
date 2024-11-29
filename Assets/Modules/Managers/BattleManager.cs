using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] Texture2D[] transitionTextures;
        [SerializeField] Material battleTransitionMat;

        #region Battle Transition

        public void StartBattle()
        {
            StartBattleTransition();
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        }

        public  void EndBattle()
        {
            
        }

        private void StartBattleTransition()
        {
            int rdmIndex = Random.Range(0, transitionTextures.Length);

        }

        private void EndBattleTransition()
        {

        }

        #endregion
    }

}
