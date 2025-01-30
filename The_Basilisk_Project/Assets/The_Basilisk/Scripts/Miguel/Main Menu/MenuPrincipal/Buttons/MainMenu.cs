using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void StartGame() //Metodo para iniciar el juego
    {
        //Cambiar "GameScene" por el nombre de tu scena principal

        SceneManager.LoadScene("Miguel_Scene");
    }

    public void OpenOptions()
    {

        Debug.Log("Options Menu Clicked"); //Aqui podras cargar otra escena o mostrar un submenu
    }
    
    public void ExitGame()
    {

        Debug.Log("Exiting game...");
        Application.Quit();

    }
}
