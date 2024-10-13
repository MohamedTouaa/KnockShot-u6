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

    private bool hasExploded = false; 

    protected override void Die()
    {
     
    

        base.Die(); 

        bomberRenderer.enabled = false;
        Explode(); 
    }

    public void suicideAttack()
    {
        if (!hasExploded)  
        {
            Die();
        }
    }

    private void Explode()
    {
        if (hasExploded) return; 
        hasExploded = true;

     
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position + Vector3.up, Quaternion.identity);
            SoundManager.PlaySound(SoundType.Explosion, null, 0.5f);
        }

      
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
                if (enemy != null && enemy != this)  
                {
                    Debug.Log("Deal damage to enemy.");
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }
    }
}
