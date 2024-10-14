using HYPLAY.Demo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Static instance
    public int score = 0;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private DemoLeaderboard leaderboard;

    [SerializeField]
    public GameObject SignIn;

    public bool isDoubleScore;

    private void Awake()
    {
        // Check if an instance of GameManager already exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager
        }
    }

    public void ReactivateSignIn()
    {
        SignIn.SetActive(true);    
    }



    private void Update()
    {
        if (scoreText == null && SceneManager.GetActiveScene().buildIndex !=0)
        {
            scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
        }
    }



    public void updateScore(int i)
    {
        if (!isDoubleScore)
        {
            scoreText.gameObject.GetComponent<Animator>().SetTrigger("Score");
            score += i;
            scoreText.text = score.ToString();
            leaderboard.SetScore(score);
        }
        else
        {
            scoreText.gameObject.GetComponent<Animator>().SetTrigger("Score");
            score += i*2;
            scoreText.text = score.ToString();
            leaderboard.SetScore(score);
        }
       
    }

    public void SubmitScore()
    {
        leaderboard.SubmitScore();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        this.score = 0;

    }
}
