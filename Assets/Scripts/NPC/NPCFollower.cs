using UnityEngine;
using UnityEngine.AI;

public class NPCFollower : MonoBehaviour
{
    [SerializeField] private int recordsBehind = 15;
    [SerializeField] private float stopDistance = 0.05f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float destinationUpdateInterval = 0.2f;

    private Animator animator;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private bool isFollowing = false;
    private float destinationTimer = 0f;

    private int npcLayer;
    private int playerLayer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.updatePosition = false;

        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance;

        npcLayer = LayerMask.NameToLayer("NPC");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    public void StartFollowing()
    {
        isFollowing = true;

        Physics2D.IgnoreLayerCollision(npcLayer, npcLayer, true);
        Physics2D.IgnoreLayerCollision(npcLayer, playerLayer, true);
    }

    void FixedUpdate()
    {
        if (!isFollowing || PauseController.IsGamePaused || PositionHistoryRecorder.Instance == null)
        {
            return;
        }

        agent.nextPosition = transform.position;

        destinationTimer -= Time.fixedDeltaTime;
        if (destinationTimer <= 0f)
        {
            Vector3 targetPosition =
                PositionHistoryRecorder.Instance.GetPositionAtIndex(recordsBehind);

            agent.SetDestination(targetPosition);
            destinationTimer = destinationUpdateInterval;
        }

        Vector2 desiredVelocity = agent.desiredVelocity;

        bool isMoving =
            desiredVelocity.sqrMagnitude > 0.001f &&
            agent.remainingDistance > agent.stoppingDistance;

        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);

            if (isMoving)
            {
                Vector2 dir = desiredVelocity.normalized;

                animator.SetFloat("InputX", dir.x);
                animator.SetFloat("InputY", dir.y);

                animator.SetFloat("LastInputX", dir.x);
                animator.SetFloat("LastInputY", dir.y);
            }
        }

        if (isMoving)
        {
            Vector2 newPosition = rb.position + desiredVelocity * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    void OnEnable()
    {
        MapTransition.OnPlayerAreaTransition += HandleAreaTransition;
    }

    void OnDisable()
    {
        MapTransition.OnPlayerAreaTransition -= HandleAreaTransition;
    }

    private void HandleAreaTransition(Vector3 newPlayerPosition)
    {
        if (!isFollowing) return;

        WarpTo(newPlayerPosition);
    }

    public void WarpTo(Vector3 position)
    {
        rb.position = position;
        transform.position = position;

        if (agent.isOnNavMesh)
        {
            agent.Warp(position);
        }

        destinationTimer = 0f;
    }
}