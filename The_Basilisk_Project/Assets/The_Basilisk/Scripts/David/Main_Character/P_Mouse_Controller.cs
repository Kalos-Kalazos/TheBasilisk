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

    [SerializeField] private float minVerticalAngle = -60f;  // �ngulo m�nimo para la rotaci�n en Y
    [SerializeField] private float maxVerticalAngle = 60f;   // �ngulo m�ximo para la rotaci�n en Y

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

    // Obtiene la entrada del rat�n
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

    // Mueve la c�mara
    void MovePlayer()
    {
        // Rotaci�n horizontal (alrededor del eje Y)
        currentLookingPosX += smoothedMousePosX;
        transform.localRotation = Quaternion.AngleAxis(currentLookingPosX, Vector3.up);

        // Rotaci�n vertical (alrededor del eje X)
        currentLookingPosY -= smoothedMousePosY; // Invertir el movimiento del eje Y para que sea hacia arriba cuando mueves el rat�n hacia arriba
        currentLookingPosY = Mathf.Clamp(currentLookingPosY, minVerticalAngle, maxVerticalAngle); // Limita el �ngulo de rotaci�n vertical

        Camera.main.transform.localRotation = Quaternion.Euler(currentLookingPosY, 0f, 0f); // Aplica la rotaci�n vertical a la c�mara
    }
}