using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour
{

    public GameObject controlImagen; // La imagen de los controles

    // Start is called before the first frame update
    void Start()
    {
        
        //Asegurarse de que la imagen esta oculta al inicio
        if(controlImagen != null)
        {

            controlImagen.SetActive(false);

        }


    }

    // Metodo para ocultar la imagen de los controles
    public void ShowControls()
    {

        if(controlImagen != null)
        {

            controlImagen.SetActive(true);

        }



    }


    // Metodo para Ocultar la imagen de los controles
    public void HideControls()
    {

        if(controlImagen != null)
        {

            controlImagen.SetActive(false);

        }

    }

    
}
