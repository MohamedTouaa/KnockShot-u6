using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f;  // Radius of the explosion
    [SerializeField] private float explosionForce = 700f; // Force applied to nearby objects
    [SerializeField] private float damageAmount = 50f;    // Damage dealt to enemies
    [SerializeField] private GameObject explosionEffect;  // Prefab of explosion visual effect

    private bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Ensure the grenade only explodes once, when it touches any object
        if (!hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        // Show explosion effect
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Find all nearby objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            // Check if the object has an EnemyHealth component
            EnemyHealth enemyHealth = nearbyObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Apply damage to the enemy
                enemyHealth.TakeDamage(damageAmount);
            }

            // Apply explosion force to objects with a Rigidbody
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Destroy the grenade object after the explosion
        Destroy(gameObject);
    }

    // Visualize the explosion radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
