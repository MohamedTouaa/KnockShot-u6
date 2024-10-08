using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public List<Transform> Playerposition;
    public Transform targetPosition;
    public int NumberOfEnemiesToSpawn = 5;
    public float SpawnDelay = 1f;
    public List<Ennemy> EnemyPrefabs = new List<Ennemy>();
    public List<Transform> SpawnPositions = new List<Transform>();
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;

    private NavMeshTriangulation Triangulation;
    private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();

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
        StartCoroutine(SpawnEnemies());


       
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);

        int SpawnedEnemies = 0;

        while (SpawnedEnemies < NumberOfEnemiesToSpawn)
        {
            if (EnemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(SpawnedEnemies);
            }
            else if (EnemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }
            else if( EnemySpawnMethod == SpawnMethod.SpawnAtPosition)
            {
                SpawnEnemyAtChosenPosition();
            }

            SpawnedEnemies++;

            yield return Wait;
        }
    }
    private IEnumerator ChanegTarget()
    {
        while (true)
        {
            targetPosition = Playerposition[Random.Range(0, Playerposition.Count)];

            yield return new WaitForSeconds(0.9f);
        }
          
      
    }
    private void SpawnRoundRobinEnemy(int SpawnedEnemies)
    {
        int SpawnIndex = SpawnedEnemies % EnemyPrefabs.Count;

        DoSpawnEnemy(SpawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, EnemyPrefabs.Count));
    }

    private void SpawnEnemyAtChosenPosition()
    {
        if (SpawnPositions.Count == 0)
        {
            Debug.LogError("No spawn positions defined.");
            return;
        }

        // Pick a random spawn position or use a specific one
        Transform chosenSpawnPoint = SpawnPositions[Random.Range(0, SpawnPositions.Count)];
        DoSpawnEnemyAtPosition(chosenSpawnPoint);
    }

    private void DoSpawnEnemy(int SpawnIndex)
    {
        PoolableObject poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

        if (poolableObject != null)
        {
            Ennemy enemy = poolableObject.GetComponent<Ennemy>();

            int VertexIndex = Random.Range(0, Triangulation.vertices.Length);

            NavMeshHit Hit;
            if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f, -1))
            {
                enemy.meshAgent.Warp(Hit.position);
                // enemy needs to get enabled and start chasing now.
                enemy.ennemyMovement.targetPosition = targetPosition;
                enemy.meshAgent.enabled = true;
                enemy.ennemyMovement.StartChasing();
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {Triangulation.vertices[VertexIndex]}");
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy of type {SpawnIndex} from object pool. Out of objects?");
        }
    }

    private void DoSpawnEnemyAtPosition(Transform spawnPosition)
    {
        int SpawnIndex = Random.Range(0, EnemyPrefabs.Count);
        PoolableObject poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

        if (poolableObject != null)
        {
            Ennemy enemy = poolableObject.GetComponent<Ennemy>();

            NavMeshHit Hit;
            if (NavMesh.SamplePosition(spawnPosition.position, out Hit, 2f, -1))
            {
                enemy.meshAgent.Warp(Hit.position);
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
       
    }
}