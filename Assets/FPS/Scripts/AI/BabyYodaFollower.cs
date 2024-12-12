using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;

public class BabyYodaFollower : MonoBehaviour
{
    public Transform Player; // Reference to the player
    public float FollowDistance = -2.0f; // Distance to maintain from the player
    public float MoveSpeed = 3.5f; // Speed at which Baby Yoda moves

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not attached to Baby Yoda! Disabling script.");
            enabled = false;
            return;
        }

        // Dynamically find the Player if not assigned
        if (Player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                Player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player with tag 'Player' not found in the scene! Disabling script.");
                enabled = false;
                return;
            }
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
        if (Player == null || navMeshAgent == null)
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
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        // Move towards the player if not within follow distance
        if (distanceToPlayer > FollowDistance)
        {
            navMeshAgent.SetDestination(Player.position);
            navMeshAgent.speed = MoveSpeed;
            Debug.Log($"Baby Yoda moving towards Player. Distance: {distanceToPlayer}");
        }
        else
        {
            navMeshAgent.ResetPath();
            Debug.Log("Baby Yoda is within follow distance. Stopping movement.");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a circle representing the follow distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FollowDistance);
    }
}