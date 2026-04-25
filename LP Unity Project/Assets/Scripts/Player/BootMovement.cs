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
    public float magnetRotationSpeed = 10f;
    public float magnetGravityStrength = 20f;
    public float magnetSnapAngleThreshold = 40f;
    public float magnetTransitionTime = 0.2f;
    public float dismountCooldown = 0.5f;

    private bool isMagnetActive = false;
    private bool isAttachedToMagnet = false;
    private float dismountTimer = 0f;
    private Vector3 currentGravityDir = Vector3.down;
    private Vector3 targetSurfaceNormal = Vector3.up;
    private Vector3 activeSurfaceNormal = Vector3.up;
    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    private Quaternion transitionStartRot;

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
        movement  = GetComponent<PlayerController>();
        rb        = GetComponent<Rigidbody>();

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
                isSteamActive     = false;
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
        if (newBoots != BootType.MagnetBoots)
            DisableMagnetBoots();

        currentBoots = newBoots;
        OnBootSwap?.Invoke(true);
    }

    public void ChangeToSteamBoots()      => ChangeBoots(BootType.RocketBoots);
    public void ChangeToDetectionBoots()  => ChangeBoots(BootType.DetectionBoots);
    public void ChangeToMagnetBoots()     => ChangeBoots(BootType.MagnetBoots);

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

        bool boostHeld = Input.GetKey(KeyCode.Space)      || Input.GetKey(KeyCode.Joystick1Button0);
        bool boostDown = Input.GetKeyDown(KeyCode.Space)   || Input.GetKeyDown(KeyCode.Joystick1Button0);
        bool boostUp   = Input.GetKeyUp(KeyCode.Space)     || Input.GetKeyUp(KeyCode.Joystick1Button0);

        if (boostDown) holdTimer = 0f;

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

        if (boostUp) holdTimer = 0f;

        bool dashPressed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Joystick1Button1);

        if (dashPressed && airTime > dashLockoutTime && !usedHorizontalDash && dashTimer <= 0f)
        {
            usedHorizontalDash = true;
            dashTimer          = dashCooldown;

            Vector3 boostDir = movement.transform.forward;
            rb.AddForce(boostDir * horizontalBoostForce, ForceMode.VelocityChange);
            OnBootEffect?.Invoke(BootType.RocketBoots);
        }
    }

    void HandleMagnetBootsUpdate()
    {
        if (!isMagnetActive)
        {
            isMagnetActive        = true;
            isAttachedToMagnet    = false;
            movement.magnetActive = false;
            activeSurfaceNormal   = Vector3.up;
            targetSurfaceNormal   = Vector3.up;
            currentGravityDir     = Vector3.down;
            rb.useGravity         = true;
            Physics.gravity       = new Vector3(0f, -9.81f, 0f);
        }

        bool onMagneticSurface = CheckMagnet();

        if (dismountTimer > 0f)
            dismountTimer -= Time.deltaTime;

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3)) && isAttachedToMagnet)
        {
            DetachFromMagnet();
            return;
        }

        if (!isAttachedToMagnet)
        {
            if (onMagneticSurface && dismountTimer <= 0f)
            {
                isAttachedToMagnet           = true;
                movement.magnetActive        = true;
                rb.useGravity                = false;
                currentGravityDir            = -targetSurfaceNormal;
                Physics.gravity              = currentGravityDir * magnetGravityStrength;
                activeSurfaceNormal          = targetSurfaceNormal;
                movement.magnetSurfaceNormal = activeSurfaceNormal;
            }
            return;
        }

        if (!onMagneticSurface)
        {
            bool touchingNonMagneticGround = Physics.CheckSphere(
                groundCheck.position, groundCheckRadius,
                ~MagneticLayer, QueryTriggerInteraction.Ignore);

            if (touchingNonMagneticGround && movement.IsGrounded)
            {
                DetachFromMagnet();
                return;
            }
        }

        if (!isTransitioning)
        {
            float angle = Vector3.Angle(activeSurfaceNormal, targetSurfaceNormal);
            if (onMagneticSurface && angle > magnetSnapAngleThreshold)
                StartSurfaceTransition(targetSurfaceNormal);
        }

        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, transitionTimer / magnetTransitionTime);

            transform.rotation = Quaternion.Slerp(transitionStartRot,
                movement.GetMagnetRotation(activeSurfaceNormal, magnetRotationSpeed), t);

            if (transitionTimer >= magnetTransitionTime)
            {
                isTransitioning              = false;
                transitionTimer              = 0f;
                activeSurfaceNormal          = targetSurfaceNormal;
                currentGravityDir            = -activeSurfaceNormal;
                Physics.gravity              = currentGravityDir * magnetGravityStrength;
                movement.magnetSurfaceNormal = activeSurfaceNormal;
            }
        }
        else
        {
            transform.rotation = movement.GetMagnetRotation(activeSurfaceNormal, magnetRotationSpeed);
        }
    }

    void DetachFromMagnet()
    {
        isAttachedToMagnet           = false;
        isTransitioning              = false;
        transitionTimer              = 0f;
        dismountTimer                = dismountCooldown;
        movement.magnetActive        = false;
        activeSurfaceNormal          = Vector3.up;
        targetSurfaceNormal          = Vector3.up;
        currentGravityDir            = Vector3.down;
        Physics.gravity              = new Vector3(0f, -9.81f, 0f);
        rb.useGravity                = true;
    }

    void HandleMagnetBootsFixed()
    {
        if (!isMagnetActive) return;

        float currentSpeed = movement.IsGrounded ? 5f : 3f;

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, activeSurfaceNormal).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(transform.right,   activeSurfaceNormal).normalized;

        Vector3 moveDir = (forward * movement.GetVerticalInput() +
                           right   * movement.GetHorizontalInput()).normalized;

        Vector3 targetVel    = moveDir * currentSpeed;
        Vector3 currentVel   = rb.velocity;
        Vector3 projectedVel = Vector3.ProjectOnPlane(currentVel, activeSurfaceNormal);
        Vector3 velChange    = targetVel - projectedVel;

        rb.AddForce(velChange, ForceMode.VelocityChange);

        rb.AddForce(currentGravityDir * magnetGravityStrength, ForceMode.Acceleration);
    }

    bool CheckMagnet()
    {
        Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, MagneticLayer);

        if (hits.Length > 0)
        {
            Vector3 bestNormal = activeSurfaceNormal;
            float   bestDot    = -1f;

            foreach (Collider col in hits)
            {
                Vector3 toSurface = (groundCheck.position - col.ClosestPoint(groundCheck.position)).normalized;

                if (toSurface == Vector3.zero)
                    toSurface = activeSurfaceNormal;

                float dot = Vector3.Dot(toSurface, transform.up);

                if (dot > bestDot)
                {
                    bestDot    = dot;
                    bestNormal = toSurface;
                }
            }

            targetSurfaceNormal = SnapToRightAngle(bestNormal);
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 SnapToRightAngle(Vector3 normal)
    {
        Vector3[] axes = new Vector3[]
        {
            Vector3.up, Vector3.down,
            Vector3.right, Vector3.left,
            Vector3.forward, Vector3.back
        };

        Vector3 best    = normal;
        float   bestDot = -1f;

        foreach (Vector3 axis in axes)
        {
            float dot = Vector3.Dot(normal, axis);
            if (dot > bestDot)
            {
                bestDot = dot;
                best    = axis;
            }
        }

        return best;
    }

    void StartSurfaceTransition(Vector3 newNormal)
    {
        isTransitioning     = true;
        transitionTimer     = 0f;
        transitionStartRot  = transform.rotation;
        activeSurfaceNormal = newNormal;

        OnBootEffect?.Invoke(BootType.MagnetBoots);
    }

    void DisableMagnetBoots()
    {
        if (!isMagnetActive) return;
        DetachFromMagnet();
        isMagnetActive = false;
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
        currentCharge  = Mathf.Clamp(currentCharge, 0, maxCharge);

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
        float kb  = Input.GetAxis(keyboardAxis);
        float joy = 0f;
        try { joy = Input.GetAxis(joystickAxis); } catch { }
        return Mathf.Abs(kb) > Mathf.Abs(joy) ? kb : joy;
    }
}