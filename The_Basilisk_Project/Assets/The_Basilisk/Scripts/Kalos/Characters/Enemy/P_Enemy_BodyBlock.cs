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

        // Guardar la posición inicial
        lastPosition = transform.position;

        // Asegurarse de que el NavMeshAgent no se vea afectado por colisiones físicas
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        // Asegurar que el enemigo se mantenga en la posición dictada por el NavMeshAgent
        if (agent != null)
        {
            Vector3 agentPosition = agent.nextPosition;
            transform.position = agentPosition;
        }
        else
        {
            transform.position = lastPosition;
        }

        // Actualizar la última posición conocida
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificar si el jugador está empujando
        if (collision.collider.CompareTag("Player"))
        {
            // Reposicionar el enemigo en caso de empuje
            transform.position = lastPosition;
        }
    }
}
