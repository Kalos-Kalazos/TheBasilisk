using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Raycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private GameObject raycastOrigin;  // El GameObject desde donde se lanzar� el raycast
    [SerializeField] private Camera cameraToFollow;    // C�mara que determina la direcci�n del raycast
    [SerializeField] private float raycastDistance = 100f;  // Distancia m�xima del raycast

    [Header("Raycast Debug")]
    [SerializeField] private Color rayColor = Color.red; // Color del rayo para depuraci�n

    void Update()
    {
        FireRaycast();
    }

    void FireRaycast()
    {
        // Aseg�rate de que raycastOrigin y cameraToFollow est�n asignados
        if (raycastOrigin != null && cameraToFollow != null)
        {
            // Obtenemos la direcci�n hacia la que la c�mara est� mirando
            Vector3 rayDirection = cameraToFollow.transform.forward;

            // Disparamos el raycast desde el GameObject asignado (raycastOrigin)
            Ray ray = new Ray(raycastOrigin.transform.position, rayDirection);

            // Hacemos el raycast y vemos si colisiona con algo
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                // Si el raycast colisiona con algo, podemos hacer algo con el objeto golpeado
                Debug.Log("Raycast hit: " + hit.collider.name);

                // Aqu� podr�as agregar l�gicas adicionales (como aplicar efectos, activar algo, etc.)
            }

            // Para depuraci�n, dibujamos el rayo en la escena
            Debug.DrawRay(raycastOrigin.transform.position, rayDirection * raycastDistance, rayColor);
        }
        else
        {
            Debug.LogError("Raycast origin or camera is not assigned.");
        }
    }
}