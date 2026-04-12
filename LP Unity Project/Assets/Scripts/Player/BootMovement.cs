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


    private bool magnetJumpQueued;
    private bool magnetIsActive;
    private Vector3 magnetSurfaceNormal;

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

        switch (currentBoots)
        {
            case BootType.RocketBoots:
                HandleRocketBoots();
                break;
            case BootType.MagnetBoots:
                HandleMagnetBootsUpdate();
                break;
            case BootType.SteamBoots:
                HandleSteamBoots();
                break;
            default:
                Debug.LogWarning("No boots equipped or unrecognized boot type.");
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentBoots == BootType.MagnetBoots)
            HandleMagnetBootsFixed();
    }


    void HandleRocketBoots()
    {
        if (movement == null || rb == null) return;

        if (movement.IsGrounded)
        {
            usedHorizontalDash = false;
            usedVerticalBoost  = false;
            holdTimer          = 0f;
            airTime            = 0f;
            return;
        }

        airTime += Time.deltaTime;

    
        bool boostHeld = Input.GetKey(KeyCode.Space)
                      || Input.GetKey(KeyCode.Joystick1Button0);

        bool boostDown = Input.GetKeyDown(KeyCode.Space)
                      || Input.GetKeyDown(KeyCode.Joystick1Button0);

        bool boostUp   = Input.GetKeyUp(KeyCode.Space)
                      || Input.GetKeyUp(KeyCode.Joystick1Button0);

        if (boostDown)
            holdTimer = 0f;

        if (boostHeld)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer > holdThreshold && !usedVerticalBoost)
            {
                if (rb.velocity.y < maxVerticalSpeed)
                    rb.AddForce(Vector3.up * verticalBoostForce, ForceMode.Acceleration);

                usedVerticalBoost = true;
            }
        }

        if (boostUp)
            holdTimer = 0f;

   
        bool dashPressed = Input.GetKeyDown(KeyCode.LeftShift)
                        || Input.GetKeyDown(KeyCode.Joystick1Button1); 

        if (dashPressed &&
            airTime > dashLockoutTime &&
            !usedHorizontalDash &&
            dashTimer <= 0f)
        {
            usedHorizontalDash = true;
            dashTimer          = dashCooldown;

            Vector3 boostDir = movement.transform.forward;
            rb.AddForce(boostDir * horizontalBoostForce, ForceMode.VelocityChange);
        }
    }


    void HandleMagnetBootsUpdate()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            magnetJumpQueued = true;
    }

    void HandleMagnetBootsFixed()
    {
        RaycastHit hit;
        float sphereCastRadius  = 0.5f;
        float sphereCastDistance = 2f;

        rb.freezeRotation = true;
        rb.useGravity     = true;

        bool onMagnetic = Physics.SphereCast(
            transform.position, sphereCastRadius,
            -transform.up, out hit,
            sphereCastDistance, MagneticLayer);

        magnetIsActive = onMagnetic;

        if (onMagnetic)
        {
            magnetSurfaceNormal = hit.normal;

           
            rb.AddForce(-magnetSurfaceNormal * 9.81f, ForceMode.Acceleration);

           
            float rotationSpeed     = 10f;
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, magnetSurfaceNormal) * transform.rotation;
            transform.rotation      = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);

           
            float keyH     = Input.GetAxis("Horizontal");
            float stickH   = 0f;
            try { stickH   = Input.GetAxis("LeftStickX"); } catch { }
            float moveInput = Mathf.Abs(keyH) > Mathf.Abs(stickH) ? keyH : stickH;

            float keyV     = Input.GetAxis("Vertical");
            float stickV   = 0f;
            try { stickV   = Input.GetAxis("LeftStickY"); } catch { }
            float forwardInput = Mathf.Abs(keyV) > Mathf.Abs(stickV) ? keyV : stickV;

            float moveSpeed = 5f;
            Vector3 moveDirection = transform.right * moveInput + transform.forward * forwardInput;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            
            if (magnetJumpQueued)
                rb.AddForce(magnetSurfaceNormal * verticalBoostForce, ForceMode.Acceleration);
        }
        else
        {
            
            Quaternion uprightRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, uprightRotation, Time.fixedDeltaTime * 5f);
        }

        magnetJumpQueued = false;
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