using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElderWalkingController : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    [SerializeField] private float speed = 2.0f;
    private Vector2 motionVector;
    private Animator animator;

    private float directionChangeInterval = 3.0f; // Time in seconds before changing direction
    private float collisionStopDuration = 10.0f; // Time in seconds to stop after a collision with the player

    private enum MovementState
    {
        Random,
        PathFollowing,
        Stopped // New state for when the NPC is stopped due to collision
    }

    private MovementState currentState;

    public List<Transform> waypoints; // Use a List for dynamic waypoint management
    public float arrivalThreshold = 0.1f; // Distance within which the NPC is considered to have reached a waypoint

    private int currentWaypointIndex = 0;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SetStaticGravity();

        // Ensure Rigidbody2D is kinematic initially for path following
        rigidbody2d.isKinematic = true;

        // Determine the initial state
        if (waypoints.Count > 0)
        {
            currentState = MovementState.PathFollowing;
            StartCoroutine(FollowPathCoroutine());
        }
        else
        {
            currentState = MovementState.Random;
            StartCoroutine(RandomMovementCoroutine());
        }
    }

    void Update()
    {
        // Update animation parameters based on motionVector
        animator.SetFloat("horizontal", motionVector.x);
        animator.SetFloat("vertical", motionVector.y);
    }

    void FixedUpdate()
    {
        if (currentState == MovementState.PathFollowing)
        {
            Move();
        }
    }

    private void Move()
    {
        if (rigidbody2d.isKinematic)
        {
            // Set kinematic Rigidbody2D velocity
            rigidbody2d.velocity = motionVector * speed;
        }
    }

    private IEnumerator RandomMovementCoroutine()
    {
        while (currentState == MovementState.Random)
        {
            // Randomly choose a direction: horizontal or vertical only
            int directionChoice = Random.Range(0, 2); // 0 for horizontal, 1 for vertical

            if (directionChoice == 0)
            {
                // Horizontal movement
                float randomX = Random.Range(-1f, 1f);
                motionVector = new Vector2(Mathf.Sign(randomX), 0); // Move left or right
            }
            else
            {
                // Vertical movement
                float randomY = Random.Range(-1f, 1f);
                motionVector = new Vector2(0, Mathf.Sign(randomY)); // Move up or down
            }

            // Normalize the direction vector to ensure consistent speed
            motionVector = motionVector.normalized;

            // Update animator parameters
            animator.SetFloat("horizontal", motionVector.x);
            animator.SetFloat("vertical", motionVector.y);

            // Wait for the specified interval
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    private IEnumerator FollowPathCoroutine()
    {
        while (currentState == MovementState.PathFollowing)
        {
            if (waypoints.Count == 0)
                yield break;

            MoveTowardsWaypoint();
            UpdateAnimation();

            // Ensure Rigidbody2D is kinematic while following the path
            rigidbody2d.isKinematic = true;

            // Wait until reaching the waypoint
            yield return new WaitUntil(() => Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < arrivalThreshold);

            // Remove the reached waypoint from the list
            waypoints.RemoveAt(currentWaypointIndex);

            // If there are still waypoints left, move to the next one
            if (waypoints.Count > 0)
            {
                // Ensure the index does not go out of range
                if (currentWaypointIndex >= waypoints.Count)
                {
                    currentWaypointIndex = 0;
                }
            }
            else
            {
                // No waypoints left, stop movement
                StopMovement();
                yield break;
            }
        }
    }

    private void MoveTowardsWaypoint()
    {
        if (waypoints.Count == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        motionVector = (targetWaypoint.position - transform.position).normalized;

        // Ensure consistent speed in both directions
        motionVector = new Vector2(
            Mathf.Clamp(motionVector.x, -1, 1),
            Mathf.Clamp(motionVector.y, -1, 1)
        );
    }

    private void UpdateAnimation()
    {
        if (motionVector != Vector2.zero)
        {
            animator.SetFloat("horizontal", motionVector.x);
            animator.SetFloat("vertical", motionVector.y);
        }
        else
        {
            animator.SetFloat("horizontal", 0);
            animator.SetFloat("vertical", 0);
        }
    }

    public void ResumeMovement()
    {
        if (currentState == MovementState.Stopped)
        {
            // Resume the previous state before stopping
            if (waypoints.Count > 0)
            {
                StartPathFollowing();
            }
            else
            {
                StartRandomMovement();
            }
        }
    }

    public void StopMovement()
    {
        rigidbody2d.velocity = Vector2.zero;
        motionVector = Vector2.zero;
        animator.SetFloat("horizontal", 0);
        animator.SetFloat("vertical", 0);

        // Ensure Rigidbody2D is kinematic when stopped
        rigidbody2d.isKinematic = true;
    }

    public void SetStaticGravity()
    {
        rigidbody2d.gravityScale = 0; // Permanently set gravity to 0
    }

    // Call this method to switch to path following mode
    public void StartPathFollowing()
    {
        StopCoroutine(RandomMovementCoroutine());
        currentState = MovementState.PathFollowing;
        StartCoroutine(FollowPathCoroutine());
    }

    // Call this method to switch to random movement mode
    public void StartRandomMovement()
    {
        StopCoroutine(FollowPathCoroutine());
        currentState = MovementState.Random;
        StartCoroutine(RandomMovementCoroutine());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(HandleCollisionWithPlayer());
        }
    }

    private IEnumerator HandleCollisionWithPlayer()
    {
        if (currentState != MovementState.Stopped)
        {
            StopMovement();
            currentState = MovementState.Stopped;

            yield return new WaitForSeconds(collisionStopDuration);

            ResumeMovement();
        }
    }
}
