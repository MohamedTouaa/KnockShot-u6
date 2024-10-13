using SmallHedge.SoundManager;
using UnityEngine;
using System.Collections;

public class SuicideAttack : EnemyAttack
{
    [SerializeField] private float explosionRadius = 5f;     // Explosion radius for the suicide attack
    [SerializeField] private float explosionDamage = 50f;    // Damage dealt by the explosion
    [SerializeField] private GameObject explosionEffect;

    [SerializeField]
    private ExplosiveEnemy explosiveEnemy;

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
    protected override void OnTriggerExit(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {
            explosiveEnemy.suicideAttack();
        }
    }
    // Method to trigger the explosion when the enemy suicides
    private void TriggerExplosion()
    {
        Debug.Log("Suicide Enemy exploded!");

        explosiveEnemy.suicideAttack();

       
        
    }
}
