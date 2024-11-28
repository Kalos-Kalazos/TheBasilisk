using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Raycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private GameObject raycastOrigin;  // El GameObject desde donde se lanzará el raycast
    [SerializeField] private Camera cameraToFollow;    // Cámara que determina la dirección del raycast
    [SerializeField] private float raycastDistance = 100f;  // Distancia máxima del raycast

    [Header("Raycast Debug")]
    [SerializeField] private Color rayColor = Color.red; // Color del rayo para depuración

    void Update()
    {
        FireRaycast();
    }

    void FireRaycast()
    {
        // Asegúrate de que raycastOrigin y cameraToFollow estén asignados
        if (raycastOrigin != null && cameraToFollow != null)
        {
            // Obtenemos la dirección hacia la que la cámara está mirando
            Vector3 rayDirection = cameraToFollow.transform.forward;

            // Disparamos el raycast desde el GameObject asignado (raycastOrigin)
            Ray ray = new Ray(raycastOrigin.transform.position, rayDirection);

            // Hacemos el raycast y vemos si colisiona con algo
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                // Si el raycast colisiona con algo, podemos hacer algo con el objeto golpeado
                Debug.Log("Raycast hit: " + hit.collider.name);

                // Aquí podrías agregar lógicas adicionales (como aplicar efectos, activar algo, etc.)
            }

            // Para depuración, dibujamos el rayo en la escena
            Debug.DrawRay(raycastOrigin.transform.position, rayDirection * raycastDistance, rayColor);
        }
        else
        {
            Debug.LogError("Raycast origin or camera is not assigned.");
        }
    }
}