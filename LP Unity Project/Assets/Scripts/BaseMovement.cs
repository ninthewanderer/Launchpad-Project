using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float turnSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Cursor.lockState = CursorLockMode.Locked;

        yaw = transform.eulerAngles.y;
        pitch = 0f;
    }

    void Update()
    {
        ReadInput();
        HandleJump();
        UpdateCameraTarget();
    }

    void FixedUpdate()
    {
        CheckGround();
        MovePlayer();
        RotatePlayer();
    }

    void ReadInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

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
        
        Vector3 moveDir = new Vector3(horizontalInput, 0f, verticalInput);
        Vector3 horizontalVelocity = moveDir.normalized * speed;
        horizontalVelocity = transform.TransformDirection(horizontalVelocity);

       
        Vector3 newVelocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        rb.velocity = newVelocity;
    }

    void RotatePlayer()
    {
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
    }


    void HandleJump()
    {
        jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0);
        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
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