using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class KillProgressBar : MonoBehaviour
{
    public Image progressBar;          // Reference to the fill image
    public int killsRequired = 10;     // The number of kills required to fill the bar
    private int currentKills = 0;      // Current number of kills
    public float powerUpDuration = 10f; // Duration the power-up lasts (in seconds)

    private bool powerUpActive = false;

    // Reference to the Shotgun script to activate the one-shot power-up
    [SerializeField] private Shotgun shotgun;
    [SerializeField] private TextMeshProUGUI surgeText;

    // This function should be called each time you kill an enemy
    public void AddKill()
    {
        if (powerUpActive) return;  // Don't add kills if the power-up is still active

        currentKills++;

        // Update the progress bar
        progressBar.fillAmount = (float)currentKills / killsRequired;

        // Check if the bar is full
        if (currentKills >= killsRequired)
        {
            surgeText.gameObject.GetComponent<Animator>().SetTrigger("Surge");
            StartCoroutine(ActivatePowerUp());
        }
    }

    private IEnumerator ActivatePowerUp()
    {
        powerUpActive = true;

        // Keep the bar full at the start of the power-up
        progressBar.fillAmount = 1f;

        // Activate the one-shot power-up in the Shotgun script
        shotgun.ActivateOneShotPowerUp();

        // Gradually decrease the fill amount over the power-up duration
        float elapsedTime = 0f;

        while (elapsedTime < powerUpDuration)
        {
            elapsedTime += Time.deltaTime;
            progressBar.fillAmount = Mathf.Lerp(1f, 0f, elapsedTime / powerUpDuration);  // Lerp from 1 to 0 over powerUpDuration
            yield return null;
        }

        // After the power-up duration, reset the progress bar
        ResetProgressBar();
    }

    // Reset the progress bar after the power-up expires
    private void ResetProgressBar()
    {
        currentKills = 0;
        progressBar.fillAmount = 0;
        powerUpActive = false;
        Debug.Log("Power-Up Ended. Progress bar reset.");
    }
}
