using UnityEngine;
using UnityEngine.AI;

public class AI_NPC : MonoBehaviour
{
    public Transform target; // Assign a target position in the Inspector
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (target != null)
        {
            MoveToTarget(target.position);
        }
    }

    void MoveToTarget(Vector3 targetPosition)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);
        }
    }

    // Optionally update target dynamically
    void Update()
    {
        if (target != null)
        {
            MoveToTarget(target.position);
        }
    }
}