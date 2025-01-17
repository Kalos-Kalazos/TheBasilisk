using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; //Asigna aqui tu canvas de la UI
    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {

        // Detecta si el jugador presiona la tecla para pausa (por ejemplo, "Escable")

        if (Input.GetKeyDown(KeyCode.P))
        {

            if (isPaused)
            {

                ResumeGame();

            }
            else
            {

                PauseGame();

            }

        }

    }

    public void PauseGame()
    {

        //Activa el cambas del menu de pausa 
        pauseMenuUI.SetActive(true);

        //Pausa el tiempo del juego
        Time.timeScale = 0f;

        // Muestra y desbloquea el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void ResumeGame()
    {

        //Desactiva el Canvas del Menu pausa
        pauseMenuUI.SetActive(false);
        //Restaura el tiempo del juego
        Time.timeScale = 1f;
        // Oculta y bloquea el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }


    public void OnResumeButtonClicked()
    {




    }


}
