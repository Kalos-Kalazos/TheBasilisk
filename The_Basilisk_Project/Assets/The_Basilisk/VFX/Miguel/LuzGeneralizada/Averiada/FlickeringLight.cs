using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light flickerLight;
    public float minIntensity = 0.2f; //Intensidad Minima
    public float maxIntensity = 2.0f; //Intensidad Maxima
    public float flickerSpeed = 5.0f; //Velocidad de parpadeo
    public float noiseScale = 0.1f; //Escala de ruido

    private float randomOffset; //Para variar el ruido entre luces

    // Start is called before the first frame update
    void Start()
    {
        if (flickerLight == null)
        {

            flickerLight = GetComponent<Light>();

            randomOffset =
            Random.Range(0f, 100f);

        }
    }

    // Update is called once per frame
    void Update()
    {

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        flickerLight.intensity = intensity;

    }
}
