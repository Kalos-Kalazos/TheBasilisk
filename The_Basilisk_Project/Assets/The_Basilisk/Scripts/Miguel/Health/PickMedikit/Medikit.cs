using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Medikit : MonoBehaviour
{

    public BatterySystem batterySystem; // Referencia al batterySystem


    // Este Metodo se llama cuando otro objeto entra en el trigger

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) //Asegurar que es el jugador el que choca
        {

            batterySystem.AddBattery(); //sumar una pila al sistema

            Destroy(gameObject); // Elimina el medikit del juego despues de recogerlo
        }

    }
}
