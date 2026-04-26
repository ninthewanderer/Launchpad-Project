using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(BootMovement))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int ParamSpeed     = Animator.StringToHash("Speed");
    private static readonly int ParamIsGround  = Animator.StringToHash("isGrounded");
    private static readonly int ParamJump      = Animator.StringToHash("Jump");
    private static readonly int ParamStomp     = Animator.StringToHash("Stomp");
    private static readonly int ParamIsDashing = Animator.StringToHash("IsDashing");

    [Header("Tuning")]
    public float speedDampTime = 0.05f;

    private Animator         anim;
    private PlayerController player;
    private BootMovement     boots;
    private Rigidbody        rb;

    private bool wasGrounded   = true;
    private bool dashWasActive = false;

    void Awake()
    {
        anim   = GetComponentInChildren<Animator>();
        player = GetComponent<PlayerController>();
        boots  = GetComponent<BootMovement>();
        rb     = GetComponent<Rigidbody>();

        if (anim   == null) Debug.LogError("PlayerAnimator: Animator not found in children.");
        if (player == null) Debug.LogError("PlayerAnimator: PlayerController not found.");
        if (boots  == null) Debug.LogError("PlayerAnimator: BootMovement not found.");
        if (rb     == null) Debug.LogError("PlayerAnimator: Rigidbody not found.");

    }

    void Update()
    {
        if (anim == null || rb == null) return;

        bool grounded = player.IsGrounded;

        UpdateSpeed();
        UpdateGrounded(grounded);
        UpdateJump(grounded);
        UpdateStomp();
        UpdateDash();

        wasGrounded = grounded;
    }

    void UpdateSpeed()
    {
        float speed;

        if (boots.currentBoots == BootMovement.BootType.MagnetBoots)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            speed = new Vector2(h, v).magnitude;
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            speed = horizontalVelocity.magnitude;
        }

        if (speed < 0.1f) speed = 0f;

        anim.SetFloat(ParamSpeed, speed, speedDampTime, Time.deltaTime);
    }

    void UpdateGrounded(bool grounded)
    {
        anim.SetBool(ParamIsGround, grounded);
    }

    void UpdateJump(bool grounded)
    {
        bool justLeftGround = wasGrounded && !grounded;
        if (justLeftGround && rb.velocity.y > 0.5f)
            anim.SetTrigger(ParamJump);
        else
            anim.ResetTrigger(ParamJump);
    }

    void UpdateStomp()
    {
        bool detectionAbilityUsed = boots.currentBoots == BootMovement.BootType.DetectionBoots
            && !boots.onCooldown
            && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button3));

        if (detectionAbilityUsed)
            anim.SetTrigger(ParamStomp);
        else
            anim.ResetTrigger(ParamStomp);
    }

    void UpdateDash()
    {
        bool isDashing = false;

        if (boots.currentBoots == BootMovement.BootType.RocketBoots && !player.IsGrounded)
        {
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isDashing = horizontalVelocity.magnitude > boots.horizontalBoostForce * 0.6f;
        }

        if (isDashing != dashWasActive)
        {
            anim.SetBool(ParamIsDashing, isDashing);
            dashWasActive = isDashing;
        }
    }
}