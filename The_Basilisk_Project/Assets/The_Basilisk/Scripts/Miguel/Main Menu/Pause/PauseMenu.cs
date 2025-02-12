using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject gameOverMenu;
    private bool isMenuActive = false;
    public PlayerInput playerInput;
    [SerializeField] P_Mouse_Controller mouseController;
    GameObject playerObject;
    [SerializeField] BatterySystem playerHealth;

    private void Start()
    {
        playerObject = playerInput.gameObject;

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
        if (playerHealth.currentPiles <= 0)
        {
            GameOverBehavoir();
        }

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
                PauseBehavoir();
            }
            else
            {
                ReanudeBehavoir();
            }
        }
    }


    
    public void PauseBehavoir()
    {
        //Mostrar el cursor y detener el juego
        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;

        mouseController.enabled = false;

        playerObject.SetActive(false);

        Time.timeScale = 0f; //Pausar el juego

    }

    public void ReanudeBehavoir()
    {
        // Esconder el cursor y reanudar el juego
        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;

        mouseController.enabled = true;

        playerObject.SetActive(true);

        Time.timeScale = 1f; //reanudar el juego

    }

    public void GameOverBehavoir()
    {
        //Mostrar el cursor y detener el juego
        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;

        mouseController.enabled = false;

        playerObject.SetActive(false);

        Time.timeScale = 0f; //Pausar el juego

        gameOverMenu.SetActive(true);

    }

}
