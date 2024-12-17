using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100; // Vida del Jugador
    public GameObject gameOverUI; //Asigna tu panel Game Over en el Inspector


    // Update is called once per frame
    void Update()
    {
        
        if(health <= 0)
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

        health -= damage;

    }

}
