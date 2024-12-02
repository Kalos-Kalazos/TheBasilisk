using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class P_AI_Enemy : MonoBehaviour
{
    [Header("=== Enemy Status Settings ===")]
    [SerializeField] int health;
    [SerializeField] int speedChase;
    [SerializeField] int damage;

    [Header("=== Enemy Patrol Settings ===")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] int currentPatrolIndex;
    [SerializeField] NavMeshAgent agent;

    [Header("=== Enemy Detection Settings ===")]
    [SerializeField] float detectionRange = 10;
    [SerializeField] Transform player;
    [SerializeField] bool isChasing;
    [SerializeField] float maxDetectionAngle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;

        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }


    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToTarget <= maxDetectionAngle)
            {
                if (HasVision(player))
                {
                    isChasing = true;
                }
            }
            else isChasing = false;
        }
    }

    bool HasVision(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToTarget, out hit, detectionRange))
        {
            if(hit.transform == target)
            {
                return true;
            }
        }

        return false;
    }

    void ChasePlayer()
    {
        agent.destination = player.position;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
        {
            isChasing = false;
            if (patrolPoints.Length > 0)
            {
                agent.destination = patrolPoints[currentPatrolIndex].position;
                agent.speed = speedChase;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (isChasing) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
