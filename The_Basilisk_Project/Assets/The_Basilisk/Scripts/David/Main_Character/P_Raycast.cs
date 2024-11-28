using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Raycast : MonoBehaviour
{
    [SerializeField] private GameObject raycastOrigin;
    [SerializeField] private Camera cameraToFollow;
    [SerializeField] private float raycastDistance = 100f;

    [SerializeField] private Color rayColor = Color.red;

    private Vector3 raycastDirection;

    private void FixedUpdate()
    {
        PerformRaycast();
    }

    void PerformRaycast()
    {
        if (raycastOrigin != null && cameraToFollow != null)
        {
            raycastDirection = cameraToFollow.transform.forward;

            Ray ray = new Ray(raycastOrigin.transform.position, raycastDirection);
            Debug.Log("Raycast fired from: " + raycastOrigin.transform.position + " in direction: " + raycastDirection);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);
            }

            Debug.DrawRay(raycastOrigin.transform.position, raycastDirection * raycastDistance, rayColor);
        }
        else
        {
            Debug.LogError("Raycast origin or camera is not assigned.");
        }
    }

    public Vector3 GetRaycastDirection()
    {
        return raycastDirection;
    }
}