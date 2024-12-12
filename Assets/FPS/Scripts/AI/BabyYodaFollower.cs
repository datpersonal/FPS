using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;

public class BabyYodaFollower : MonoBehaviour
{
    public float ActivateFollowDistance = 5.0f; // Distance to activate following behavior
    public float StopFollowDistance = 8.0f; // Distance to stop following the player
    public float MoveSpeed = 3.5f; // Speed at which Baby Yoda moves

    private NavMeshAgent navMeshAgent;
    private Transform playerTransform; // Reference to the player's Transform

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not attached to Baby Yoda! Disabling script.");
            enabled = false;
            return;
        }

        // Dynamically find the Player by tag
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found in the scene! Disabling script.");
            enabled = false;
            return;
        }

        // Snap Baby Yoda to the NavMesh
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.LogError("Baby Yoda is not close enough to the NavMesh! Disabling script.");
            enabled = false;
            return;
        }
        else
        {
            transform.position = hit.position; // Snap to the nearest point on the NavMesh
            Debug.Log("Baby Yoda successfully snapped to the NavMesh.");
        }

        // Force NavMeshAgent to reset
        navMeshAgent.enabled = false;
        navMeshAgent.enabled = true;
    }

    void Update()
    {
        if (playerTransform == null || navMeshAgent == null)
        {
            Debug.LogWarning("Player or NavMeshAgent is missing. Stopping Baby Yoda.");
            return;
        }

        // Check if NavMeshAgent is active and on the NavMesh
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning($"NavMeshAgent is not on a NavMesh! Position: {transform.position}");
            Debug.Log($"NavMeshAgent enabled: {navMeshAgent.enabled}, Transform position: {transform.position}");
            return;
        }

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Check if the player is within the follow activation range
        if (distanceToPlayer <= ActivateFollowDistance)
        {
            // Move towards the player if they are within the follow distance
            if (distanceToPlayer > StopFollowDistance)
            {
                navMeshAgent.SetDestination(playerTransform.position);
                navMeshAgent.speed = MoveSpeed;
                Debug.Log($"Baby Yoda moving towards Player. Distance: {distanceToPlayer}");
            }
            else
            {
                // Stop movement if the player is too far away
                navMeshAgent.ResetPath();
                Debug.Log("Baby Yoda is too far from the player. Stopping movement.");
            }
        }
        else
        {
            // Stop following if the player is out of the activation range
            navMeshAgent.ResetPath();
            Debug.Log("Player is out of follow range. Baby Yoda stopped.");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a circle representing the activation follow distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ActivateFollowDistance);

        // Draw a circle representing the stop follow distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, StopFollowDistance);
    }
}
