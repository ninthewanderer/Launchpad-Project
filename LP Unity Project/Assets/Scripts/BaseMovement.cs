using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 9f;
    public float turnSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Camera/Look")]
    public Transform cameraTarget;
    public float cameraYOffset = 1.5f;
    public float mouseSensitivity = 200f;
    public float controllerSensitivity = 150f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private Rigidbody rb;
    private bool isGrounded;
    private float yaw;
    private float pitch;
    private float horizontalInput;
    private float verticalInput;
    private bool jumpPressed;
    private bool isSprinting;

    public bool IsGrounded => isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Cursor.lockState = CursorLockMode.Locked;

        yaw = transform.eulerAngles.y;
        pitch = 0f;
    }

    void Update()
    {
        CheckGround();
        ReadInput();
        HandleJump();
        UpdateCameraTarget();
    }

    void FixedUpdate()
    {
        
        MovePlayer();
        RotatePlayer();
    }

    void ReadInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        isSprinting = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Joystick1Button2);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        float stickX = 0f;
        float stickY = 0f;
        try
        {
            stickX = Input.GetAxis("RightStickX") * controllerSensitivity * Time.deltaTime;
            stickY = Input.GetAxis("RightStickY") * controllerSensitivity * Time.deltaTime;
        }
        catch { }

        yaw += mouseX + stickX;
        pitch -= mouseY + stickY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void MovePlayer()
    {
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 moveDir = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDir = moveDir.normalized * currentSpeed;

        Vector3 newVelocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
        rb.velocity = newVelocity;
    }

    void RotatePlayer()
    {
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
    }

    void HandleJump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void UpdateCameraTarget()
    {
        if (cameraTarget == null) return;

        Vector3 targetPos = transform.position + Vector3.up * cameraYOffset;
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, targetPos, 10f * Time.deltaTime);

        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}