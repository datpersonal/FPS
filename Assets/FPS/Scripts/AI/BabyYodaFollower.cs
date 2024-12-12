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
            enabled = false;
            return;
        }

        // Snap Baby Yoda to the NavMesh
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            enabled = false;
            return;
        }
        else
        {
            transform.position = hit.position; // Snap to the nearest point on the NavMesh
        }

        // Force NavMeshAgent to reset
        navMeshAgent.enabled = false;
        navMeshAgent.enabled = true;
    }

    void Update()
    {
        if (playerTransform == null || navMeshAgent == null)
        {
            return;
        }

        if (!navMeshAgent.isOnNavMesh)
        {
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
                }
                navMeshAgent.speed = MoveSpeed;
            }
            else
            {
                navMeshAgent.ResetPath();
            }
        }
        else
        {
            navMeshAgent.ResetPath();
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