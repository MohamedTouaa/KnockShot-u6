using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float distance = 15f;
    [SerializeField] private GameObject muzzle;
    [SerializeField] private GameObject muzzleTarget;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float knockbackForce = 5f; // Player knockback force
    [SerializeField] private float enemyKnockbackForce = 10f; // Enemy knockback force
    [SerializeField] private float cooldownTime = 1f;
    [SerializeField] private float damageAmount = 20f; // Normal damage
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private float BulletSpeed = 100f;

    [SerializeField] private float oneShotDamage = 300f; // Damage value when the power-up is active
    [SerializeField] private float powerUpDuration = 5f; // Duration of the one-shot power-up

    private bool isOneShotActive = false;
    private float nextFireTime = 0f;
    private float powerUpEndTime = 0f; // Tracks when the power-up should wear off
    private Animator animator;
    private Camera cam;


    // Grenade launcher fields
    [Header("Grenade Launcher")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float grenadeCooldownTime = 2f;
    [SerializeField] private float grenadeSpeed = 10f;
    public bool isGrenadeLauncherActive = false; // This will lock the grenade launcher until the power-up is active
    private float nextGrenadeFireTime = 0f;

    [Header("Enemy Knockback")]
    [SerializeField] private bool applyKnockbackToEnemies = true; // Toggle for enemy knockback

    [Header("Post-Processing")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float chromaticAberrationIntensity = 1f; // Intensity for chromatic aberration effect
    [SerializeField] private float chromaticLerpDuration = 0.5f;

    private ChromaticAberration chromaticAberration;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cam = Camera.main;

        if (globalVolume.profile.TryGet(out chromaticAberration))
        {
            Debug.Log("Vignette effect found.");
        }
        else
        {
            Debug.LogError("Vignette effect not found in Volume profile.");
        }

    }

    private void Update()
    {
        // Regular shotgun fire
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            animator.SetTrigger("Shoot");
            Shoot();
            nextFireTime = Time.time + cooldownTime;
        }

        // Grenade launcher fire, only available if the power-up is active
        if (Input.GetButtonDown("Fire2") && isGrenadeLauncherActive && Time.time >= nextGrenadeFireTime)
        {
            FireGrenade();
            nextGrenadeFireTime = Time.time + grenadeCooldownTime;
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

                    // Apply knockback to the enemy if the boolean is set
                    if (applyKnockbackToEnemies)
                    {
                        Rigidbody enemyRb = hit.collider.GetComponent<Rigidbody>();
                        if (enemyRb != null)
                        {
                            Vector3 knockbackDirection = (hit.transform.position - cam.transform.position).normalized; // Get knockback direction
                            enemyRb.AddForce(knockbackDirection * enemyKnockbackForce, ForceMode.Impulse); // Use enemy knockback force
                        }
                    }
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

    private void FireGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, bulletSpawnPosition.position, Quaternion.identity);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        Vector3 launchDirection = cam.transform.forward;
        grenadeRb.AddForce(launchDirection * grenadeSpeed, ForceMode.Impulse);

        SoundManager.PlaySound(SoundType.Gun, null, 0.5f); // Play sound for grenade launch
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

        StartCoroutine(TriggerChromaticAberration());


        Debug.Log("One-shot power-up activated! It will last for " + powerUpDuration + " seconds.");
    }

    private void DeactivateOneShotPowerUp()
    {
        isOneShotActive = false;
        VFX.SetActive(false);
        Debug.Log("One-shot power-up deactivated.");
    }

    private IEnumerator TriggerChromaticAberration()
    {
        float elapsedTime = 0f;
        float initialIntensity = chromaticAberration.intensity.value;

        // Fade in chromatic aberration
        while (elapsedTime < chromaticLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            chromaticAberration.intensity.Override(Mathf.Lerp(initialIntensity, chromaticAberrationIntensity, elapsedTime / chromaticLerpDuration));
            yield return null;
        }

        // Fade out after short delay
        yield return new WaitForSeconds(0.2f);

        elapsedTime = 0f;
        while (elapsedTime < chromaticLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            chromaticAberration.intensity.Override(Mathf.Lerp(chromaticAberrationIntensity, initialIntensity, elapsedTime / chromaticLerpDuration));
            yield return null;
        }
    }

    private IEnumerator TriggerChromaticAberrationForPowerUp()
    {
        chromaticAberration.intensity.Override(chromaticAberrationIntensity);

        yield return new WaitForSeconds(powerUpDuration);

        chromaticAberration.intensity.Override(0f);
    }
}
