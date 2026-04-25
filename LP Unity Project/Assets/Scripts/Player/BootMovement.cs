using System;
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

    public static event Action<BootType> OnBootEffect;
    public static event Action<bool> OnBootSwap;

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
    public float magnetRotationSpeed = 10f;
    public float gravityStrength = 9.81f;
    public float magnetMoveSpeed = 5f;
    private static readonly Vector3 WorldGravity = new Vector3(0f, -9.81f, 0f);

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
        
        if (chargeBarCanvas.gameObject.activeSelf)
            chargeBarCanvas.gameObject.SetActive(false);

        if (steamBootsCanvas.gameObject.activeSelf)
            steamBootsCanvas.gameObject.SetActive(false);
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
        OnBootSwap?.Invoke(true);

    }

    public void ChangeToSteamBoots()
    {
        currentBoots = BootType.RocketBoots;
        OnBootSwap?.Invoke(true);
    }

    public void ChangeToDetectionBoots()
    {
        currentBoots = BootType.DetectionBoots;
        OnBootSwap?.Invoke(true);
    }

    public void ChangeToMagnetBoots()
    {
        currentBoots = BootType.MagnetBoots;
        OnBootSwap?.Invoke(true);
    }
    
    void HandleRocketBoots()
    {
        if (movement == null || rb == null) return;

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
                OnBootEffect?.Invoke(BootType.RocketBoots);
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
        if (rb == null) return;

        const float sphereCastRadius = 0.5f;
        const float sphereCastDistance = 2f;


        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            sphereCastRadius,
            -transform.up,
            sphereCastDistance,
            MagneticLayer);

        bool onMagnetic = false;
        RaycastHit bestHit = default;
        float bestDot = -1f;

        foreach (RaycastHit hit in hits)
        {
            
            Transform hitTransform = hit.collider.transform;
            Vector3 thinAxis = hitTransform.up;

            
            float dot = Mathf.Abs(Vector3.Dot(hit.normal, thinAxis));
            if (dot > 0.7f && dot > bestDot)
            {
                bestDot = dot;
                bestHit = hit;
                onMagnetic = true;
            }
        }

        movement.magnetActive = onMagnetic;

        if (onMagnetic)
        {
            Vector3 normal = bestHit.normal;
            movement.magnetSurfaceNormal = normal;

            Physics.gravity = -normal * gravityStrength;
            rb.AddForce(-normal * gravityStrength, ForceMode.Acceleration);

            
            transform.rotation = movement.GetMagnetRotation(normal, magnetRotationSpeed);

            float inputH = GetAxis("Horizontal", "LeftStickX");
            float inputV = GetAxis("Vertical", "LeftStickY");

            Vector3 camForward = movement.cameraTarget.forward;
            Vector3 surfaceForward = Vector3.ProjectOnPlane(camForward, normal).normalized;
            Vector3 surfaceRight = Vector3.Cross(normal, surfaceForward);

            Vector3 moveDir = surfaceForward * inputV + surfaceRight * inputH;
            rb.MovePosition(rb.position + moveDir * magnetMoveSpeed * Time.fixedDeltaTime);

            if (magnetJumpQueued)
            {
                rb.AddForce(normal * verticalBoostForce, ForceMode.Impulse);
                Physics.gravity = new Vector3(0f, -9.81f, 0f);
            }
        }
        else
        {
            Physics.gravity = new Vector3(0f, -9.81f, 0f);

            Quaternion upright = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                upright,
                Time.fixedDeltaTime * magnetRotationSpeed * 0.5f);
        }

        magnetJumpQueued = false;
    }

    void HandleDetectionBoots()
    {
        if (!isDetectionActive)
        {
            chargeBarCanvas.gameObject.SetActive(true);
            chargeBar.SetMaxCharge(maxCharge);
            chargeBar.SetCurrentCharge(maxCharge);
            currentCharge = maxCharge;
            chargeBar.BarOffCooldown();
            StartCoroutine(CooldownCheck());
            isDetectionActive = true;
        }
        
        if (!onCooldown && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3)))
        {
            onCooldown = true;
            SetCharge(-chargeLost);
            CheckForTraces();
            // playerSFX.PlayDetectionBootsSound();
            OnBootEffect?.Invoke(BootType.DetectionBoots);
        }
    }
    
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
    
    private void SetCharge(float newCharge)
    {
        currentCharge += newCharge;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);

        if (currentCharge == 0)
            SceneManager.LoadScene("LoseScene");

        chargeBar.SetCurrentCharge(currentCharge);
    }

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

    float GetAxis(string keyboardAxis, string joystickAxis)
    {
        float kb = Input.GetAxis(keyboardAxis);
        float joy = 0f;
        try { joy = Input.GetAxis(joystickAxis); } catch { }
        return Mathf.Abs(kb) > Mathf.Abs(joy) ? kb : joy;
    }

    void CheckMagnet()
    {
        isMagnetActive = Physics.CheckSphere(groundCheck.position, groundCheckRadius, MagneticLayer);
    }
}