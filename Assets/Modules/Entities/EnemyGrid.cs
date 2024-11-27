using System.Collections;
using Entities.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entities
{
    public class EnemyGrid : GridEntity, ITurnable, IMovable
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            Debug.Log("hit");
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        }

        #region ITurnable

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            yield return Movement.LEFT;
        }

        #endregion

        #region IMovable

        Transform IMovable.Transform => transform;

        #endregion
    }
}