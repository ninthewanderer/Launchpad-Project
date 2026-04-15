using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 9f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public float steamBoost = 15f;

    [Header("Camera/Look")]
    public Transform cameraTarget;
    public float cameraYOffset = 1.5f;
    public float mouseSensitivity = 200f;
    public float controllerSensitivity = 150f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float shoulderOffsetX = 0.5f;

    [Header("Controller")]
    public float stickDeadZone = 0.15f;

    private Rigidbody rb;
    private bool isGrounded;
    private float yaw;
    private float pitch;
    private float horizontalInput;
    private float verticalInput;
    private bool isSprinting;

    private Vector3 wallNormal = Vector3.zero;
    private bool isTouchingWall = false;

    // ── Magnet boot integration ──────────────────────────────────────────────
    [HideInInspector] public bool  magnetActive       = false;
    [HideInInspector] public Vector3 magnetSurfaceNormal = Vector3.up;

    public bool IsGrounded => isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.material = null;

        Cursor.lockState = CursorLockMode.Locked;
        yaw   = transform.eulerAngles.y;
        pitch = 0f;
    }

    void Update()
    {
        CheckGround();
        ReadInput();

        if (!magnetActive)
            HandleJump();
    }

    void FixedUpdate()
    {
        MovePlayer();
        if (isGrounded) CancelWallVelocity();
        RotatePlayer();
    }

    void LateUpdate()
    {
        UpdateCameraTarget();
    }

    // ── Input ─────────────────────────────────────────────────────────────────

    float ApplyDeadZone(float value)
    {
        if (Mathf.Abs(value) < stickDeadZone) return 0f;
        return Mathf.Sign(value) * (Mathf.Abs(value) - stickDeadZone) / (1f - stickDeadZone);
    }

    void ReadInput()
    {
        float keyboardH = Input.GetAxis("Horizontal");
        float keyboardV = Input.GetAxis("Vertical");

        float leftStickH = 0f;
        float leftStickV = 0f;
        try
        {
            leftStickH = ApplyDeadZone(Input.GetAxis("LeftStickX"));
            leftStickV = ApplyDeadZone(Input.GetAxis("LeftStickY"));
        }
        catch { }

        horizontalInput = Mathf.Abs(keyboardH) > Mathf.Abs(leftStickH) ? keyboardH : leftStickH;
        verticalInput   = Mathf.Abs(keyboardV) > Mathf.Abs(leftStickV) ? keyboardV : leftStickV;

        isSprinting = Input.GetKey(KeyCode.LeftControl)
                   || Input.GetKey(KeyCode.Joystick1Button8)
                   || Input.GetKey(KeyCode.Joystick1Button2);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        float stickX = 0f;
        float stickY = 0f;
        try
        {
            stickX = ApplyDeadZone(Input.GetAxis("RightStickX")) * controllerSensitivity * Time.deltaTime;
            stickY = ApplyDeadZone(Input.GetAxis("RightStickY")) * controllerSensitivity * Time.deltaTime;
        }
        catch { }

        yaw   += mouseX + stickX;
        pitch -= mouseY + stickY;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    // ── Movement ──────────────────────────────────────────────────────────────

    void MovePlayer()
    {

        if (magnetActive) return;

        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 moveDir = (transform.forward * verticalInput
                         + transform.right   * horizontalInput).normalized;

        if (isTouchingWall && moveDir != Vector3.zero)
        {
            float dot = Vector3.Dot(moveDir, wallNormal);
            if (dot < 0f)
            {
                moveDir -= wallNormal * dot;
                if (moveDir.sqrMagnitude < 0.001f) return;
                moveDir = moveDir.normalized;
            }
        }

        Vector3 targetVelocity = moveDir * currentSpeed;
        Vector3 velocityChange = targetVelocity - new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        velocityChange = Vector3.ClampMagnitude(velocityChange, currentSpeed);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void CancelWallVelocity()
    {
        if (!isTouchingWall) return;
        float penetrationSpeed = Vector3.Dot(rb.velocity, -wallNormal);
        if (penetrationSpeed > 0f)
            rb.velocity += wallNormal * penetrationSpeed;
    }

    // ── Rotation ──────────────────────────────────────────────────────────────

    void RotatePlayer()
    {
        if (magnetActive)
        {

            Quaternion surfaceYaw = Quaternion.AngleAxis(yaw, magnetSurfaceNormal);

            Vector3 surfaceForward = Vector3.ProjectOnPlane(
                Quaternion.Euler(0f, yaw, 0f) * Vector3.forward,
                magnetSurfaceNormal).normalized;

            if (surfaceForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(surfaceForward, magnetSurfaceNormal);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 15f));
            }
        }
        else
        {

            rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
        }
    }

    // ── Jump ──────────────────────────────────────────────────────────────────

    void HandleJump()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space)
                        || Input.GetKeyDown(KeyCode.Joystick1Button0);

        if (jumpPressed && isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // ── Ground check ──────────────────────────────────────────────────────────

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ── Collisions ───────────────────────────────────────────────────────────

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Steam"))
            rb.AddForce(Vector3.up * steamBoost, ForceMode.Impulse);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Steam")) return;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y < 0.4f)
            {
                wallNormal     = contact.normal;
                isTouchingWall = true;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isTouchingWall = false;
        wallNormal     = Vector3.zero;
    }

    // ── Camera ────────────────────────────────────────────────────────────────

    void UpdateCameraTarget()
    {
        if (cameraTarget == null)
        {
            Debug.LogError("cameraTarget is NULL!");
            return;
        }

        Vector3 cameraUp = magnetActive ? magnetSurfaceNormal : Vector3.up;


        cameraTarget.position = transform.position + cameraUp * cameraYOffset;


        Quaternion baseYaw    = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(Quaternion.Euler(0f, yaw, 0f) * Vector3.forward, cameraUp).normalized,
            cameraUp);
        Quaternion pitchRot   = Quaternion.AngleAxis(pitch, baseYaw * Vector3.right);
        cameraTarget.rotation = pitchRot * baseYaw;


        cameraTarget.position += cameraTarget.right * shoulderOffsetX;
    }
}