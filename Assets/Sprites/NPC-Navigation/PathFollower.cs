using System.Collections;
using UnityEngine;

public class PathAndTargetFollower : MonoBehaviour
{
    public Transform[] pathNodes; // Array of path nodes
    public Transform[] targetNodes; // Array of target nodes
    public float speed = 2.0f; // Speed at which the NPC moves
    public float arrivalThreshold = 0.1f; // Distance within which the NPC is considered to have reached a node
    public float waitTimeAtTarget = 2.0f; // Time to wait at each target node

    private int currentNodeIndex = 0;
    private bool isWaiting = false;
    private bool waitingForInteraction = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (pathNodes.Length > 0 || targetNodes.Length > 0)
        {
            MoveToNextNode();
        }
    }

    void Update()
    {
        if (isWaiting || waitingForInteraction)
            return;

        MoveTowardsCurrentNode();
        UpdateAnimation();
    }

    private void MoveTowardsCurrentNode()
    {
        Transform targetNode = GetCurrentNode();
        Vector2 direction = (targetNode.position - transform.position).normalized;
        Vector2 velocity = direction * speed;

        // Move the NPC
        transform.Translate(velocity * Time.deltaTime);

        // Check if the NPC has reached the current node
        if (Vector2.Distance(transform.position, targetNode.position) < arrivalThreshold)
        {
            if (IsCurrentNodeTarget())
            {
                StartCoroutine(WaitAtTargetNode());
            }
            else
            {
                MoveToNextNode();
            }
        }
    }

    private Transform GetCurrentNode()
    {
        if (IsCurrentNodeTarget())
        {
            return targetNodes[currentNodeIndex - pathNodes.Length];
        }
        else
        {
            return pathNodes[currentNodeIndex];
        }
    }

    private bool IsCurrentNodeTarget()
    {
        return currentNodeIndex >= pathNodes.Length;
    }

    private IEnumerator WaitAtTargetNode()
    {
        waitingForInteraction = true;
        animator.SetBool("IsMoving", false); // Stop moving animation
        yield return new WaitForSeconds(waitTimeAtTarget);
        waitingForInteraction = false;

        // Move to the next node (if there is a path node to follow)
        currentNodeIndex++;
        if (currentNodeIndex >= pathNodes.Length + targetNodes.Length)
        {
            currentNodeIndex = 0; // Start again from the first path node if there are no more target nodes
        }

        MoveToNextNode();
    }

    private void MoveToNextNode()
    {
        if (pathNodes.Length > 0 && !IsCurrentNodeTarget())
        {
            animator.SetBool("IsMoving", true); // Start moving animation
        }
        else
        {
            animator.SetBool("IsMoving", false); // Stop moving animation if at a target node
        }
    }

    private void UpdateAnimation()
    {
        if (pathNodes.Length > 0 || targetNodes.Length > 0)
        {
            Transform targetNode = GetCurrentNode();
            Vector2 direction = (targetNode.position - transform.position).normalized;

            // Update animation parameters
            animator.SetFloat("horizontal", direction.x);
            animator.SetFloat("vertical", direction.y);
        }
        else
        {
            // Stop animation if there are no nodes or if waiting
            animator.SetFloat("horizontal", 0);
            animator.SetFloat("vertical", 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (waitingForInteraction && other.CompareTag("Player"))
        {
            // Trigger dialogue or interaction
            // e.g., StartDialogue();
        }
    }
}
