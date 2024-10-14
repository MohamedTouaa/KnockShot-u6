using JetBrains.Annotations;
using SmallHedge.SoundManager;
using System.Collections;
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
    protected TextMeshProUGUI score;

    private const string DamageTrigger = "Damage";
    private const string DieTrigger = "Die";

    [SerializeField]
    private EnnemyMovement enemyMovement;

    public float stunDuration = 0.15f;

    // VFX prefab for hit effect
    [SerializeField]
    private GameObject hitVFX;
    [SerializeField]
    private GameObject Pow;

    // Reference to the enemy's head transform (assign this in the inspector)
    [SerializeField]
    private Transform headTransform;

    [SerializeField]
    private KillProgressBar killProgressBar;

    protected EnemySpawner spawner;
    protected GameManager manager;

    public bool hasDied = false; // Flag to ensure Die method is called once.

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        killProgressBar = GameObject.FindGameObjectWithTag("KillBar").GetComponent<KillProgressBar>();  
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        hasDied = false; // Reset hasDied flag when enabled
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
        if (hasDied) return; // Prevent taking damage after death

        currentHealth -= amount;
        Debug.Log(gameObject.name + " took damage: " + amount);

        if (maxHealth > 100)
        {
            SoundManager.PlaySound(SoundType.Rock, null, 0.6f);
        }
        else
        {
            SoundManager.PlaySound(SoundType.Hit, null, 1f);
        }

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

    protected virtual void Die()
    {
        if (hasDied) return; // Ensure Die is called only once
        hasDied = true;

        Debug.Log(gameObject.name + " died.");
        animator.SetTrigger(DieTrigger);
        killProgressBar.AddKill();
        StartCoroutine(DieAfterAnimation());
       
    }
  
    private IEnumerator DieAfterAnimation()
    {
        int scoreadd;
      

        if (maxHealth > 100)
        {
            scoreadd = Random.Range(40, 75);
            scoreadd = Mathf.RoundToInt(scoreadd); // Apply multiplier
            score.text = scoreadd + "P";
            score.gameObject.GetComponent<Animator>().SetTrigger("Score2");
            manager.updateScore(scoreadd);
        }
        else
        {
            scoreadd = Random.Range(25, 50);
            scoreadd = Mathf.RoundToInt(scoreadd); // Apply multiplier
            score.text = scoreadd + "P";
            score.gameObject.GetComponent<Animator>().SetTrigger("Score");
            manager.updateScore(scoreadd);
        }


        yield return new WaitForSeconds(1f);
        spawner.killChaser++;   
        this.gameObject.GetComponent<Ennemy>().OnDisable();
    }

    // Function to spawn the VFX on hit
    private void SpawnHitVFX()
    {
        if (hitVFX != null && headTransform != null)
        {
            if (maxHealth > 100)
            {
                Instantiate(Pow, headTransform.position + new Vector3(0, 4f, 2.5f), Quaternion.identity);
            }
            else
            {
                Instantiate(hitVFX, headTransform.position, Quaternion.identity);
                Instantiate(Pow, headTransform.position + new Vector3(0, 1.5f, 1f), Quaternion.identity);
            }
        }
    }
}
