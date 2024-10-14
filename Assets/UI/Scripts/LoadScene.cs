using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject signIn;

    private GameManager gameManager;
    public void Start()
    {
        Time.timeScale = 1.0f;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();  
       
    }

  
    public void LoadSceneByIndex(int sceneIndex)
    {
        if(signIn !=null)
        {
            signIn.SetActive(false);
        }
        if (SceneManager.GetActiveScene().buildIndex ==1) { 
        gameManager.ReactivateSignIn();
        }
        gameManager.score = 0;
        SceneManager.LoadScene(sceneIndex);
          
        
        

    }
}
