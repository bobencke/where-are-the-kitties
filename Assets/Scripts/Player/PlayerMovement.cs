using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

    void Start()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PauseController.IsGamePaused || LoadingScreenController.IsTransitioning)
        {
            animator.SetBool("isWalking", false);
            StopFootsteps();
            return;
        }

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isMoving);

        if(isMoving && !playingFootsteps)
        {
            StartFootsteps();
        }
        else if(!isMoving && playingFootsteps)
        {
            StopFootsteps();
        }
    }

    void FixedUpdate()
    {
        if (PauseController.IsGamePaused || LoadingScreenController.IsTransitioning)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep", true);
    }
}
