using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public bool isDead = false;

    private void Update()
    {
        if (isDead)
        {
            canvasGroup.alpha += Time.deltaTime;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
