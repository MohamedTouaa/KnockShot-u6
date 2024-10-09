using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void updateScore(int i)
    {
        score += i;
        scoreText.text = score.ToString();
    }
}
