using SmallHedge.SoundManager;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    public float launchForce = 10f;  // Adjust the force applied to the player
    public Vector3 launchDirection = Vector3.up;  // Default launch direction, adjust if needed

    [SerializeField]
    private GameObject boing;

    // Make sure the player has a Rigidbody and is tagged as "Player"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                Instantiate(boing, playerRb.transform.position, Quaternion.identity);
                Vector3 force = launchDirection.normalized * launchForce;
                SoundManager.PlaySound(SoundType.Boing, null, 0.9f);
                // Apply force to the player's Rigidbody
                playerRb.AddForce(force, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Normalize the direction to ensure the force is consistent
                Vector3 force = launchDirection.normalized * launchForce;
                SoundManager.PlaySound(SoundType.Boing, null, 0.9f);
                // Apply force to the player's Rigidbody
                playerRb.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}
