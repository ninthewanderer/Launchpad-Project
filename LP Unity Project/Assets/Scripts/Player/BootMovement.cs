using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootMovement : MonoBehaviour
{
    public enum BootType
    {
        None,
        MagnetBoots,
        SteamBoots,
        RocketBoots
    }

    public float horizontalBoostForce = 20f;
    public float verticalBoostForce = 30f;
    public float maxVerticalSpeed = 12f;

    public float dashCooldown = 0.5f;
    public float holdThreshold = 0.15f;
    public float dashLockoutTime = 0.15f;
    public LayerMask MagneticLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    private float dashTimer;
    private float holdTimer;
    private float airTime;

    private bool usedHorizontalDash;
    private bool usedVerticalBoost;
    private bool isMagnetActive;
 

    public BootType currentBoots = BootType.None;
    public PlayerController movement;

    private Rigidbody rb;

    void Start()
    {
        movement = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleCooldowns();

        if (currentBoots == BootType.RocketBoots)
        {
            HandleRocketBoots();
        
        }
        else if (currentBoots == BootType.MagnetBoots)
        {
            HandleMagnetBoots();
        }
        else if (currentBoots == BootType.SteamBoots)
        {
            HandleSteamBoots();
        }
        else
        {
            Debug.LogWarning("No boots equipped or unrecognized boot type.");
        }
    }

void HandleRocketBoots()
{
    if (movement == null || rb == null) return;

    if (movement.IsGrounded)
    {
        usedHorizontalDash = false;
        usedVerticalBoost = false;
        holdTimer = 0f;
        airTime = 0f;
        return;
    }
    else
    {
        airTime += Time.deltaTime;
    }


    if (Input.GetKeyDown(KeyCode.Space))
    {
        holdTimer = 0f;
    }

    if (Input.GetKey(KeyCode.Space))
    {
        holdTimer += Time.deltaTime;

        if (holdTimer > holdThreshold && !usedVerticalBoost)
        {
            if (rb.velocity.y < maxVerticalSpeed)
            {
                rb.AddForce(Vector3.up * verticalBoostForce, ForceMode.Acceleration);
            }

            usedVerticalBoost = true;
        }
    }

    if (Input.GetKeyUp(KeyCode.Space))
    {
        holdTimer = 0f;
    }


    if (Input.GetKeyDown(KeyCode.LeftShift) &&
        airTime > dashLockoutTime &&
        !usedHorizontalDash &&
        dashTimer <= 0f)
    {
        usedHorizontalDash = true;
        dashTimer = dashCooldown;

        Vector3 boostDir = movement.transform.forward;
        rb.AddForce(boostDir * horizontalBoostForce, ForceMode.VelocityChange);
    }
}

    void HandleMagnetBoots()
    {
        //Still in progress
        RaycastHit hit;
        float sphereCastRadius = 0.5f;
        float sphereCastDistance = 2f;
        rb.freezeRotation = true;
        rb.useGravity = true;

        bool magnet = Physics.SphereCast(transform.position, sphereCastRadius, -transform.up, out hit, sphereCastDistance, MagneticLayer);

        if (magnet)
        {
            Vector3 surfaceNormal = hit.normal;

            float gravityForce = 9.81f;
            rb.AddForce(-surfaceNormal * gravityForce, ForceMode.Acceleration);

            float rotationSpeed = 10f;
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            float moveInput = Input.GetAxis("Horizontal");
            float moveSpeed = 5f;

            Vector3 moveDirection = transform.right * moveInput;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(surfaceNormal * verticalBoostForce, ForceMode.Acceleration);
            }
        }
        else
        {
            Quaternion uprightRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, uprightRotation, Time.deltaTime * 5f);
        }
    }

    void HandleSteamBoots()
    {
        // Placeholder for steam boots logic
    }


    void HandleCooldowns()
    {
        if (dashTimer > 0f)
            dashTimer -= Time.deltaTime;
    }
    void CheckMagnet()
    {
        isMagnetActive = Physics.CheckSphere(groundCheck.position, groundCheckRadius, MagneticLayer);
    }
}