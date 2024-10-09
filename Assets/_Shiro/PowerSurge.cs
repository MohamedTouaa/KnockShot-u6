using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KillProgressBar : MonoBehaviour
{
    public Image progressBar;          // Reference to the fill image
    public int killsRequired = 10;     // The number of kills required to fill the bar
    private int currentKills = 0;      // Current number of kills
    public float powerUpDuration = 10f; // Duration the power-up lasts (in seconds)

    private bool powerUpActive = false;

    // Reference to the Shotgun script to activate the one-shot power-up
    [SerializeField] private Shotgun shotgun;

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
            // Activate the one-shot power-up on the Shotgun and prevent further kills from affecting the bar
            StartCoroutine(ActivatePowerUp());
        }
    }

    // Coroutine to handle the power-up duration
    private IEnumerator ActivatePowerUp()
    {
        powerUpActive = true;

        // Keep the bar full during the power-up
        progressBar.fillAmount = 1f;

        // Activate the one-shot power-up in the Shotgun script
        shotgun.ActivateOneShotPowerUp();

        // Wait for the power-up duration
        yield return new WaitForSeconds(powerUpDuration);

        // Reset the progress bar and allow new kills to be counted
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
