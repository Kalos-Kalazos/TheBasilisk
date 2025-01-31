using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonOptions : MonoBehaviour
{
    [SerializeField] private GameObject menuPrincipal; //Arrastra tu objeto MenuPrincipal en el inspector

    public void MostrarMenuPrincipal()
    {

        menuPrincipal.SetActive(true); //Activa el menu principal

    }

    public void OcultarMenuActual(GameObject menuActual)
    {

        menuActual.SetActive(false);//Desactiva el menu actual

    }
}
