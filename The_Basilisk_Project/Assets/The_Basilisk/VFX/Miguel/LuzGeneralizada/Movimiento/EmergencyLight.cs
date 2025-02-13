using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyLight : MonoBehaviour
{
    public float rotationSpeed = 100f; //Velocidad de giro

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

    }
}
