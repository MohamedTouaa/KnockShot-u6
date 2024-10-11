using SmallHedge.SoundManager;
using UnityEngine;
using System.Collections;

public class SuicideAttack : EnemyAttack
{
    [SerializeField] private float explosionRadius = 5f;     // Explosion radius for the suicide attack
    [SerializeField] private float explosionDamage = 50f;    // Damage dealt by the explosion
    [SerializeField] private GameObject explosionEffect;


    [SerializeField]
    private SkinnedMeshRenderer bomberRenderer;// Prefab for the explosion visual effect

    // This method will override the existing AttackPlayer method to include suicide logic
    protected override IEnumerator AttackPlayer()
    {
        isAttacking = true;

        while (player != null)
        {
            // Play attack sound and trigger the animation
            SoundManager.PlaySound(SoundType.Explosion, null, 0.5f);
            animator.SetTrigger(Attack);

            yield return new WaitForSeconds(0.5f);

            // Apply damage to the player
            ApplyDamageToPlayer();

            // Trigger the suicide explosion after the attack
            TriggerExplosion();

            yield return new WaitForSeconds(attackCooldown);
        }

        isAttacking = false;
    }

    // Method to trigger the explosion when the enemy suicides
    private void TriggerExplosion()
    {
        Debug.Log("Suicide Enemy exploded!");

        
        if (explosionEffect != null)
        {
            SoundManager.PlaySound(SoundType.Explosion, null, 1);
            bomberRenderer.enabled = false;
            Instantiate(explosionEffect, transform.position +Vector3.up, Quaternion.identity);
        }

        // Apply AOE damage to nearby entities (including enemies)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // Deal damage to the player
                Player player = hit.GetComponent<Player>();
                if (player != null)
                {
                    player.Die();
                }
            }
            else if (hit.CompareTag("Enemy"))
            {
                // Deal damage to other enemies
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null && enemyHealth != this)
                {
                    enemyHealth.TakeDamage(explosionDamage);
                }
            }
        }

        // Destroy this enemy after the explosion
        
    }
}
