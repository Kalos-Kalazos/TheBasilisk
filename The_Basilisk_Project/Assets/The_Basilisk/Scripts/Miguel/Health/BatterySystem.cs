using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatterySystem : MonoBehaviour
{
    public Image[] batteryPiles; //Array de imagenes de las pilas

    public int totalPiles = 5; //Numero total de pilas del inicio

    private int currentPiles; //Numero actual de pilas disponibles

    public int batteryCount = 0; //Contador de pilas

    public GameObject gameOverUI; //Asigna tu panel Game Over en el Inspector

    public GameObject manager;

    //public Text batteryText; //UI Text para mostrar el numero de pilas

    // Start is called before the first frame update
    void Start()
    {

        currentPiles = totalPiles; //Inicializamos con todas las pilas
        UpdateBatteryUI(); //actualizar la interfaz del inicio

        manager = FindAnyObjectByType<GameOverManager>().gameObject;
        manager.SetActive(false);

    }

    void Update()
    {

        if (totalPiles <= 0)
        {

            Die();


        }

    }

    void Die() //Metodo que llama cuando el jugador muere
    {

        gameOverUI.SetActive(true); // Activa el game Over UI


        Time.timeScale = 0f; // Pausa el juego
        Time.timeScale = 1f; // Reinicia el juego

    }

    public void TakeDamage(int damage) //Ejemplo de daño al jugador
    {

        totalPiles -= damage;

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

            currentPiles--; UpdateBatteryUI();

            if( currentPiles == 0)
            {

                PlayerDied();

            }

        }


    }

    void PlayerDied()
    {

        Debug.Log("el jugador a muerto");

        manager.SetActive(true);

    }


    // Funcion para reponer pilas si es necesario

  public void RestoreBattery(int amount)
  {

        currentPiles = Mathf.Min(totalPiles, currentPiles + amount); UpdateBatteryUI();


  }

}
