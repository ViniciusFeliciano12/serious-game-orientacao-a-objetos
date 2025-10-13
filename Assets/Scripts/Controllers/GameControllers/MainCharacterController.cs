using Esper.FeelSpeak;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MainCharacterController : MonoBehaviour
{
    public static MainCharacterController Instance { get; private set; }

    [Tooltip("Speed ​​at which the character moves. It is not affected by gravity or jumping.")]
    public float velocity = 5f;
    [Tooltip("This value is added to the speed value while the character is sprinting.")]
    public float sprintAdittion = 3.5f;
    [Tooltip("The higher the value, the higher the character will jump.")]
    public float jumpForce = 18f;
    [Tooltip("Stay in the air. The higher the value, the longer the character floats before falling.")]
    public float jumpTime = 0.85f;
    [Space]
    [Tooltip("Force that pulls the player down. Changing this value causes all movement, jumping and falling to be changed as well.")]
    public float gravity = 9.8f;

    float jumpElapsedTime = 0;
    public float transitionSpeed = 5f;
    public float speedTransitionSpeed = 5f;

    //audios

    private AudioSource[] audioSources;

    // Player states
    bool isJumping = false;
    bool isSprinting = false;

    // Inputs
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputSprint;

    private float timer = 0f;
    private readonly float interval = 2f; 

    public Animator animator;
    CharacterController cc;

    private bool initialPositionSet = false; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSources = GetComponents<AudioSource>();

        if (animator == null)
            Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
    }

    void Update()
    {
        if (CannotMove())
        {
            inputHorizontal = 0f;
            inputVertical = 0f;
            inputJump = false;
            isSprinting = false;
        }
        else
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");
            inputJump = Input.GetAxis("Jump") == 1f;
            inputSprint = Input.GetAxis("Fire3") == 1f;
           
            if (inputJump && cc.isGrounded)
            {
                isJumping = true;
            }
        }

        if (cc.isGrounded && animator != null)
        {
            float minimumSpeed = 0.9f;
            float targetSpeed = new Vector2(cc.velocity.x, cc.velocity.z).magnitude > 0.1f ? 1f : 0f;
            animator.SetFloat("Speed", targetSpeed);

            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
        }

        if (animator != null)
        {
            float targetValue = cc.isGrounded ? 0f : 1f;
            animator.SetFloat("Jumping", targetValue);
        }

        HeadHittingDetect();
    }

    private void FixedUpdate()
    {
        if (!initialPositionSet)
        {
            var playerPosition = GameController.Instance.GetPlayerPosition();
            if (playerPosition != new Vector3())
            {
                Debug.Log("Aplicando posição inicial: " + playerPosition);
                cc.enabled = false;
                transform.position = playerPosition;
                cc.enabled = true;
            }
            initialPositionSet = true;
        }

        timer += Time.fixedDeltaTime;
        if (timer >= interval)
        {
            GameController.Instance.UpdatePlayerPosition(transform.position);
            timer = 0f;
        }

        float velocityAdittion = 0;
        if (isSprinting)
            velocityAdittion = sprintAdittion;

        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

        if (isJumping)
        {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

        directionY -= gravity * Time.deltaTime;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        forward *= directionZ;
        right *= directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = forward + right;

        Vector3 moviment = verticalDirection + horizontalDirection;
        cc.Move(moviment);

        // 🎵 Controle do som de passos
        bool isMoving = (directionX != 0 || directionZ != 0);
        bool grounded = cc.isGrounded;

        if (isMoving && grounded)
        {
            if (!audioSources[1].isPlaying)
            {
                audioSources[1].loop = true;
                audioSources[1].Play();
            }
        }
        else
        {
            if (audioSources[1].isPlaying)
            {
                audioSources[1].Stop();
            }
        }
    }

    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

    public bool CannotMove()
    {
        return  PauseController.Instance.TimeStopped ||
                DialogueManagement.Instance.HasActiveDialogue() || 
                (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && 
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Run") && 
                !animator.GetCurrentAnimatorStateInfo(0).IsName("RunJump"));
    }
}
