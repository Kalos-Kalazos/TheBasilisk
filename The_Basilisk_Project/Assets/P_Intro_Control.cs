using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class P_Intro_Control : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "NombreDeLaSiguienteEscena"; // Cambia esto por el nombre de la escena

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Suscribirse al evento que se ejecuta cuando el video termina
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
