using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;

public class ExplosiveEnemy : EnemyHealth
{
    [SerializeField]
    private float explosionRadius = 5f;
    [SerializeField]
    private float explosionDamage = 100;
    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private SkinnedMeshRenderer bomberRenderer;

    private bool hasExploded = false; // Flag to ensure Explode is called only once

    protected override void Die()
    {
     
    

        base.Die(); // Call base class Die method for general death behavior

        bomberRenderer.enabled = false;
        Explode(); // Add custom explosion behavior
    }

    public void suicideAttack()
    {
        if (!hasExploded)  // Check if it has already exploded
        {
            Die();
        }
    }

    private void Explode()
    {
        if (hasExploded) return; // Ensure Explode is called only once
        hasExploded = true;

        // Show explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position + Vector3.up, Quaternion.identity);
            SoundManager.PlaySound(SoundType.Explosion, null, 0.5f);
        }

        // Deal AOE damage to nearby objects
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                Player player = hit.GetComponent<Player>();
                if (player != null)
                {
                    player.Die();
                }
            }
            else if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null && enemy != this)  // Avoid damaging itself
                {
                    Debug.Log("Deal damage to enemy.");
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }
    }
}
