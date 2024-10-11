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

    protected override void Die()
    {
        base.Die();
        bomberRenderer.enabled = false;
        Explode();   // Add custom explosion behavior
    }



    private void Explode()
    {
     

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
                    Debug.Log("deal damage");
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }

        // Destroy the enemy after explosion
      
    }
}
