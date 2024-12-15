using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class P_Enemy_BodyBlock : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 lastPosition;

    void Start()
    {
        // Obtener el NavMeshAgent del enemigo
        agent = GetComponent<NavMeshAgent>();

        // Guardar la posici�n inicial
        lastPosition = transform.position;

        // Asegurarse de que el NavMeshAgent no se vea afectado por colisiones f�sicas
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        // Asegurar que el enemigo se mantenga en la posici�n dictada por el NavMeshAgent
        if (agent != null)
        {
            Vector3 agentPosition = agent.nextPosition;
            transform.position = agentPosition;
        }
        else
        {
            transform.position = lastPosition;
        }

        // Actualizar la �ltima posici�n conocida
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificar si el jugador est� empujando
        if (collision.collider.CompareTag("Player"))
        {
            // Reposicionar el enemigo en caso de empuje
            transform.position = lastPosition;
        }
    }
}
