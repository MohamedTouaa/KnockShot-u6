using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIManager : MonoBehaviour
{
    public Image powerUpIcon;  // Reference to the UI Image

    // Sprites for each power-up type
    public Sprite knockbackIcon;
    public Sprite multiplierIcon;
    public Sprite grenadeIcon;

    // Updates the UI based on the power-up type
    public void UpdatePowerUpIcon(PickupObject.PowerUpType powerUpType)
    {
        if (powerUpIcon == null) return; // Safety check

        switch (powerUpType)
        {
            case PickupObject.PowerUpType.KnockbackEnemy:
                powerUpIcon.sprite = knockbackIcon;
                powerUpIcon.gameObject.SetActive(true);  // Enable the Image object
                break;

            case PickupObject.PowerUpType.Multiplier:
                powerUpIcon.sprite = multiplierIcon;
                powerUpIcon.gameObject.SetActive(true);  // Enable the Image object
                break;

            case PickupObject.PowerUpType.Grenade:
                powerUpIcon.sprite = grenadeIcon;
                powerUpIcon.gameObject.SetActive(true);  // Enable the Image object
                break;

            default:
                powerUpIcon.gameObject.SetActive(false);  // Hide if no valid power-up
                break;
        }
    }

    // Hide the icon when no power-ups are active
    public void HidePowerUpIcon()
    {
        if (powerUpIcon != null)
        {
            powerUpIcon.gameObject.SetActive(false);  // Deactivate the Image object
        }
    }
}
