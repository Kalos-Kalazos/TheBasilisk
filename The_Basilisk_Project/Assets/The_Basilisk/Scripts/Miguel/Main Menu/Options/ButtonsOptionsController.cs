using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsOptionsController : MonoBehaviour
{
    public Button backButton;
    public Button ControlButton;
    public GameObject controlsImage;

    // Start is called before the first frame update
    void Start()
    {

        //Asignar funciones a los botones 
        backButton.onClick.AddListener(BackToMainMenu);

        ControlButton.onClick.AddListener(ToggleControlsImage);

        //Asegurarse de que la imagen de controles oculta al inicio
        if(controlsImage != null)
        {

            controlsImage.SetActive(false);

        }

    }

    void BackToMainMenu()
    {

        SceneManager.LoadScene("MainMenu");

    }

    void ToggleControlsImage()
    {

        if (controlsImage != null)
        {

            controlsImage.SetActive(!controlsImage.activeSelf); //Cambia el estado activo

        }
    }
}
