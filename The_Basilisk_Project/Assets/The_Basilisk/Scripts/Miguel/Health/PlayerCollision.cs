using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    public BatterySystem batterySystem; // Referencia al sistema de bateria

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && batterySystem.currentPiles > 0)
        {
            //Llamamos al metodo loseBattery() cuando el jugador es atacado por un enemigo.

            batterySystem.LoseBattery();
        }

    }
    
}
