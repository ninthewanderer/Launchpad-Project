using System;
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
    private BootMovement boots;
    private bool isGrounded;
    private float yaw;
    private float pitch;
    private float horizontalInput;
    private float verticalInput;
    private bool isSprinting;

    [HideInInspector] public bool magnetActive = false;
    [HideInInspector] public Vector3 magnetSurfaceNormal = Vector3.up;

    private Vector3 wallNormal = Vector3.zero;
    private float initialJump;

    public bool IsGrounded => isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boots = GetComponent<BootMovement>();

        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        yaw = transform.eulerAngles.y;
        initialJump = jumpForce;

        BootMovement.OnBootSwap += JumpForceChange;
    }

    private void OnDisable()
    {
        BootMovement.OnBootSwap -= JumpForceChange;
    }

    private void JumpForceChange(BootMovement.BootType type)
    {
        if (type == BootMovement.BootType.MagnetBoots)
            jumpForce *= 2;
        else
            jumpForce = initialJump;
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
        if (!magnetActive)
            MovePlayer();

        RotatePlayer();
    }

    void LateUpdate()
    {
        UpdateCameraTarget();
    }

    float ApplyDeadZone(float value)
    {
        if (Mathf.Abs(value) < stickDeadZone) return 0f;
        return Mathf.Sign(value) * (Mathf.Abs(value) - stickDeadZone) / (1f - stickDeadZone);
    }

    void ReadInput()
    {
        float keyboardH = Input.GetAxis("Horizontal");
        float keyboardV = Input.GetAxis("Vertical");

        float stickH = ApplyDeadZone(Input.GetAxis("LeftStickX"));
        float stickV = ApplyDeadZone(Input.GetAxis("LeftStickY"));

        horizontalInput = Mathf.Abs(keyboardH) > Mathf.Abs(stickH) ? keyboardH : stickH;
        verticalInput = Mathf.Abs(keyboardV) > Mathf.Abs(stickV) ? keyboardV : stickV;

        isSprinting = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Joystick1Button8);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        float stickX = ApplyDeadZone(Input.GetAxis("RightStickX")) * controllerSensitivity * Time.deltaTime;
        float stickY = ApplyDeadZone(Input.GetAxis("RightStickY")) * controllerSensitivity * Time.deltaTime;

        yaw += mouseX + stickX;
        pitch -= mouseY + stickY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void MovePlayer()
    {
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 moveDir = (transform.forward * verticalInput +
                           transform.right * horizontalInput).normalized;

        Vector3 targetVelocity = moveDir * currentSpeed;
        Vector3 velocityChange = targetVelocity - new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void RotatePlayer()
    {
        if (magnetActive) return;
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }

    public Quaternion GetMagnetRotation(Vector3 surfaceNormal, float rotationSpeed)
    {
        Quaternion surfaceUpright = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, surfaceNormal);
        Quaternion target = yawRotation * surfaceUpright;
        return Quaternion.Slerp(transform.rotation, target, Time.fixedDeltaTime * rotationSpeed);
    }

    public float GetHorizontalInput() => horizontalInput;
    public float GetVerticalInput() => verticalInput;

    public void SuppressUpwardVelocity(float maxUpward = 0f)
    {
        Vector3 v = rb.velocity;
        if (v.y > maxUpward)
            rb.velocity = new Vector3(v.x, maxUpward, v.z);
    }

    void HandleJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Steam"))
        {
            rb.AddForce(Vector3.up * steamBoost, ForceMode.Impulse);
            return;
        }

        if (boots != null && boots.isDashing)
            SuppressUpwardVelocity(0f);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Steam")) return;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y < 0.4f)
            {
                wallNormal = contact.normal;
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        wallNormal = Vector3.zero;
    }

    // Specific to moving platforms because otherwise the player will jitter and continuously slide off.
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.transform.parent != null && gameObject.transform.parent.CompareTag("Moving Off"))
            rb.interpolation = RigidbodyInterpolation.None;
    }

    void OnTriggerExit(Collider other)
    {
        if (rb.interpolation == RigidbodyInterpolation.None)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void UpdateCameraTarget()
    {
        cameraTarget.position = transform.position + Vector3.up * cameraYOffset;
        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
        cameraTarget.position += cameraTarget.right * shoulderOffsetX;
    }
}