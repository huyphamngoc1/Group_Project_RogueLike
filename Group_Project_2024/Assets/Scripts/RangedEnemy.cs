using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] Transform target;  // The player's position
    [SerializeField] float safeDistance = 10f;  // Desired distance to maintain from the player
    [SerializeField] float followDistance = 5f;  // Distance to maintain from the EliteEnemy
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        // Default logic to stay away from the player, if not commanded to follow
        if (target != null)
        {
            // Calculate a direction away from the player
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            Vector3 targetPosition = target.position + directionAwayFromPlayer * safeDistance;

            // Set the agent's destination
            agent.SetDestination(targetPosition);
        }
    }

    // Method to command the enemy to follow the EliteEnemy
    public void CommandFollow(Vector3 leaderPosition)
    {
        Vector3 followPosition = leaderPosition;
        // Keep a distance from the EliteEnemy
        Vector3 directionAwayFromLeader = (transform.position - followPosition).normalized;
        Vector3 targetPosition = followPosition + directionAwayFromLeader * followDistance;

        // Set the agent's destination
        agent.SetDestination(targetPosition);
    }
}
