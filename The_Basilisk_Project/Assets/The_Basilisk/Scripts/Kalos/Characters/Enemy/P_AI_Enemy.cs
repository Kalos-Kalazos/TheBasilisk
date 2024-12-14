using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using static P_WeaponController;

public class P_AI_Enemy : MonoBehaviour
{
    [Header("=== Enemy Status Settings ===")]
    [SerializeField] float health = 100;
    [SerializeField] float speedChase = 7;
    [SerializeField] float speedDefault = 3.5f;
    [SerializeField] int damage = 2;
    [SerializeField] EnemyState currentState;

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

    [Header("=== Enemy Attack Settings ===")]
    [SerializeField] float lastAttackTime;
    [SerializeField] float attackCD;
    [SerializeField] float attackRange;

    [Header("=== Enemy Animation Settings ===")]
    Animator animator;
    bool isPausing = false;

    [SerializeField] GameObject ui;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;

        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
        Iddle
    }


    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                DetectPlayer();
                break; 

            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Attacking:
                AttackPlayer();
                break;
            case EnemyState.Iddle:
                IdleBehavior();
                break;
        }

        /*
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
        }*/
    }

    #region -- Burn Logic
    private Coroutine burnCoroutine;

    public void ApplyBurnEffect(float burnDuration, float burnDamagePerSecond)
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);  // Detener cualquier efecto anterior
        }

        burnCoroutine = StartCoroutine(BurnEnemy(burnDuration, burnDamagePerSecond));
    }

    private IEnumerator BurnEnemy(float burnDuration, float burnDamagePerSecond)
    {
        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {
            TakeDamage(burnDamagePerSecond * Time.deltaTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
    #endregion

    void Patrol()
    {
        if (isPausing) return;

        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            StartCoroutine(PauseAtPatrolPoint());
        }
    }
    private IEnumerator PauseAtPatrolPoint()
    {
        isPausing = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(1, 5));

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.destination = patrolPoints[currentPatrolIndex].position;

        agent.isStopped = false;
        isPausing = false;
    }

    /*
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
    */

    void IdleBehavior()
    {
        agent.isStopped = true;
        animator.SetTrigger("Iddle");
               
        Invoke(nameof(ReturnToPatrol), 3f);
    }

    void ReturnToPatrol()
    {
        if (currentState == EnemyState.Iddle)
        {
            currentState = EnemyState.Patrolling;
            agent.isStopped = false;
            if (patrolPoints.Length > 0)
            {
                agent.destination = patrolPoints[currentPatrolIndex].position;
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

            if (angleToTarget <= maxDetectionAngle && HasVision(player))
            {
               currentState = EnemyState.Chasing;
               Warn();
            }
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
        agent.isStopped = false;
        agent.destination = player.position;
        agent.speed = speedChase;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Iddle;
            agent.speed = speedDefault;
        }
    }

    void AttackPlayer()
    {
        agent.isStopped = true;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 7.5f);

        if (Time.time - lastAttackTime >= attackCD)
        {
            lastAttackTime = Time.time;
            //Animation
            animator.SetTrigger("Attack");
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = EnemyState.Chasing;
            agent.isStopped = false;
        }
    }

    public void ApplyDamageToPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange - 0.1f)
        {
            BatterySystem batterySystem = ui.GetComponentInChildren<BatterySystem>();
            if (batterySystem != null)
            {
                batterySystem.LoseBattery();
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


    public void TakeDamage(float damage)
    {
        if (damage > 0)
        {
            Debug.Log("Entra " + damage + " de daño");
            health -= damage;
        }
        /*        
        if (tempPP != null && !isChasing)
        {
            tempPP.transform.position = player.position;
            tempPP.transform.rotation = player.rotation;
            tempPP.SetActive(true);

            isChecking = true;
        }
        */
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnAlert()
    {
        currentState = EnemyState.Chasing;
        detectionRange = 50;
        Invoke(nameof(ResetDetectionRange), 8);
    }

    void ResetDetectionRange()
    {
        detectionRange = 20;
    }

    private void OnDrawGizmosSelected()
    {
        if (currentState == EnemyState.Chasing) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
