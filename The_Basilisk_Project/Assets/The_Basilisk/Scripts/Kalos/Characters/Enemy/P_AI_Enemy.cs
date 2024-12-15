using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Search;
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


    #region --Search on sound heard

    [SerializeField] float searchRadius = 10f;
    [SerializeField] float searchTime = 5f;
    bool isSearching = false;

    private void OnEnable()
    {
        P_GameManager.OnGunshot += OnGunshotHeard;
    }

    private void OnDisable()
    {
        P_GameManager.OnGunshot -= OnGunshotHeard;
    }

    public void OnGunshotHeard(Vector3 gunshotPosition, string sourceTag)
    {
        if (Vector3.Distance(transform.position, gunshotPosition) <= searchRadius)
        {
            Debug.Log($"{gameObject.name} heard a gunshot");

            if (!isSearching && !player.GetComponent<P_Character_HookSwing>().activeGrapple && !player.GetComponent<P_Character_HookSwing>().swinging && currentState != EnemyState.Chasing)
            {
                StartCoroutine(SearchArea(gunshotPosition));
            }
        }
    }

    IEnumerator SearchArea(Vector3 gunshotPosition)
    {
        isSearching = true;
        if (agent == null || !agent.isActiveAndEnabled) yield break;
        agent.destination = gunshotPosition; 
        Debug.Log($"{agent.destination} destination");

        // Search
        float searchDuration = Random.Range(5, 10);
        float timeElapsed = 0f;

        while (timeElapsed < searchDuration)
        {
            if (!gameObject.activeInHierarchy) yield break;

            // Look
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0; //horizontal
            Vector3 lookPosition = transform.position + randomDirection;

            Quaternion lookRotation = Quaternion.LookRotation((lookPosition - transform.position).normalized);
            float rotationTime = 1f;
            float rotationElapsed = 0f;

            while (rotationElapsed < rotationTime)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                rotationElapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(1, 3));

            timeElapsed += rotationTime + Random.Range(1, 3);
        }

        if (agent != null)
        {
            currentState = EnemyState.Patrolling;
        }
        isSearching = false;
    }
    #endregion


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
                agent.speed = speedDefault;
                break; 

            case EnemyState.Chasing:
                ChasePlayer();
                agent.speed = speedChase;
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
            StopCoroutine(burnCoroutine);
        }

        burnCoroutine = StartCoroutine(BurnEnemy(burnDuration, burnDamagePerSecond));
    }

    private IEnumerator BurnEnemy(float burnDuration, float burnDamagePerSecond)
    {
        if (agent == null) yield break;

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

        yield return new WaitForSeconds(Random.Range(1, 8));

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

        detectionRange += 5;

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
            if (agent != null)
            {
                agent.enabled = false;  // Desactiva el agente antes de destruir
            }
            Destroy(gameObject);
        }
    }

    void OnAlert()
    {
        currentState = EnemyState.Chasing;
        detectionRange += 5;
        Invoke(nameof(ResetDetectionRange), 8);
    }

    void ResetDetectionRange()
    {
        detectionRange -= 5;
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
