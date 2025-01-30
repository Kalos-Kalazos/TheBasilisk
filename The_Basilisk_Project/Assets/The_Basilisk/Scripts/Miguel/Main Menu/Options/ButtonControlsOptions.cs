using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControlsOptions : MonoBehaviour
{
    public Button buttonControllerOptions; //Referencia al boton
    public GameObject imageObject; //Referencia a la imagen en el camvas
    private bool isImageActive = false; //Estado inicial de la imagen

    // Start is called before the first frame update
    void Start()
    {

        //Asegurate de que la imagen este desactivada al inicio
        imageObject.SetActive(false);

        //Asigna el evento al boton
        buttonControllerOptions.onClick.AddListener(ToggleImage);

        Button imageButton = imageObject.GetComponent<Button>();
        if (imageButton != null)
        {

            imageButton.onClick.AddListener(HideImage);
            
        }

    }

    void ToggleImage()
    {

        isImageActive = !isImageActive; //Cambia el estado

        imageObject.SetActive(isImageActive); //Activa o desactiva la imagen

        //Si la imagen esta activa, añade un evento de clic para ocultarla
        if (isImageActive)
        {

            imageObject.GetComponent<Button>().onClick.AddListener(HideImage);

        }
        else
        {

            imageObject.GetComponent<Button>().onClick.RemoveListener(HideImage);

        }
    }

    void HideImage()
    {

        isImageActive = false;

        imageObject.SetActive(false);

        imageObject.GetComponent<Button>().onClick.RemoveListener(HideImage);

    }
    
}
