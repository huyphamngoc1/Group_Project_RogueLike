using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] Transform target; // The player's position
    [SerializeField] float safeDistance = 10f; // Desired distance to maintain from the player
    [SerializeField] float followDistance = 5f; // Distance to maintain from the EliteEnemy
    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;
    Animator animator;

    public float shootingRange;
    public GameObject bullet;
    public GameObject bulletParent;

    private void OnDrawGizmos()
    {
        // Set the Gizmo color for the safe distance
        Gizmos.color = Color.blue;

        // Draw a wire sphere representing the safe distance radius
        Gizmos.DrawWireSphere(transform.position, safeDistance);

        // Set the Gizmo color for the follow distance
        Gizmos.color = Color.red;

        // Draw a wire sphere representing the follow distance radius
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing on this GameObject!");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing on this GameObject!");
        }
    }


    [SerializeField] private float attackCooldown = 2f; // Thời gian hồi chiêu giữa mỗi phát bắn
    private float lastAttackTime = -Mathf.Infinity; // Thời điểm lần bắn cuối

    private void Update()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= safeDistance)
            {
                // Move away from player while trying to avoid cornering itself
                Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
                Vector3 targetPosition = target.position + directionAwayFromPlayer * safeDistance;

                // Set the agent's destination to avoid getting too close to the player
                agent.SetDestination(targetPosition);
            }
            else
            {
                // If the player is far enough away, move towards the target to close the gap (not necessarily the exact position)
                Vector3 directionTowardsPlayer = (target.position - transform.position).normalized;
                agent.SetDestination(target.position + directionTowardsPlayer * safeDistance); // Keeps a buffer between enemy and player
            }

            // Attack logic
            if (distanceToTarget <= shootingRange && Time.time >= lastAttackTime + attackCooldown)
            {
                // Switch to attack animation
                animator.SetBool("Enemy_Attack", true);

                // Check for bullet existence and spawn it
                if (bullet != null && bulletParent != null)
                {
                    Instantiate(bullet, bulletParent.transform.position, Quaternion.identity);
                    lastAttackTime = Time.time; // Update the time of the last shot
                }
            }
            else
            {
                animator.SetBool("Enemy_Attack", false);
            }

            // Flip sprite based on the player's position
            FlipSprite(target.position.x);
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

    // Flip the sprite based on the player's position
    private void FlipSprite(float targetX)
    {
        if (spriteRenderer != null)
        {
            // Flip the sprite if the target is on the left or right
            spriteRenderer.flipX = targetX >= transform.position.x;
        }
    }
}
