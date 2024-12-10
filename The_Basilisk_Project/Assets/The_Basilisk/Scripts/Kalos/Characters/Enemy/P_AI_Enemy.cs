using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class P_AI_Enemy : MonoBehaviour
{
    [Header("=== Enemy Status Settings ===")]
    [SerializeField] int health = 100;
    [SerializeField] float speedChase = 7;
    [SerializeField] float speedDefault = 3.5f;
    [SerializeField] int damage = 2;

    [Header("=== Enemy Patrol Settings ===")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] int currentPatrolIndex;
    [SerializeField] NavMeshAgent agent;

    [Header("=== Enemy Detection Settings ===")]
    [SerializeField] float detectionRange = 20;
    [SerializeField] Transform player;
    [SerializeField] GameObject tempPP;
    [SerializeField] bool isChasing = false;
    [SerializeField] bool isChecking = false;
    [SerializeField] float maxDetectionAngle = 45;
    [SerializeField] float alertRadius = 50;

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
            if (isChecking)
            {
                GoCheck(tempPP);
            }
            else
            {
                Patrol();
                DetectPlayer();
            }
        }

        if (health <= 0)
        {
            Destroy(gameObject);
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

    void GoCheck(GameObject tempPP)
    {
        if (!tempPP.activeInHierarchy && !isChecking) return;

        agent.destination = tempPP.transform.position;

        float distanceToPoint = Vector3.Distance(transform.position, player.position);

        if (distanceToPoint < 0.2f)
        {
            if (patrolPoints.Length > 0)
            {
                agent.destination = patrolPoints[currentPatrolIndex].position;
                isChecking = false;
                tempPP.SetActive(false);
            }
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
                    Warn();
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
            //else return false;
        }

        return false;
    }

    void ChasePlayer()
    {
        agent.destination = player.position;
        agent.speed = speedChase;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
        {
            isChasing = false;
            agent.speed = speedDefault;
            if (patrolPoints.Length > 0)
            {
                agent.destination = patrolPoints[currentPatrolIndex].position;
            }
        }
    }

    void Warn()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, alertRadius);

        detectionRange = 50;

        foreach (Collider collider in nearbyEnemies)
        {
            P_AI_Enemy enemy = collider.GetComponent<P_AI_Enemy>();
            if (enemy != null && enemy != this)
            {
                enemy.OnAlert();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0) health--;

        if (tempPP != null && !isChasing)
        {
            tempPP.transform.position = player.position;
            tempPP.transform.rotation = player.rotation;
            tempPP.SetActive(true);

            isChecking = true;
        }

    }

    void OnAlert()
    {
        isChasing = true;
        detectionRange = 50;
        Invoke(nameof(ResetDetectionRange), 8);
    }

    void ResetDetectionRange()
    {
        detectionRange = 20;
    }

    private void OnDrawGizmosSelected()
    {
        if (isChasing) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
