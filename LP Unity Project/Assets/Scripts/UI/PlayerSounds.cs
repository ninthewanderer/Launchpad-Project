using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    [Header("Movement Sounds")]
    public AudioClip walkSound;
    public AudioClip runSound;

    [Header("Boot Sounds")]
    public AudioClip rocketBootsSound;
    public AudioClip detectionBootsSound;

    [Header("Volume")]
    [Range(0f, 1f)] public float walkVolume = 0.7f;
    [Range(0f, 1f)] public float runVolume = 1f;
    [Range(0f, 1f)] public float rocketBootsVolume = 0.8f;
    [Range(0f, 1f)] public float detectionBootsVolume = 0.9f;

    private AudioSource audioSource;
    private PlayerController playerController;
    private BootMovement bootMovement;

    private const float MoveThreshold = 0.1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;

        playerController = GetComponent<PlayerController>();
        bootMovement = GetComponent<BootMovement>();

        if (playerController == null)
            Debug.LogWarning("PlayerSounds: No PlayerController found on this GameObject.");
        if (bootMovement == null)
            Debug.LogWarning("PlayerSounds: No BootMovement found on this GameObject.");
    }

    void Update()
    {
        UpdateMovementSound();
    }

    void UpdateMovementSound()
    {
        if (playerController == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isMoving = new Vector2(h, v).magnitude > MoveThreshold;

        bool isSprinting = Input.GetKey(KeyCode.LeftControl)
                        || Input.GetKey(KeyCode.Joystick1Button8)
                        || Input.GetKey(KeyCode.Joystick1Button2);


        bool rocketActive = bootMovement != null
                         && bootMovement.currentBoots == BootMovement.BootType.RocketBoots
                         && !playerController.IsGrounded;

        if (rocketActive)
        {
            PlayLoop(rocketBootsSound, rocketBootsVolume);
            return;
        }

        if (playerController.IsGrounded && isMoving)
        {
            AudioClip target = isSprinting ? runSound : walkSound;
            float volume     = isSprinting ? runVolume : walkVolume;
            PlayLoop(target, volume);
            return;
        }


        if (audioSource.isPlaying)
            audioSource.Stop();
    }

  
    void PlayLoop(AudioClip clip, float volume)
    {
        if (clip == null) return;

        if (audioSource.clip == clip)
        {
            
            audioSource.volume = volume;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            
            audioSource.Stop();
            audioSource.clip   = clip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }


    public void PlayDetectionBootsSound()
    {
        if (detectionBootsSound != null)
            audioSource.PlayOneShot(detectionBootsSound, detectionBootsVolume);
    }
}