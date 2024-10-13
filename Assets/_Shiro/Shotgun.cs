using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float distance = 15f;
    [SerializeField] private GameObject muzzle;
    [SerializeField] private GameObject muzzleTarget;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private float damageAmount = 20f; // Normal damage
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private float BulletSpeed = 100f;

    [SerializeField] private float oneShotDamage = 100f; // Damage value when the power-up is active
    [SerializeField] private float powerUpDuration = 5f; // Duration of the one-shot power-up

    private bool isOneShotActive = false;
    private float nextFireTime = 0f;
    private float powerUpEndTime = 0f; // Tracks when the power-up should wear off
    private Animator animator;
    private Camera cam;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cam = Camera.main;

    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            animator.SetTrigger("Shoot");
            Shoot();
            nextFireTime = Time.time + cooldownTime;
        }

        // Check if the power-up duration has ended
        if (isOneShotActive && Time.time >= powerUpEndTime)
        {
            DeactivateOneShotPowerUp();
        }
    }

    private void Shoot()
    {
        float currentDamage = isOneShotActive ? oneShotDamage : damageAmount; // Use the boosted damage if power-up is active

        Debug.Log("Current Damage: " + currentDamage + ", One-shot Active: " + isOneShotActive);

        RaycastHit hit;
        GameObject muzzleInstance = Instantiate(muzzle, spawnPoint.position, spawnPoint.localRotation);
        muzzleInstance.transform.parent = spawnPoint;

        Vector3[] hitPoints = new Vector3[4];
        int mask = ~ignoreLayer.value;

        SoundManager.PlaySound(SoundType.Gun, null, 0.5f);

        // Fire rays
        for (int i = 0; i < 4; i++)
        {
            Vector3 direction = cam.transform.forward;
            if (i == 1) direction += new Vector3(-0.2f, 0f, 0f); // Left
            else if (i == 2) direction += new Vector3(0f, 0.1f, 0f); // Up
            else if (i == 3) direction += new Vector3(0f, -0.1f, 0f); // Down

            if (Physics.Raycast(cam.transform.position, direction, out hit, distance, mask))
            {
                hitPoints[i] = hit.point;
                Instantiate(muzzleTarget, hit.point, Quaternion.LookRotation(hit.normal));

                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(currentDamage);
                }
            }
            else
            {
                hitPoints[i] = cam.transform.position + direction * distance;
            }
        }

        // Create trails for hit points
        for (int i = 0; i < 4; i++)
        {
            TrailRenderer trail = Instantiate(trailRenderer, bulletSpawnPosition.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hitPoints[i]));
        }

        // Apply knockback to the player
        ApplyKnockback();
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endPosition)
    {
        Vector3 startPosition = trail.transform.position;
        float travelTime = Vector3.Distance(startPosition, endPosition) / BulletSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        trail.transform.position = endPosition;
        Destroy(trail.gameObject, trail.time); // Destroy after trail fades
    }

    [Header("Recoil")]  

    public PlayerMovement playerMovement;
    public float liftForce = 5f;
    private bool canApplyLift = true;
    public float liftCooldown = 0.5f;
    [SerializeField] private float maxHeight = 10f; // Maximum height limit for knockback

    public void ApplyKnockback()
    {
        if (playerRigidbody == null || playerMovement == null) return;

        Vector3 knockbackDirection = -cam.transform.forward;

        // Apply knockback force horizontally
        playerRigidbody.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);

        // Apply vertical lift only if the player is below the maxHeight
        if (playerRigidbody.position.y < maxHeight)
        {
            float heightAdjustment = Mathf.Clamp(maxHeight - playerRigidbody.position.y, 0, liftForce);
            playerRigidbody.AddForce(Vector3.up * heightAdjustment, ForceMode.Impulse);
        }

        // Additional grounded check and lift with cooldown if needed
        // if (playerMovement.grounded && canApplyLift)
        // {
        //     playerRigidbody.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        //     canApplyLift = false;
        //     StartCoroutine(ResetLift());
        // }
    }

    private IEnumerator ResetLift()
    {
        yield return new WaitForSeconds(liftCooldown); 
        canApplyLift = true; 
    }



    [Header("PowerUp")]
    [SerializeField] private GameObject VFX;
    
    public void ActivateOneShotPowerUp()
    {
        
        isOneShotActive = true;
        SoundManager.PlaySound(SoundType.SuperPower, null, 1f);
        powerUpEndTime = Time.time + powerUpDuration; // Set the time when the power-up will wear off
        VFX.SetActive(true);
        Debug.Log("One-shot power-up activated! It will last for " + powerUpDuration + " seconds.");
    }

    // Deactivate the one-shot power-up
    private void DeactivateOneShotPowerUp()
    {
        isOneShotActive = false;
        VFX.SetActive(false);
        Debug.Log("One-shot power-up deactivated.");
    }
}
