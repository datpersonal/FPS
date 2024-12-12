using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
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

        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning($"NavMeshAgent is not on a NavMesh! Position: {transform.position}");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Log agent details
        Debug.Log($"Agent Has Path: {navMeshAgent.hasPath}, Is Stuck: {navMeshAgent.isStopped}, Remaining Distance: {navMeshAgent.remainingDistance}");

        if (distanceToPlayer <= ActivateFollowDistance)
        {
            if (distanceToPlayer > StopFollowDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.1f)
                {
                    navMeshAgent.SetDestination(playerTransform.position);
                    Debug.Log("Setting new destination for Baby Yoda.");
                }
                navMeshAgent.speed = MoveSpeed;
            }
            else
            {
                navMeshAgent.ResetPath();
                Debug.Log("Baby Yoda is close to the player. Stopping movement.");
            }
        }
        else
        {
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