using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();  
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;

        gameManager.score= 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
       
        SceneManager.LoadScene(sceneIndex);


    }
}
