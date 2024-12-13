using UnityEngine;
using UnityEngine.AI;

public class BabyYodaFollower : MonoBehaviour
{
    public float ActivateFollowDistance = 10.0f; // Distance to activate following behavior
    public float StopFollowDistance = 2.0f; // Distance to stop following the player
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
            playerTransform.position += new Vector3(0, 2, 0);
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found in the scene! Disabling script.");
            enabled = false;
            return;
        }

        // Snap Baby Yoda to the nearest point on the NavMesh
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 500f, NavMesh.AllAreas))
        {
            transform.position = closestHit.position;
            Debug.Log("Baby Yoda successfully snapped to the NavMesh.");
        }
        else
        {
            Debug.LogError("Could not find a position on the NavMesh close to Baby Yoda's starting position.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (playerTransform == null || navMeshAgent == null)
        {
            return;
        }

        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning($"NavMeshAgent is not on a NavMesh! Position: {transform.position}");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Follow player if within activation range
        if (distanceToPlayer <= ActivateFollowDistance)
        {
            if (distanceToPlayer > StopFollowDistance)
            {
                navMeshAgent.SetDestination(playerTransform.position);
                navMeshAgent.speed = MoveSpeed;
                Debug.Log($"Baby Yoda moving towards Player. Distance: {distanceToPlayer}");
            }
            else
            {
                // Stop moving when close to the player
                navMeshAgent.ResetPath();
                Debug.Log("Baby Yoda is close to the player. Stopping movement.");
            }
        }
        else
        {
            // Stop following if the player is out of range
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