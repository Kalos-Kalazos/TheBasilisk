using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonMenu : MonoBehaviour
{
   
    public void StartGame()
    {

        SceneManager.LoadScene("GameScene"); //Reemplaza "GameScene" con el nombre exacto de tu escena del juego


    }

    public void ExitGame()
    {

        Application.Quit(); //cierra la aplicacion

        //Solo para la depuracion en el editor de unity
        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
        #endif

    }

}
