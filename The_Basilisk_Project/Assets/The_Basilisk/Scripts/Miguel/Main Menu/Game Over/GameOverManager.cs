using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {

        Time.timeScale = 1f; // Reanuda el tiempo

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Reinicia la escena actual

    }

    public void QuitGame()
    {

        Application.Quit(); // Cierra el juego

    }
}
