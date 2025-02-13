using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasEmitter : MonoBehaviour
{
    public ParticleSystem gasParticleSystem;
    public float emissionInterval = 3f; //Tiempo entre emisiones

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EmitGasPeriodically());
    }

    IEnumerator EmitGasPeriodically()
    {

        while (true)
        {

            gasParticleSystem.Play(); //Activar Particulas
            yield return new WaitForSeconds(gasParticleSystem.main.duration); // Espera la duracion del sistema

            gasParticleSystem.Stop(); // Detener particulas
            yield return new WaitForSeconds(emissionInterval); //Esperar antes de la siguiente emision

        }
    }
}
