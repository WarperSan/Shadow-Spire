using System.Collections;
using GridModule;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Enemies
{
    public class EnemyGrid : GridEntity
    {
        protected override IEnumerator Think()
        {
            yield return Movement.LEFT;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            Debug.Log("hit");
            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        }
    }
}