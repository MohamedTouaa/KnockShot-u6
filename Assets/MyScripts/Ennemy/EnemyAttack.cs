using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1.5f;    
    [SerializeField] private Animator animator;             

    private bool isAttacking = false;                       
    private Transform player;                               
    private const string Attack = "Attack";                  
    private SphereCollider attackRadius;                   

    private void Awake()
    {
   
        attackRadius = GetComponent<SphereCollider>();
        attackRadius.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isAttacking)
        {
            Debug.Log("Entered");
            player = other.transform;
            StartCoroutine(AttackPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop attacking if the player leaves the attack radius
        if (other.CompareTag("Player"))
        {
            StopAttacking();
        }
    }

    private IEnumerator AttackPlayer()
    {
       
        isAttacking = true;

        while (player != null )
        {
            SoundManager.PlaySound(SoundType.Roar, null, 1);
            animator.SetTrigger(Attack);    
            yield return new WaitForSeconds(0.5f); 
            ApplyDamageToPlayer();

           
            yield return new WaitForSeconds(attackCooldown);
        }

        isAttacking = false;
    }

    private void StopAttacking()
    {
        isAttacking = false;
        StopAllCoroutines();
    }

    private void ApplyDamageToPlayer()
    {
        // Assuming the player has a script like "PlayerHealth" to handle damage
        Player playerHealth = player.GetComponent<Player>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage();
        }
        else
        {
            Debug.LogWarning("Target does not have a PlayerHealth component to receive damage.");
        }
    }
}
