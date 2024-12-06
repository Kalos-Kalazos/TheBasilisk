using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatterySystem : MonoBehaviour
{
    public Image[] batteryPiles; //Array de imagenes de las pilas

    public int totalPiles = 5; //Numero total de pilas del inicio

    private int currentPiles; //Numero actual de pilas disponibles

    // Start is called before the first frame update
    void Start()
    {

        currentPiles = totalPiles; //Inicializamos con todas las pilas
        UpdateBatteryUI();


    }

    void UpdateBatteryUI()
    {
        // Muestra las pilas que estan activas
        for (int i = 0; i < batteryPiles.Length; i++)
        {

            if(i < currentPiles)
            {
                batteryPiles[i].enabled = true;//Mostrar la pila

            }
            else
            {

                batteryPiles[i].enabled = false;//Ocultar la pila

            }


        }

    }

    //Funcion para llamar cuando el jugador recibe un ataque

    public void LoseBattery()
    {

        if (currentPiles > 0)
        {

            currentPiles--; UpdateBatteryUI();


        }


    }

    // Funcion para reponer pilas si es necesario

  public void RestoreBattery(int amount)
  {

        currentPiles = Mathf.Min(totalPiles, currentPiles + amount); UpdateBatteryUI();


  }

}
