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

    [SerializeField] private float minVerticalAngle = -60f;  // Ángulo mínimo para la rotación en Y
    [SerializeField] private float maxVerticalAngle = 60f;   // Ángulo máximo para la rotación en Y

    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        GetInput();
        ModifyInput();
        MovePlayer();
    }

    // Obtiene la entrada del ratón
    void GetInput()
    {
        xMousePos = Input.GetAxisRaw("Mouse X");
        yMousePos = Input.GetAxisRaw("Mouse Y");
    }

    // Modifica la entrada para aplicar la sensibilidad y el suavizado
    void ModifyInput()
    {
        xMousePos *= sensitivity * smoothing;
        smoothedMousePosX = Mathf.Lerp(smoothedMousePosX, xMousePos, 1f / smoothing);

        yMousePos *= sensitivity * smoothing;
        smoothedMousePosY = Mathf.Lerp(smoothedMousePosY, yMousePos, 1f / smoothing);
    }

    // Mueve la cámara
    void MovePlayer()
    {
        // Rotación horizontal (alrededor del eje Y)
        currentLookingPosX += smoothedMousePosX;
        transform.localRotation = Quaternion.AngleAxis(currentLookingPosX, Vector3.up);

        // Rotación vertical (alrededor del eje X)
        currentLookingPosY -= smoothedMousePosY; // Invertir el movimiento del eje Y para que sea hacia arriba cuando mueves el ratón hacia arriba
        currentLookingPosY = Mathf.Clamp(currentLookingPosY, minVerticalAngle, maxVerticalAngle); // Limita el ángulo de rotación vertical

        Camera.main.transform.localRotation = Quaternion.Euler(currentLookingPosY, 0f, 0f); // Aplica la rotación vertical a la cámara
    }
}