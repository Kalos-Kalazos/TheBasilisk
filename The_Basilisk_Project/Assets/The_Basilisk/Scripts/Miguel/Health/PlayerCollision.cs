using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    public BatterySystem batterySystem; // Referencia al sistema de bateria

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Llamamos al metodo loseBattery() cuando el jugador es atacado por un enemigo.

            batterySystem.LoseBattery();
        }

    }
    
}
