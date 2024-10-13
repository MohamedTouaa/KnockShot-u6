using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void updateScore(int i)
    {
        scoreText.gameObject.GetComponent<Animator>().SetTrigger("Score");
        score += i;
        scoreText.text = score.ToString();
    }

    public void RestartGame()
    {
        // Reset game state
        Time.timeScale = 1f; // Resume time // Reset paused state
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }
}
