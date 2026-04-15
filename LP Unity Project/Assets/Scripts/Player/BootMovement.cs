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
    public float verticalBoostForce   = 30f;
    public float maxVerticalSpeed     = 12f;

    public float dashCooldown   = 0.5f;
    public float holdThreshold  = 0.15f;
    public float dashLockoutTime = 0.15f;

    public LayerMask MagneticLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;


    public float magnetRotationSpeed = 10f;

    public float gravityStrength = 9.81f;

    public float magnetMoveSpeed = 5f;

    private float dashTimer;
    private float holdTimer;
    private float airTime;

    private bool usedHorizontalDash;
    private bool usedVerticalBoost;

    private bool magnetJumpQueued;
    private bool magnetIsActive;
    private Vector3 magnetSurfaceNormal;

    public BootType currentBoots = BootType.None;
    public PlayerController movement;

    private Rigidbody rb;


    private static readonly Vector3 WorldGravity = new Vector3(0f, -9.81f, 0f);

    void Start()
    {
        movement = GetComponent<PlayerController>();
        rb       = GetComponent<Rigidbody>();
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

    // ─── Rocket Boots ────────────────────────────────────────────────────────

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

        if (boostDown) holdTimer = 0f;

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

        if (boostUp) holdTimer = 0f;

        bool dashPressed = Input.GetKeyDown(KeyCode.LeftShift)
                        || Input.GetKeyDown(KeyCode.Joystick1Button1);

        if (dashPressed && airTime > dashLockoutTime && !usedHorizontalDash && dashTimer <= 0f)
        {
            usedHorizontalDash = true;
            dashTimer          = dashCooldown;
            rb.AddForce(movement.transform.forward * horizontalBoostForce, ForceMode.VelocityChange);
        }
    }

    // ─── Magnet Boots ─────────────────────────────────────────────────────────

    void HandleMagnetBootsUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            magnetJumpQueued = true;
    }

    void HandleMagnetBootsFixed()
    {
        if (rb == null) return;

        rb.freezeRotation = true;

        const float sphereCastRadius   = 0.5f;
        const float sphereCastDistance = 2f;
        

        bool onMagnetic = Physics.SphereCast(
            transform.position, sphereCastRadius,
            -transform.up, out RaycastHit hit,
            sphereCastDistance, MagneticLayer);

        magnetIsActive = onMagnetic;
        movement.magnetActive         = onMagnetic;
        movement.magnetSurfaceNormal  = onMagnetic ? hit.normal : Vector3.up;

        if (onMagnetic)
        {
            magnetSurfaceNormal = hit.normal;


            Physics.gravity = -magnetSurfaceNormal * gravityStrength;


            rb.AddForce(-magnetSurfaceNormal * gravityStrength, ForceMode.Acceleration);


            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, magnetSurfaceNormal)
                                      * transform.rotation;
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRotation,
                Time.fixedDeltaTime * magnetRotationSpeed);


            float inputH = GetAxis("Horizontal", "LeftStickX");
            float inputV = GetAxis("Vertical",   "LeftStickY");


            Vector3 surfaceRight   = Vector3.ProjectOnPlane(transform.right,   magnetSurfaceNormal).normalized;
            Vector3 surfaceForward = Vector3.ProjectOnPlane(transform.forward, magnetSurfaceNormal).normalized;

            Vector3 moveDir = surfaceRight * inputH + surfaceForward * inputV;
            rb.MovePosition(rb.position + moveDir * magnetMoveSpeed * Time.fixedDeltaTime);


            if (magnetJumpQueued)
            {
                rb.AddForce(magnetSurfaceNormal * verticalBoostForce, ForceMode.Impulse);

                Physics.gravity = WorldGravity;
            }
        }
        else
        {

            Physics.gravity = WorldGravity;

            Quaternion uprightRotation = Quaternion.FromToRotation(transform.up, Vector3.up)
                                        * transform.rotation;
            transform.rotation = Quaternion.Slerp(
                transform.rotation, uprightRotation,
                Time.fixedDeltaTime * magnetRotationSpeed * 0.5f);
        }

        magnetJumpQueued = false;
    }

    // ─── Steam Boots ──────────────────────────────────────────────────────────

    void HandleSteamBoots()
    {
        // Placeholder
    }



    void HandleCooldowns()
    {
        if (dashTimer > 0f) dashTimer -= Time.deltaTime;
    }


    float GetAxis(string keyboardAxis, string joystickAxis)
    {
        float kb = Input.GetAxis(keyboardAxis);
        float joy = 0f;
        try { joy = Input.GetAxis(joystickAxis); } catch { }
        return Mathf.Abs(kb) > Mathf.Abs(joy) ? kb : joy;
    }
}