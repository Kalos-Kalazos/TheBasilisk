using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    public GameObject arrowSprite; //El objeto de la flecha

    //Metodo que se ejecutara al entrar el puntero en el botton
    public void ShowArrow()
    {

        if (arrowSprite != null)
        {

            arrowSprite.SetActive(true);

        }


    }

    //Metodo que se ejecutara al salir el puntero del boton
    public void HideArrow()
    {

        if (arrowSprite != null)
        {

            arrowSprite.SetActive(false);

        }

    }
}
