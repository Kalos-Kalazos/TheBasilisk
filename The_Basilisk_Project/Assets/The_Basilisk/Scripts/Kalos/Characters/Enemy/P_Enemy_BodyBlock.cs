using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class P_Enemy_BodyBlock : MonoBehaviour
{
    NavMeshAgent agent;
    P_AI_Enemy enemyControl;
    [SerializeField] bool isStatic;
    /*
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyControl = GetComponent<P_AI_Enemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);

            if(!isStatic)
                StaticEnemy();
        }

    }

    void StaticEnemy()
    {
        Debug.Log("Enemy Stopped");

        agent.enabled = false;
        isStatic = true;
    }


    void DynamicEnemy()
    {
        Debug.Log("Enemy not stopped");

        agent.enabled = true;
        isStatic = false;
        gameObject.isStatic = isStatic;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isStatic)
        {
            DynamicEnemy();
        }
    }*/
}
