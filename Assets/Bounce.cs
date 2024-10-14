using SmallHedge.SoundManager;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float bounceForce = 10f;
    private Rigidbody rb;
    [SerializeField]
    private GameObject boing;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                
                Vector3 normal = collision.contacts[0].normal;

                
                Vector3 bounceDirection = Vector3.Reflect(rb.linearVelocity, normal);

                // Apply force in the bounce direction
                rb.linearVelocity = bounceDirection.normalized * bounceForce;
                SoundManager.PlaySound(SoundType.Boing, null, 0.9f);
                
                
            }
        }
    }

  
}
