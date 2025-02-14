using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsPause : MonoBehaviour
{

    public GameObject PauseMenuUI;//Referencia al panel del de pausa
    public GameObject optionsMenuUI; //Referencia al panel de opciones
    [SerializeField] PauseMenu pauseControl;
    [SerializeField] P_GameManager gameManager;

    private bool isPaused = false;

    private void Start()
    {
        pauseControl = GetComponent<PauseMenu>();
    }

    public void Resume()
    {

        PauseMenuUI.SetActive(false); //Oculta el menu de pausa
        pauseControl.ReanudeBehavoir();


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
        //alternativamente carga la scena del menu principal:

        SceneManager.LoadScene("MainMenu");

    }
    public void RestartScene()
    {

        Time.timeScale = 1f;// Asegurar de que reanuda el tiempo
        //alternativamente carga la scena de nuevo:

        SceneManager.LoadScene(gameManager.actualScene);

    }

    public void QuitGame()
    {

        Debug.Log("Saliendo del Juego...");//funciona al copilar el juego
        Application.Quit();
    }

}
