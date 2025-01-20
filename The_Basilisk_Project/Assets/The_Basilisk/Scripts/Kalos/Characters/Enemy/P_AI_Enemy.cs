using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class P_AI_Enemy : MonoBehaviour
{
    [Header("=== Enemy Status Settings ===")]
    [SerializeField] float health = 100;
    [SerializeField] float speedChase = 7;
    [SerializeField] float speedDefault = 3.5f;
    [SerializeField] int damage = 2;
    public EnemyState currentState;

    [Header("=== Enemy Patrol Settings ===")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] int currentPatrolIndex;
    [SerializeField] NavMeshAgent agent;

    [Header("=== Enemy Detection Settings ===")]
    [SerializeField] float detectionRange = 15;
    [SerializeField] float detectionCloseRange = 5;
    [SerializeField] Transform player;
    [SerializeField] GameObject tempPP;
    [SerializeField] bool isChasing = false;
    [SerializeField] bool isChecking = false;
    [SerializeField] bool playerIsCrouched = false;
    [SerializeField] bool playerIsWalking = false;
    [SerializeField] float maxDetectionAngle = 45;
    [SerializeField] float alertRadius = 40;

    [Header("=== Enemy Attack Settings ===")]
    [SerializeField] float attackRate;
    [SerializeField] float attackCD;
    [SerializeField] float attackRange;

    [Header("=== Enemy Animation Settings ===")]
    Animator animator;
    bool isPausing = false;

    [SerializeField] P_GameManager gm;

    [SerializeField] GameObject ui;

    float timeElapsedChase = 0;

    #region --Search on sound heard
    Coroutine currentCoroutineState;
    [SerializeField] float searchRadius = 20;
    [SerializeField] float searchTime = 15;
    bool isSearching = false;
    [SerializeField] BatterySystem batterySystem;
    [SerializeField] LayerMask EnemyLayer;
    [SerializeField] LayerMask PlayerLayer;

    [SerializeField] GameObject vfxBlood;
    public Transform pivotVFX;
    bool died;

    public void OnGunshotHeard(Vector3 gunshotPosition, string sourceTag)
    {
        if (sourceTag == null || gunshotPosition == null)
        {
            Debug.LogError("GunshotHeard Error");
            return;
        }

        if (Vector3.Distance(transform.position, gunshotPosition) <= searchRadius)
        {
            if (!player.GetComponent<P_Character_HookSwing>().activeGrapple && !player.GetComponent<P_Character_HookSwing>().swinging && currentState != EnemyState.Chasing)
            {
                if (currentCoroutineState != null)
                {
                    StopCoroutine(currentCoroutineState);
                    isSearching = false;
                }

                currentCoroutineState = StartCoroutine(SearchArea(gunshotPosition));
            }
        }
    }

    IEnumerator SearchArea(Vector3 gunshotPosition)
    {
        if (agent == null || !agent.enabled) yield break;
        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.destination = gunshotPosition;
        }
        isSearching = true;

        // Search
        float timeElapsed = 0;

        while (timeElapsed < searchTime)
        {
            if (currentState == EnemyState.Chasing)
            {
                isSearching = false;
                yield break;
            }

            timeElapsed += Time.deltaTime;

            yield return null; // Evitar bloqueos
        }

        currentState = EnemyState.Iddle;
        isSearching = false;

        if (patrolPoints.Length > 0 && !agent.enabled)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }
    }
    #endregion


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = FindAnyObjectByType<P_Mouse_Controller>().transform;
        gm = FindAnyObjectByType<P_GameManager>();
        batterySystem = ui.GetComponentInChildren<BatterySystem>();
        died = false;

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
        Iddle,
        Dead
    }


    void Update()
    {
        DetectPlayer();

        playerIsCrouched = player.GetComponent<P_Character_Move_v2>().isCrouched;
        playerIsWalking = player.GetComponent<P_Character_Move_v2>().isWalking;

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

                #region --TimeSearching
                while (timeElapsedChase < searchTime)
                {
                    timeElapsedChase += Time.deltaTime;
                }
                #endregion

                break;

            case EnemyState.Attacking:
                AttackPlayer();
                break;

            case EnemyState.Iddle:
                IdleBehavior();
                break;

            case EnemyState.Dead:
                if(!died)
                    Death();
                break;
        }

        #region --Attack Cooldown
        if (attackCD > 0)
        {
            attackCD -= Time.deltaTime;
        }

        if (attackCD < 0) attackCD = 0;
        #endregion

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


        #region --PJ on Stealth
        if (player.GetComponent<P_Character_Move_v2>().isCrouched)
        {
            detectionRange = 7;
            maxDetectionAngle = 30;
            searchTime = 10;
        }
        else
        {
            detectionRange = 14;
            maxDetectionAngle = 60;
            searchTime = 15;
        }
        #endregion
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

        if (agent.enabled)
        {
            agent.isStopped = false;

            if (!agent.pathPending && agent.remainingDistance < 1f)
            {
                StartCoroutine(PauseAtPatrolPoint());
            }            
        }
    }

    private IEnumerator PauseAtPatrolPoint()
    {
        if (currentState == EnemyState.Dead) yield break;
        if (!agent.enabled) yield break;

        isPausing = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(5);

        if (!isSearching && agent.enabled)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }

        if (agent.enabled) agent.isStopped = false;
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
        if (currentState == EnemyState.Dead) return;

        if (agent.enabled) agent.isStopped = true; 
        
        animator.SetTrigger("Iddle");
               
        Invoke(nameof(ReturnToPatrol), 2f);
    }

    void ReturnToPatrol()
    {
        if (currentState == EnemyState.Dead) return;

        if (currentState == EnemyState.Iddle && agent.enabled)
        {
            agent.isStopped = false;
            currentState = EnemyState.Patrolling;
            agent.destination = patrolPoints[currentPatrolIndex].position;
        }
    }


    void DetectPlayer()
    {
        if (currentState == EnemyState.Dead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToTarget <= maxDetectionAngle && HasVision(player))
            {
                timeElapsedChase = 0;
                Warn();
                currentState = EnemyState.Chasing;
            }
        }

        if (distanceToPlayer <= detectionCloseRange && !playerIsCrouched && playerIsWalking)
        {
            timeElapsedChase = 0;
            Warn();
            currentState = EnemyState.Chasing;
        }
    }

    bool HasVision(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToTarget, out hit, detectionRange, PlayerLayer))
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
        if (currentState == EnemyState.Dead) return;

        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.destination = player.position;
            agent.speed = speedChase;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distanceToPlayer > detectionRange && timeElapsedChase >= searchTime)
        {
            currentState = EnemyState.Iddle;
        }
    }

    void AttackPlayer()
    {
        if (currentState == EnemyState.Dead) return;
        if (agent.enabled) agent.isStopped = true;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 7.5f);

        if (attackCD <= 0)
        {
            Debug.Log("Enemy Attack");
            attackCD = attackRate;
            //Animation
            animator.SetTrigger("Attack");

            TryApplyDamage();
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = EnemyState.Chasing;

            if (agent.enabled) agent.isStopped = false;
        }
    }
    void TryApplyDamage()
    {
        if (currentState == EnemyState.Dead) return;

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Hit!");
            batterySystem.LoseBattery();
        }
        else Debug.Log("Attack missed!");
    }

    void Warn()
    {
        if (currentState == EnemyState.Dead) return;

        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, alertRadius, EnemyLayer);


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
        if (currentState == EnemyState.Dead) return;

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
            currentState = EnemyState.Dead;
        }
    }

    void Death()
    {
        died = true;
        if (agent != null)
        {
            agent.enabled = false;  // Desactiva el agente antes de destruir
        }
        gm.deadCount++;
        gm.CheckToOpen();
        GameObject hitBloodLoopTemp = Instantiate(vfxBlood, pivotVFX.position, pivotVFX.rotation);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 3.0f;
        rb.AddForce(-transform.forward, ForceMode.Impulse);
        Destroy(hitBloodLoopTemp, 1.5f);
        Destroy(gameObject, 2.5f);
    }

    void OnAlert()
    {
        if (currentState == EnemyState.Dead) return;

        currentState = EnemyState.Chasing;
    }

    private void OnDrawGizmosSelected()
    {
        if (currentState == EnemyState.Chasing) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionCloseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
