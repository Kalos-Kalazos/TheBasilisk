using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsPause : MonoBehaviour
{

    public GameObject PauseMenuUI;//Referencia al panel del de pausa
    public GameObject optionsMenuUI; //Referencia al panel de opciones
    public GameObject mainMenuUI; //Referencia al panel del menu principal

    private bool isPaused = false;

    public void Resume()
    {

        PauseMenuUI.SetActive(false); //Oculta el menu de pausa
        Time.timeScale = 1f; //Reanuda el tiempo
        isPaused = false;

    }

    public void OpenOptions()
    {

        optionsMenuUI.SetActive(true); //activa el menu de opciones
        PauseMenuUI.SetActive(false); //Oculta el menu de pausa
        

    }

    public void BackToPauseMenu()
    {

        optionsMenuUI.SetActive(false); //Oculta el menu de opciones
        PauseMenuUI.SetActive(true); // vuelve al menu de pausa

    }

    public void LoadMainMenu()
    {

        Time.timeScale = 1f;// Asegurar de que reanuda el tiempo

        mainMenuUI.SetActive(true); //Muestra el Menu principal
        //alternativamente carga la scena del menu principal:

        SceneManager.LoadScene("MainMenu");

    }

    public void QuitGame()
    {

        Debug.Log("Saliendo del Juego...");//funciona al copilar el juego

    }

}
