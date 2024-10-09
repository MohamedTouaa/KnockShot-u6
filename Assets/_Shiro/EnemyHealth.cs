using SmallHedge.SoundManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private NavMeshAgent agent;

    // Reference to the Animator
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private TextMeshProUGUI score;

    private const string DamageTrigger = "Damage";
    private const string DieTrigger = "Die";

    [SerializeField]
    private EnnemyMovement enemyMovement;

    public float stunDuration = 0.15f;

    // VFX prefab for hit effect
    [SerializeField]
    private GameObject hitVFX;

    // Reference to the enemy's head transform (assign this in the inspector)
    [SerializeField]
    private Transform headTransform;


    private EnemySpawner spawner;
    private GameManager manager;
 
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();  
    }
    private void OnEnable()
    {
        currentHealth = maxHealth;
        if (enemyMovement == null)
        {
            enemyMovement = GetComponent<EnnemyMovement>();
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (enemyMovement == null)
        {
            enemyMovement = GetComponent<EnnemyMovement>();
        }
    }

    // Function to take damage
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took damage: " + amount);
        SoundManager.PlaySound(SoundType.Hit, null, 1f);

        // Trigger the Damage animation
        animator.SetTrigger(DamageTrigger);

        // Spawn VFX on the enemy's head
        SpawnHitVFX();

        StartCoroutine(StunEnemy());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private IEnumerator StunEnemy()
    {
        if (enemyMovement != null)
        {
            StopEnemy();
        }

        yield return new WaitForSeconds(stunDuration);

        if (enemyMovement != null)
        {
            ResumeEnemy();
        }
    }

    public void StopEnemy()
    {
        agent.isStopped = true;
        animator.SetBool("IsWalking", false);
    }

    public void ResumeEnemy()
    {
        agent.isStopped = false;
        animator.SetBool("IsWalking", true);
    }

    private void Die()
    {
      
        Debug.Log(gameObject.name + " died.");
        animator.SetTrigger(DieTrigger);
        StartCoroutine(DieAfterAnimation());
    }

    private IEnumerator DieAfterAnimation()
    {
        score.text = "+" + 10.ToString();
        score.gameObject.GetComponent<Animator>().SetTrigger("Score");
        manager.updateScore(10);
        yield return new WaitForSeconds(1f);
        spawner.killChaser++;
        this.gameObject.GetComponent<Ennemy>().OnDisable(); 
    }

    // Function to spawn the VFX on hit
    private void SpawnHitVFX()
    {
        if (hitVFX != null && headTransform != null)
        {
            Instantiate(hitVFX, headTransform.position, Quaternion.identity);
        }
    }
}
