using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Medikit : MonoBehaviour
{

    public BatterySystem batterySystem; // Referencia al batterySystem
    public ParticleSystem pickupEffect; // Referencia al sistema de particulas (efecto)

    // Este Metodo se llama cuando otro objeto entra en el trigger

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) //Asegurar que es el jugador el que choca
        {

            // Verifica si el sistema de bater�a est� asignado
            if (batterySystem != null)
            {

                batterySystem.AddBattery(); //sumar una pila al sistema
               
                //Activar el efecto de particulas

                if (pickupEffect != null)
                {

                    Instantiate(pickupEffect, transform.position, Quaternion.identity); //Instanciar el efecto en la posici�n del medikit

                }
                Destroy(gameObject); // Elimina el medikit del juego despues de recogerlo
            }
        }

    }
}
