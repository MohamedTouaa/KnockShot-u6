using SmallHedge.SoundManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

public class EnemySpawner : MonoBehaviour
{
    public List<Transform> Playerposition;
    public Transform targetPosition;
    public int NumberOfEnemiesToSpawn = 5;
    public int NeededEnnemies;
    public float SpawnDelay = 1f;
    public List<Ennemy> EnemyPrefabs = new List<Ennemy>();
    public List<Transform> SpawnPositions = new List<Transform>();
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;

    public int killChaser = 0;
    public int baseEnemiesPerWave = 5; // Base number of enemies per wave
    public int currentWave = 1; // Tracks the current wave number
    public int totalEnemiesInWave = 0;

    public float spawnWait = 1f;// Tracks the total enemies in the current wave

    private NavMeshTriangulation Triangulation;
    private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();

    public List<GameObject> pickupPrefabs = new List<GameObject>(); // List of different pickup object prefabs
    private bool isPickupCollected = false; // Flag to check if the object was collected
    public Transform pickupSpawnPosition;

    [SerializeField]
    private TextMeshProUGUI waveText;

   

    private void Awake()
    {
        for (int i = 0; i < EnemyPrefabs.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(EnemyPrefabs[i], NumberOfEnemiesToSpawn));
        }
    }

    private void Start()
    {
        Triangulation = NavMesh.CalculateTriangulation();

        StartCoroutine(ChanegTarget());
        StartCoroutine(SpawnEnemiesWave());
    }

    private IEnumerator SpawnEnemiesWave()
    {
        while (true) // Infinite loop to handle multiple waves
        {
            Debug.Log($"Wave {currentWave} starting.");

            // Define the number of each enemy type per wave.
            int zombiesToSpawn = Mathf.Max(baseEnemiesPerWave - (currentWave - 1) * 2, 0); // More zombies in higher waves.
            int bombersToSpawn = Mathf.Max((currentWave - 1) * 2, 0); // Introduce bombers in wave 2+.
            int golemsToSpawn = Mathf.Max((currentWave - 2) * 2, 0); // Introduce golems in wave 3+.

            totalEnemiesInWave = zombiesToSpawn + bombersToSpawn + golemsToSpawn;

            // Create a list of enemy types to spawn
            List<int> enemyTypesToSpawn = new List<int>();

            // Add the specified number of each enemy type to the list
            for (int i = 0; i < zombiesToSpawn; i++)
            {
                enemyTypesToSpawn.Add(0); // 0 for Zombie
            }
            for (int i = 0; i < bombersToSpawn; i++)
            {
                enemyTypesToSpawn.Add(1); // 1 for Bomber
            }
            for (int i = 0; i < golemsToSpawn; i++)
            {
                enemyTypesToSpawn.Add(2); // 2 for Golem
            }

            // Shuffle the enemy types list
            ShuffleList(enemyTypesToSpawn);

            // Spawn enemies in random order
            foreach (int enemyIndex in enemyTypesToSpawn)
            {
                SpawnEnemyAtChosenPosition(enemyIndex);
                yield return new WaitForSeconds(Random.Range(spawnWait, spawnWait + 1f)); // Delay before spawning the next enemy
            }

            // Wait until all enemies are killed before starting the next wave.
            yield return new WaitUntil(() => AreAllEnemiesInPool(totalEnemiesInWave, killChaser));

            Debug.Log($"Wave {currentWave} finished. All enemies defeated.");
           
            SpawnRandomPickupObject();
            FindObjectOfType<PickupObject>().DeactivateAllPowerUps();

            // Wait until the player collects the pickup object to proceed to the next wave
            yield return new WaitUntil(() => isPickupCollected);
            currentWave++; // Increment wave number.
            baseEnemiesPerWave += 5; // Increase base enemy count for the next wave.

            isPickupCollected = false;
        }
    }

    // Method to shuffle a list
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }

    private void SpawnRandomPickupObject()
    {
        if (pickupPrefabs.Count == 0 || pickupSpawnPosition == null)
        {
            Debug.LogError("No pickup objects or spawn position defined.");
            return;
        }

        // Pick a random pickup object from the list
        GameObject randomPickup = pickupPrefabs[Random.Range(0, pickupPrefabs.Count)];

        Instantiate(randomPickup, pickupSpawnPosition.position, Quaternion.identity);
        Debug.Log("Random pickup object spawned.");
    }

    private bool AreAllEnemiesInPool(int original, int remaining)
    {
        if (remaining == original)
        {
            killChaser = 0;
            return true;
        }
        else return false;
    }

    public void OnPickupCollected()
    {
        Debug.Log("Pickup Collected!");
        isPickupCollected = true;
        waveText.text = "WAVE " + (currentWave + 1) + " START";
        waveText.gameObject.GetComponent<Animator>().SetTrigger("Wave");
        SoundManager.PlaySound(SoundType.NewWave, null, 1f);
    }


    private IEnumerator ChanegTarget()
    {
        while (true)
        {
            targetPosition = Playerposition[Random.Range(0, Playerposition.Count)];

            yield return new WaitForSeconds(0.9f);
        }
    }

    private void SpawnEnemyAtChosenPosition(int enemyIndex)
    {
        if (SpawnPositions.Count == 0)
        {
            Debug.LogError("No spawn positions defined.");
            return;
        }

        // Pick a random spawn position or use a specific one
        Transform chosenSpawnPoint = SpawnPositions[Random.Range(0, SpawnPositions.Count)];
        DoSpawnEnemyAtPosition(chosenSpawnPoint, enemyIndex);
    }

    private void DoSpawnEnemyAtPosition(Transform spawnPosition, int enemyIndex)
    {
        if (enemyIndex < 0 || enemyIndex >= EnemyPrefabs.Count)
        {
            Debug.LogError("Invalid enemy index.");
            return;
        }

        PoolableObject poolableObject = EnemyObjectPools[enemyIndex].GetObject();

        if (poolableObject != null)
        {
            Ennemy enemy = poolableObject.GetComponent<Ennemy>();

            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition.position, out hit, 2f, -1))
            {
                enemy.meshAgent.Warp(hit.position);
                // Enable enemy and start chasing
                enemy.ennemyMovement.targetPosition = targetPosition;
                enemy.meshAgent.enabled = true;
                enemy.ennemyMovement.StartChasing();
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent at chosen spawn position {spawnPosition.position}");
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy from object pool at chosen spawn position.");
        }
    }

    public enum SpawnMethod
    {
        RoundRobin,
        Random,
        SpawnAtPosition,
        Wave,
    }
}
