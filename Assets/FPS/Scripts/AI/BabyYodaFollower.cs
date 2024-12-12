using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyYodaFollower : MonoBehaviour
{
    public Transform Player; // Reference to the player
    public float FollowDistance = 2.0f; // Distance to maintain from the player
    public float MoveSpeed = 3.5f; // Speed at which Baby Yoda moves

    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is not attached to Baby Yoda!");
        }

        if (Player == null)
        {
            // Find the player dynamically if not set in the Inspector
            Player = GameObject.FindWithTag("Player")?.transform;
            if (Player == null)
            {
                Debug.LogError("Player with tag 'Player' not found in the scene!");
            }
        }

        // Ensure Baby Yoda starts on the NavMesh
        if (!UnityEngine.AI.NavMesh.SamplePosition(transform.position, out var hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            Debug.LogError("Baby Yoda is not on the NavMesh! Please adjust its position.");
            enabled = false; // Disable the script to prevent errors
        }
    }

    void Update()
    {
        if (Player == null || navMeshAgent == null)
            return;

        // Ensure the NavMeshAgent is on the NavMesh
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on a NavMesh! Stopping movement.");
            return;
        }

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        // Move towards the player if not within follow distance
        if (distanceToPlayer > FollowDistance)
        {
            navMeshAgent.SetDestination(Player.position);
            navMeshAgent.speed = MoveSpeed;
        }
        else
        {
            // Stop moving when within follow distance
            navMeshAgent.ResetPath();
        }
    }
}