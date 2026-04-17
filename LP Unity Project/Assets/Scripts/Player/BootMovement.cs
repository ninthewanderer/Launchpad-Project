using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootMovement : MonoBehaviour
{
    private PlayerController movement;
    private Rigidbody rb;
    private PlayerSounds playerSFX;
    
    public BootType currentBoots = BootType.None;
    public enum BootType
    {
        None,
        MagnetBoots,
        DetectionBoots,
        RocketBoots
    }

    [Header("------------- Steam Boots -------------")]
    public CanvasGroup steamBootsCanvas;
    private bool isSteamActive;
    
    public float horizontalBoostForce = 20f;
    public float verticalBoostForce = 30f;
    public float maxVerticalSpeed = 12f;

    public float dashCooldown = 0.5f;
    public float holdThreshold = 0.15f;
    public float dashLockoutTime = 0.15f;
    
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    private float dashTimer;
    private float holdTimer;
    private float airTime;
    private bool usedHorizontalDash;
    private bool usedVerticalBoost;
    
    [Header("------------- Magnetic Boots -------------")]
    public LayerMask MagneticLayer;
    private bool isMagnetActive;
    private bool magnetJumpQueued;
    private bool magnetIsActive;
    private Vector3 magnetSurfaceNormal;

    [Header("------------- Detection Boots -------------")]
    public LayerMask traceLayer;
    public DetectionBootsUI chargeBar;
    public CanvasGroup chargeBarCanvas;
    
    private bool isDetectionActive;
    public float detectionRadius;
    public float disappearTime;
    public float abilityCooldown;
    private bool onCooldown = false;
    
    public float maxCharge;
    private float currentCharge;
    public float chargeLost;

    void Start()
    {
        playerSFX = GetComponent<PlayerSounds>();
        movement = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        
        // Only disables the canvases if they're not already disabled.
        if (chargeBarCanvas.gameObject.activeSelf)
        {
            chargeBarCanvas.gameObject.SetActive(false);
        }

        if (steamBootsCanvas.gameObject.activeSelf)
        {
            steamBootsCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleCooldowns();

        switch (currentBoots)
        {
            case BootType.None:
                break;
            case BootType.RocketBoots:
                isDetectionActive = false;
                chargeBarCanvas.gameObject.SetActive(false);
                HandleRocketBoots();
                break;
            case BootType.MagnetBoots:
                isDetectionActive = false;
                isSteamActive = false;
                chargeBarCanvas.gameObject.SetActive(false);
                steamBootsCanvas.gameObject.SetActive(false);
                HandleMagnetBootsUpdate();
                break;
            case BootType.DetectionBoots:
                isSteamActive = false;
                steamBootsCanvas.gameObject.SetActive(false);
                HandleDetectionBoots();
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

    public void ChangeBoots(BootType newBoots)
    {
        currentBoots = newBoots;
    }
    
    void HandleRocketBoots()
    {
        if (movement == null || rb == null) return;

        // Only happens one time so that the steam boots functionality is enabled.
        if (!isSteamActive)
        {
            steamBootsCanvas.gameObject.SetActive(true);
            isSteamActive = true;
        }

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
    
    void HandleDetectionBoots()
    {
        // One-time setup for the detection boots.
        if (!isDetectionActive)
        {
            chargeBarCanvas.gameObject.SetActive(true);
            chargeBar.SetMaxCharge(maxCharge);
            chargeBar.SetCurrentCharge(maxCharge);
            currentCharge = maxCharge;
            chargeBar.BarOffCooldown();
            StartCoroutine(CooldownCheck()); // Starts constant boot cooldown management.
            isDetectionActive = true;
        }
        
        // E on keyboard, Y on controller
        // This activates the detection boots ability.
        if (!onCooldown && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3)))
        {
            onCooldown = true;
            SetCharge(-chargeLost);
            CheckForTraces();
            playerSFX.PlayDetectionBootsSound();
        }
    }
    
    // Manages the ability cooldown for the boots.
    private IEnumerator CooldownCheck()
    {
        while (true)
        {
            if (onCooldown)
            {
                chargeBar.BarOnCooldown();
                yield return new WaitForSeconds(abilityCooldown);
                chargeBar.BarOffCooldown();
                onCooldown = false;
            }
            yield return null;
        }
    }
    
    // Manages the charge bar for the detection boots.
    private void SetCharge(float newCharge)
    {
        // Adds the newCharge value onto currentCharge, ensuring it never drops below 0 or goes above maxCharge.
        currentCharge += newCharge;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);

        // If the player runs out of charge, they lose.
        if (currentCharge == 0)
        {
            SceneManager.LoadScene("LoseScene");
        }
        chargeBar.SetCurrentCharge(currentCharge);
    }

    // Checks for traces of the cat near the player.(Detection boots ability.)
    private void CheckForTraces()
    {
        Collider[] traces = Physics.OverlapSphere(transform.position, detectionRadius, traceLayer,
            QueryTriggerInteraction.Collide);

        if (traces.Length != 0)
        {
            foreach (Collider trace in traces)
            {
                GameObject traceObj = trace.transform.gameObject;
                if (traceObj.CompareTag("Trace") && traceObj.activeSelf)
                {
                    int defaultLayer = LayerMask.NameToLayer("Default");
                    traceObj.layer = defaultLayer;
                    StartCoroutine(TraceDisappear(traceObj));
                }
            }
        }
    }

    // Makes the traces of the cat disappear after a specified amount of time.
    private IEnumerator TraceDisappear(GameObject traceObj)
    {
        yield return new WaitForSeconds(disappearTime);
        int traceLayerInt = LayerMask.NameToLayer("Cat_Traces");
        traceObj.layer = traceLayerInt;
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