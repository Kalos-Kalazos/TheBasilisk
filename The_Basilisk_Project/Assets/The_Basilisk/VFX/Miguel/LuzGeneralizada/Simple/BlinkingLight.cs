using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    public Light emergencyLight;//Arrastrar la luz en el inspector
    public float blickInterval = 0.5f;//Tirmpo entre cada parpadeo

    // Start is called before the first frame update
    void Start()
    {

        if (emergencyLight == null)
            emergencyLight = GetComponent<Light>();

        InvokeRepeating("ToggleLight", 0f, blickInterval);

    }

    void ToggleLight()
    {

        emergencyLight.enabled = !emergencyLight.enabled;

    }

}
