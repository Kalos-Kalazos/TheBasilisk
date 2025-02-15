using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenuUI; //Referencia al panel de opciones

    private void Update()
    {
        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;
    }
    public void StartGame() //Metodo para iniciar el juego
    {
        //Cambiar "GameScene" por el nombre de tu scena principal

        SceneManager.LoadScene("Animacion_Start");
    }

    public void OpenOptions()
    {
        optionsMenuUI.SetActive(true); //activa el menu de opciones
    }
    public void CloseOptions()
    {
        optionsMenuUI.SetActive(false); //oculta el menu de opciones
    }

    public void ExitGame()
    {

        Debug.Log("Exiting game...");
        Application.Quit();

    }
}
