using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    private bool isWalking = false;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("HeadBob")]
    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // FOV Settings
    [Header("FOV Settings")]
    public Camera playerCamera;
    public float baseFOV = 60f;
    public float maxFOV = 80f;
    public float fovIncreaseSpeed = 10f;

    // Speedlines effect
    [Header("Speedlines Effect")]
    public GameObject speedlinesEffect;
    public float speedThreshold = 10f; // Speed above which the effect is enabled

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;
    private float currentFOV;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        jointOriginalPos = joint.localPosition; // Store the original position of the head joint

        currentFOV = baseFOV;
        playerCamera.fieldOfView = baseFOV; // Set initial FOV

        // Ensure speedlines are disabled initially
        if (speedlinesEffect != null)
        {
            speedlinesEffect.SetActive(false);
        }
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle friction
        if (grounded)
            rb.linearDamping = groundDrag; // Set drag instead of linearDamping
        else
            rb.linearDamping = 0;

        if (enableHeadBob)
        {
            HeadBob();
        }

        AdjustFOVBasedOnSpeed(); // Adjust FOV based on player's speed
        ManageSpeedlinesEffect(); // Enable/disable speedlines based on speed
    }

    private void FixedUpdate()
    {
        MovePlayer();

        #region IsWalking Check
        if (rb.linearVelocity.x != 0 || rb.linearVelocity.z != 0 && grounded) // Use 'velocity' instead of 'linearVelocity'
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
        #endregion
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Use 'velocity' instead of 'linearVelocity'

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z); // Use 'velocity' instead of 'linearVelocity'
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Use 'velocity' instead of 'linearVelocity'

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void HeadBob()
    {
        if (isWalking)
        {
            timer += Time.deltaTime * bobSpeed;
            // Applies HeadBob movement
            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when player stops moving
            timer = 0;
            joint.localPosition = new Vector3(
                Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed),
                Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed),
                Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }

    private void AdjustFOVBasedOnSpeed()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude; // Get the current speed

        // Map the speed to an FOV value between baseFOV and maxFOV
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speed / moveSpeed);

        // Smoothly transition between the current FOV and the target FOV
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovIncreaseSpeed);

        // Apply the calculated FOV
        playerCamera.fieldOfView = currentFOV;
    }

    private void ManageSpeedlinesEffect()
    {
        // Calculate the flat speed
        float speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;

        // Enable or disable the speedlines effect based on speed
        if (speed > speedThreshold)
        {
            if (speedlinesEffect != null && !speedlinesEffect.activeInHierarchy)
            {
                speedlinesEffect.SetActive(true); // Enable speedlines effect
            }
        }
        else
        {
            if (speedlinesEffect != null && speedlinesEffect.activeInHierarchy)
            {
                speedlinesEffect.SetActive(false); // Disable speedlines effect
            }
        }
    }
}
