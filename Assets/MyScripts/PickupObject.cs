using SmallHedge.SoundManager;
using TMPro;
using UnityEngine;

public class PickupObject : MonoBehaviour
{


  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player is the one picking up
        {
            SoundManager.PlaySound(SoundType.PowerUp, null, 1f);
            

            // Notify the spawner that the object was collected
            FindObjectOfType<EnemySpawner>().OnPickupCollected();

            // Destroy the pickup object
            Destroy(gameObject);
        }
    }
}
