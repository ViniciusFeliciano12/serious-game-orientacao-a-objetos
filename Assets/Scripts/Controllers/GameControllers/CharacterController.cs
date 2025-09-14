using UnityEngine;

[RequireComponent(typeof(UnityEngine.CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterController : MonoBehaviour
{
    public static CharacterController Instance { get; private set; }

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

    // Player states
    bool isJumping = false;
    bool isSprinting = false;
    bool isCrouching = false;

    // Inputs
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputCrouch;
    bool inputSprint;

    // Combat system
    bool combatMode = false;
    bool isAttacking = false;
    bool attackLeftNext = true;
    float attackCooldown = 0f;
    readonly float attackDuration = 0.5f;

    private float timer = 0f;
    private readonly float interval = 2f; // 2 segundos

    public Animator animator;
    UnityEngine.CharacterController cc;

    private bool initialPositionSet = false; // <-- nova variável para controlar o posicionamento inicial

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
        cc = GetComponent<UnityEngine.CharacterController>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
    }

    void Update()
    {
        if (CannotMove())
            return;

        // Input checkers
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton1);

        if (inputCrouch)
            isCrouching = !isCrouching;

        if (cc.isGrounded && animator != null)
        {
            float minimumSpeed = 0.9f;
            float targetSpeed = cc.velocity.magnitude > 0 ? 1f : 0f;
            animator.SetFloat("Speed", targetSpeed);
            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
        }

        if (animator != null)
        {
            float targetValue = cc.isGrounded ? 0f : 1f;
            animator.SetFloat("Jumping", targetValue);
        }

        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
        }

        // Toggle Combat Mode
        if (Input.GetKeyDown(KeyCode.C) && !isJumping)
        {
            combatMode = !combatMode;
            if (animator != null)
            {
                animator.SetBool("CombatMode", combatMode);
            }
        }

        // Combat attack logic
        if (combatMode && !isAttacking && Input.GetMouseButtonDown(0))
        {
            if (animator != null)
            {
                string animationTrigger = attackLeftNext ? "PunchLeft" : "PunchRight";
                animator.SetTrigger(animationTrigger);

                isAttacking = true;
                attackCooldown = attackDuration;
                attackLeftNext = !attackLeftNext;
            }
        }

        if (isAttacking)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                isAttacking = false;
            }
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

        if (CannotMove())
            return;

        timer += Time.fixedDeltaTime;

        if (timer >= interval)
        {
            GameController.Instance.UpdatePlayerPosition(transform.position);
            timer = 0f;
        }

        float velocityAdittion = 0;
        if (isSprinting)
            velocityAdittion = sprintAdittion;
        if (isCrouching)
            velocityAdittion = -(velocity * 0.50f);

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
        return PauseController.Instance.timeStopped || animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") || animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking");
    }
}
