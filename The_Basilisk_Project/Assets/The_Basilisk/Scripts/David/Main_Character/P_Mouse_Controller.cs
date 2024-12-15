using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Mouse_Controller : MonoBehaviour
{
    [SerializeField] public float sensitivity = 1.5f;
    [SerializeField] public float smoothing = 1.5f;

    [SerializeField] private float xMousePos;
    [SerializeField] private float yMousePos;

    [SerializeField] private float smoothedMousePosX;
    [SerializeField] private float smoothedMousePosY;

    [SerializeField] private float currentLookingPosX;
    [SerializeField] private float currentLookingPosY;

    [SerializeField] private float minVerticalAngle = -60f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("References")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private Transform cameraTransform;

    void Start()
    {
        if (virtualCamera != null)
        {
            cameraTransform = virtualCamera.transform;
        }
        currentLookingPosX = transform.localEulerAngles.y;
        currentLookingPosY = cameraTransform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        GetInput();
        ModifyInput();
        MovePlayer();
    }

    void GetInput()
    {
        xMousePos = Input.GetAxisRaw("Mouse X");
        yMousePos = Input.GetAxisRaw("Mouse Y");
    }

    void ModifyInput()
    {
        xMousePos *= sensitivity * smoothing;
        smoothedMousePosX = Mathf.Lerp(smoothedMousePosX, xMousePos, 1f / smoothing);

        yMousePos *= sensitivity * smoothing;
        smoothedMousePosY = Mathf.Lerp(smoothedMousePosY, yMousePos, 1f / smoothing);
    }

    void MovePlayer()
    {
        currentLookingPosX += smoothedMousePosX;
        transform.localRotation = Quaternion.AngleAxis(currentLookingPosX, Vector3.up);

        currentLookingPosY -= smoothedMousePosY;
        currentLookingPosY = Mathf.Clamp(currentLookingPosY, minVerticalAngle, maxVerticalAngle);

        cameraTransform.localRotation = Quaternion.Euler(currentLookingPosY, 0f, 0f);
    }
}