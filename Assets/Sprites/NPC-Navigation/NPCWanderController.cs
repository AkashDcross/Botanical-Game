using UnityEngine;

public class NPCWanderController : MonoBehaviour
{
    private Animator animator;
    private Vector2 direction;
    private float moveSpeed = 2.0f; // Speed at which the NPC moves
    private float changeDirectionTime = 3.0f; // Time to walk in one direction
    private float timer;
    private bool isWalkingUp = true; // Flag to toggle between directions

    private void Start()
    {
        animator = GetComponent<Animator>();
        timer = changeDirectionTime;
        SetDirectionAndAnimation(Vector2.up, 2); // Start by walking up
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ChangeDirection();
            timer = changeDirectionTime; // Reset timer
        }

        MoveNPC();
        UpdateAnimation();
    }

    private void ChangeDirection()
    {
        if (isWalkingUp)
        {
            SetDirectionAndAnimation(Vector2.right, 1); // Change to walking right
        }
        else
        {
            SetDirectionAndAnimation(Vector2.up, 2); // Change back to walking up
        }

        isWalkingUp = !isWalkingUp; // Toggle the flag
    }

    private void SetDirectionAndAnimation(Vector2 newDirection, int directionValue)
    {
        direction = newDirection;
        animator.SetInteger("Direction", directionValue);
        animator.SetBool("IsMoving", true); // Ensure NPC is moving
    }

    private void MoveNPC()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        // Check if the NPC should stop moving
        bool isMoving = direction != Vector2.zero;
        animator.SetBool("IsMoving", isMoving);

        // Optional: Log current state for debugging
        Debug.Log($"Direction: {animator.GetInteger("Direction")}, IsMoving: {isMoving}");
    }
}
