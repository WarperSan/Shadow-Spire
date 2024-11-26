using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hit");
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit");
        SceneManager.LoadScene("BattleScene");
    }

}
