using SmallHedge.SoundManager;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public enum PowerUpType
    {
        KnockbackEnemy,
        Multiplier,
        Grenade
    }
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();  
    }

    public PowerUpType powerUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.PlaySound(SoundType.PowerUp, null, 1f);

            // Activate the power-up
            ActivatePowerUp(powerUp);

            // Notify the spawner that the object was collected
            FindObjectOfType<EnemySpawner>().OnPickupCollected();

            // Notify the UI manager to update the power-up icon
            PowerUpUIManager uiManager = FindObjectOfType<PowerUpUIManager>();
            if (uiManager != null)
            {
                uiManager.UpdatePowerUpIcon(powerUp);
            }

            Destroy(gameObject); // Destroy the power-up object
        }
    }

    private void ActivatePowerUp(PowerUpType powerUpType)
    {
        Shotgun shotgun = FindObjectOfType<Shotgun>();
        

        switch (powerUpType)
        {
            case PowerUpType.KnockbackEnemy:
                if (shotgun != null) shotgun.applyKnockbackToEnemies = true;
                break;

            case PowerUpType.Multiplier:
                if (gameManager != null) gameManager.isDouble = true;
                break;

            case PowerUpType.Grenade:
                if (shotgun != null) shotgun.isGrenadeLauncherActive = true;
                break;
        }
    }

    public void DeactivateAllPowerUps()
    {
        Shotgun shotgun = FindObjectOfType<Shotgun>();
        EnemyHealth enemyHealth = FindObjectOfType<EnemyHealth>();

        if (shotgun != null)
        {
            shotgun.applyKnockbackToEnemies = false;
            shotgun.isGrenadeLauncherActive = false;
        }

        if (gameManager != null) gameManager.isDouble = false;

        // Hide the power-up icon through the UI manager
        PowerUpUIManager uiManager = FindObjectOfType<PowerUpUIManager>();
        if (uiManager != null)
        {
            uiManager.HidePowerUpIcon();
        }
    }
}
