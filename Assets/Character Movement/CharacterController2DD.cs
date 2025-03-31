using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2DD : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private Animator animator;

    [SerializeField] private float speed = 5f; // Character movement speed
    private Vector2 movement; // Vector for storing movement input

    private bool canMove = true; // Control if the character can move

    void Start()
    {
        // Get components
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Set gravity scale to 0 to prevent falling
        rigidbody2d.gravityScale = 0;
    }

    void Update()
    {
        if (canMove)
        {
            // Get movement input from the player
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Send movement input to the animator
            animator.SetFloat("horizontal", movement.x);
            animator.SetFloat("vertical", movement.y);
            animator.SetFloat("speed", movement.sqrMagnitude); // This helps determine if the character is moving or idle
        }
        else
        {
            // Stop the character's movement if they can't move
            movement = Vector2.zero;
            animator.SetFloat("speed", 0); // Set speed to 0 to ensure idle animation
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            // Move the character
            rigidbody2d.velocity = movement * speed;
        }
        else
        {
            // Ensure the character stops when they can't move
            rigidbody2d.velocity = Vector2.zero;
        }
    }

    public void StopMovement()
    {
        canMove = false;
        rigidbody2d.velocity = Vector2.zero; // Ensure character stops immediately
        animator.SetFloat("speed", 0); // Set speed to 0 to ensure idle animation
    }

    public void ResumeMovement()
    {
        canMove = true;
    }
}
