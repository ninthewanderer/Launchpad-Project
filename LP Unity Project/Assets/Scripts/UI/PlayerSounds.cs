using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    [Header("Movement Sounds")]
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioClip landSound;

    [Header("Boot Sounds")]
    public AudioClip rocketBootsSound;
    public AudioClip detectionBootsSound;
    public AudioClip magnetBootsSound;
    public AudioClip swapBootsSound;

    [Header("Volume")]
    [Range(0f, 1f)] public float walkVolume = 0.7f;
    [Range(0f, 1f)] public float runVolume = 1f;
    [Range(0f, 1f)] public float landVolume = 0.9f;
    [Range(0f, 1f)] public float rocketBootsVolume = 0.8f;
    [Range(0f, 1f)] public float detectionBootsVolume = 0.9f;
    [Range(0f, 1f)] public float magnetBootsVolume = 0.8f;


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

    private void OnEnable()
    {
        BootMovement.OnBootEffect += BootMovement_OnBootEffect;
        BootMovement.OnBootSwap += BootMovement_OnBootSwap;
    }

    private void OnDisable()
    {
        BootMovement.OnBootEffect -= BootMovement_OnBootEffect;
        BootMovement.OnBootSwap -= BootMovement_OnBootSwap;
    }   

    private void BootMovement_OnBootSwap(bool boot)
    {
        SoundManager.instance.PlaySoundFXClip(swapBootsSound, transform, 0.8f);
    }
    private void BootMovement_OnBootEffect(BootMovement.BootType obj)
    {
        if (obj == BootMovement.BootType.RocketBoots)
        {
            SoundManager.instance.PlaySoundFXClip(rocketBootsSound, transform, rocketBootsVolume);
        }
        else if (obj == BootMovement.BootType.DetectionBoots)
        {
            SoundManager.instance.PlaySoundFXClip(detectionBootsSound, transform, detectionBootsVolume);
        }
         else if (obj == BootMovement.BootType.MagnetBoots)
        {
            SoundManager.instance.PlaySoundFXClip(magnetBootsSound, transform, magnetBootsVolume);
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
        Debug.Log($"Boot effect sound played for {obj}.");
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

        if (playerController.IsGrounded && isMoving)
        {
            AudioClip target = isSprinting ? runSound : walkSound;
            float volume     = isSprinting ? runVolume : walkVolume;
            PlayLoop(target, volume);
            return;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && playerController.IsGrounded)
        {
            SoundManager.instance.PlaySoundFXClip(jumpSound, transform, .8f);
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