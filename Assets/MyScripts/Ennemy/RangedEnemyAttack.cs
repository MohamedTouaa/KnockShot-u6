using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAttack : EnemyAttack
{
    public NavMeshAgent Agent;
    public Bullet BulletPrefab;
    public Transform bulletSpawnPosition;
    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0); // Adjust spawn point height
   
    private ObjectPool BulletPool;

    
    [SerializeField]
    private float AimDuration = 2f; // Time spent aiming before shooting
    [SerializeField]
    private float AttackDelay = 1.5f; // Delay between attacks
    private Transform player;
    private Bullet bullet;

    [SerializeField]
    private Animator animator;

    private const string IsAiming = "IsAiming";
    private const string Attacka = "Attack";

    private void Awake()
    {
        BulletPool = ObjectPool.CreateInstance(BulletPrefab, Mathf.CeilToInt((1 / AttackDelay) * BulletPrefab.AutoDestroyTime));
     
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detect player by tag or layer
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            StartCoroutine(Attack());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(Attack());
            player = null;
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds attackWait = new WaitForSeconds(AttackDelay);

        while (player != null)
        {
            // Aim at the player
            yield return StartCoroutine(AimAtTarget());

            // Shoot after aiming
            ShootAtTarget();

            yield return attackWait; // Wait before the next attack
        }

        Agent.enabled = true;
    }

    private IEnumerator AimAtTarget()
    {
        float aimTimer = 0f;

        while (aimTimer < AimDuration && player != null)
        {
            animator.SetBool(IsAiming, true);
            aimTimer += Time.deltaTime;
            Vector3 directionToTarget = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

            // Smoothly rotate towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, aimTimer / AimDuration);

            yield return null;
        }
        animator.SetBool(IsAiming, false);
    }

    private void ShootAtTarget()
    {
        PoolableObject poolableObject = BulletPool.GetObject();
        if (poolableObject != null)
        {
            bullet = poolableObject.GetComponent<Bullet>();

            bullet.transform.position = bulletSpawnPosition.position + BulletSpawnOffset; // Set bullet spawn position
            bullet.transform.rotation = transform.rotation; // Set bullet direction

            animator.SetTrigger(Attacka);
            bullet.Rigidbody.AddForce(transform.forward * BulletPrefab.MoveSpeed, ForceMode.VelocityChange);
        }
    }
}
