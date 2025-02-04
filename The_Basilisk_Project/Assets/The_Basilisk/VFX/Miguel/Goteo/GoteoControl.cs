using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoteoControl : MonoBehaviour
{
    public ParticleSystem goteo;
    public float intervalo = 1f;


    // Start is called before the first frame update
    void Start()
    {

        InvokeRepeating("EmitirGota", 0, intervalo);

    }

    void EmitirGota()
    {

        if (goteo !=null)
        {
            goteo.Emit(Random.Range(1, 2)); //Emite entre 1 y 2 gotas
        }

    }
}
