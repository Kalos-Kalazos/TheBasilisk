using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    private bool isMenuActive = false;
    public PlayerInput playerInput;

    private void Start()
    {
        
        //Asegurarte de que el menu este desactivado al inicio
        if (menu != null)
        {

            menu.SetActive(false);

            
        }

        //Esconder el cursor al inicio del juego
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void CallPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Detecta si el jugador presiona la tecla para pausa (por ejemplo, "P")

            //Cambiar el estado del menu
            isMenuActive = !isMenuActive;

            //Activar o Desactivar el Menu
            menu.SetActive(isMenuActive);

            if (isMenuActive)
            {
                //Mostrar el cursor y detener el juego
                Cursor.visible = true;

                Cursor.lockState = CursorLockMode.None;

                Time.timeScale = 0f; //Pausar el juego

            }
            else
            {
                // Esconder el cursor y reanudar el juego
                Cursor.visible = false;

                Cursor.lockState = CursorLockMode.Locked;

                Time.timeScale = 1f; //reanudar el juego


            }
        }
    }

}
