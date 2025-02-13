using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLightSmooth : MonoBehaviour
{
    public Light emergencyLight; //Arrastra aqui la luz en el inspector
    public float minIntensity = 0f; //Intensidad Minima
    public float maxIntensity = 5f; //Intensidad Maxima
    public float speed = 2f; //Velocidad de transicion

    private float targetIntensity;
    private bool increasing = true;

    // Start is called before the first frame update
    void Start()
    {
        if (emergencyLight == null)
            emergencyLight = GetComponent<Light>();
        targetIntensity = maxIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        //Interpola suavemente entre intensidades
        emergencyLight.intensity = Mathf.MoveTowards(emergencyLight.intensity, targetIntensity, speed * Time.deltaTime);

        //cambia la direccion cuando alcanza el limite
        if (Mathf.Approximately(emergencyLight.intensity, targetIntensity))
        {

            targetIntensity = (targetIntensity == maxIntensity) ?
                minIntensity : maxIntensity;
        }
    }
}
