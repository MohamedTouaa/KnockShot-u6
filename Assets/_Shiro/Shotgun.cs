using SmallHedge.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float distance = 15f;
    [SerializeField] private GameObject muzzle;
    [SerializeField] private GameObject muzzleTarget;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float cooldownTime = 1f; // Cooldown time in seconds
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private LayerMask ignoreLayer; // LayerMask for the objects you want to hit

    [SerializeField]
    private TrailRenderer trailRenderer;

    private Animator animator;
    [SerializeField]
    private Transform bulletSpawnPosition;
    [SerializeField]
    private float BulletSpeed = 100;

    private float nextFireTime = 0f; // Tracks when the player can fire next
    Camera cam;
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            animator.SetTrigger("Shoot");
            Shoot();
            nextFireTime = Time.time + cooldownTime;
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        RaycastHit hit_1;
        RaycastHit hit_2;
        RaycastHit hit_3;
        GameObject muzzleInstance = Instantiate(muzzle, spawnPoint.position, spawnPoint.localRotation);
        muzzleInstance.transform.parent = spawnPoint;

        Vector3[] hitPoints = new Vector3[4]; // Array to store hit points for all 4 rays

        int mask = ~ignoreLayer.value;

        SoundManager.PlaySound(SoundType.Gun, null, 0.5f);

        // Forward
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance, mask))
        {
            hitPoints[0] = hit.point;

            Instantiate(muzzleTarget, hit.point, Quaternion.LookRotation(hit.normal));

            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount); // Apply damage
            }
        }
        else
        {
            hitPoints[0] = cam.transform.position + cam.transform.forward * distance;
        }

        // Left
        if (Physics.Raycast(cam.transform.position, cam.transform.forward + new Vector3(-0.2f, 0f, 0f), out hit_1, distance, mask))
        {
            hitPoints[1] = hit_1.point;

            Instantiate(muzzleTarget, hit_1.point, Quaternion.LookRotation(hit_1.normal));

            EnemyHealth enemyHealth = hit_1.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount); // Apply damage
            }
        }
        else
        {
            hitPoints[1] = cam.transform.position + (cam.transform.forward + new Vector3(-0.2f, 0f, 0f)) * distance;
        }

        // Up
        if (Physics.Raycast(cam.transform.position, cam.transform.forward + new Vector3(0f, 0.1f, 0f), out hit_2, distance, mask))
        {
            hitPoints[2] = hit_2.point;

            Instantiate(muzzleTarget, hit_2.point, Quaternion.LookRotation(hit_2.normal));

            EnemyHealth enemyHealth = hit_2.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount); // Apply damage
            }
        }
        else
        {
            hitPoints[2] = cam.transform.position + (cam.transform.forward + new Vector3(0f, 0.1f, 0f)) * distance;
        }

        // Down
        if (Physics.Raycast(cam.transform.position, cam.transform.forward + new Vector3(0f, -0.1f, 0f), out hit_3, distance, mask))
        {
            hitPoints[3] = hit_3.point;

            Instantiate(muzzleTarget, hit_3.point, Quaternion.LookRotation(hit_3.normal));

            EnemyHealth enemyHealth = hit_3.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount); // Apply damage
            }
        }
        else
        {
            hitPoints[3] = cam.transform.position + (cam.transform.forward + new Vector3(0f, -0.1f, 0f)) * distance;
        }

        // Create trails for all 4 hit points
        for (int i = 0; i < 4; i++)
        {
            TrailRenderer trail = Instantiate(trailRenderer, bulletSpawnPosition.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hitPoints[i]));
        }

        // Apply knockback to the player
        ApplyKnockback();
        Debug.Log("Knocked.");
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endPosition)
    {
        // Get the start position (bullet spawn position)
        Vector3 startPosition = trail.transform.position;

        // Calculate the travel duration based on bullet speed
        float travelTime = Vector3.Distance(startPosition, endPosition) / BulletSpeed;
        float elapsedTime = 0f;

        // Move the trail along the path
        while (elapsedTime < travelTime)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the trail reaches the end position
        trail.transform.position = endPosition;

        // Optional: Disable the trail after it reaches the target
        Destroy(trail.gameObject, trail.time);  // Destroy after the trail finishes fading
    }

    private void ApplyKnockback()
    {
        Vector3 knockbackDirection = -cam.transform.forward;
        playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }
}
