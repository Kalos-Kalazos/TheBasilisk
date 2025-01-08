using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HookCooldown : MonoBehaviour
{
    public float cooldownTime = 5f; //Tiempo de cooldown en segundos
    private bool isCooldown = false; //Bandera para saber si el hook esta en cooldown

    private float currentCooldownTime; //Tiempo restante del cooldown
    public Text cooldownText; //referencias al Text UI que mostrara el tiempo

    void Start()
    {

        currentCooldownTime = cooldownTime; //Iniciar el tiempo de cooldown
        UpdateCooldownText(); //Inicializar el texto del cooldown


    }

    // Update is called once per frame
    void Update()
    {
        //Si el jugador presiona la tecla para usar el hook
        if(Input.GetKeyDown(KeyCode.Mouse1) && !isCooldown)
        {

            UseHook();

        }

        if (isCooldown) //Si esta en cooldown actualizar el tiempo restante 
        {

            currentCooldownTime -= Time.deltaTime;

            UpdateCooldownText();

            //Si el cooldown se ha acabado resetear
            if (currentCooldownTime <=0)
            {

                isCooldown = false;

                currentCooldownTime = cooldownTime; //Resetear el tiempo de cooldown

                UpdateCooldownText(); //Actualiza el texto a 0

            }

        }

        
    }

    void UseHook()
    {
        //aqui va la logica para usar el hook (Lanzar, atrapar, etc)
        Debug.Log("Hook lanzado");

        
        StartCoroutine(CooldownCoroutine());
    }

    void UpdateCooldownText()
    {

        //mostrar el tiempo  restante en el Text UI (con solo un decimal)

        cooldownText.text = "0" + Mathf.Max(currentCooldownTime, 0).ToString("F1") + "s";


    }

    IEnumerator CooldownCoroutine()
    {
        //activar el cooldown
        isCooldown = true;

        //Esperar el tiempo del cooldown

        yield return new WaitForSeconds(cooldownTime);

        // Una vez pasado el cooldown, habilitar nuevamente el uso del hook

       isCooldown = false;

       Debug.Log("El Hook ya esta listo de nuevo");

    }
}
