using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatterySystem : MonoBehaviour
{
    public Image[] batteryPiles; //Array de imagenes de las pilas

    public int totalPiles = 5; //Numero total de pilas del inicio

    public int currentPiles; //Numero actual de pilas disponibles

    public int batteryCount = 0; //Contador de pilas


    //public Text batteryText; //UI Text para mostrar el numero de pilas

    // Start is called before the first frame update
    void Start()
    {

        currentPiles = totalPiles; //Inicializamos con todas las pilas
        UpdateBatteryUI(); //actualizar la interfaz del inicio


    }

    public void AddBattery()
    {

        currentPiles++;
        UpdateBatteryUI();// actualiza la UI cuando recoge una pila


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

        //batteryText.text = "PickUp_Medikit:" + batteryCount.ToString();


    }

    //Funcion para llamar cuando el jugador recibe un ataque

    public void LoseBattery()
    {
        if (currentPiles > 0)
        {
            currentPiles--;
            UpdateBatteryUI();

            if (currentPiles == 0)
            {
                PlayerDied();
            }
        }
    }

    void PlayerDied()
    {
        Debug.Log("El jugador ha muerto.");
    }

    // Funcion para reponer pilas si es necesario

    public void RestoreBattery(int amount)
  {

        currentPiles = Mathf.Min(totalPiles, currentPiles + amount); UpdateBatteryUI();


  }

}
