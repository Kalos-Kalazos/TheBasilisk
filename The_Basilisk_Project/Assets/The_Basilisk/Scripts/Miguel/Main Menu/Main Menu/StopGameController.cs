using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopGameController : MonoBehaviour
{

    public GameObject PauseMenuUI; //Arrastra aqui el menu desde el Inspector.

    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {

        //Detectar si se presiona la tecla Escape o cualquier otra para pausar

        if (Input.GetKeyDown(KeyCode.Escape))
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

        PauseMenuUI.SetActive(true); // Mostrar el menu.
        Time.timeScale = 0f; // Detener el tiempo
        isPaused = true;

    }

    public void ResumeGame()
    {

        PauseMenuUI.SetActive(false); //Ocultar el menu.
        Time.timeScale = 1f; //Reanudar el tiempo
        isPaused = false;


    }

    public void QuitGame()
    {

        Debug.Log("Saliendo del juego");
        Application.Quit();

    }
}
