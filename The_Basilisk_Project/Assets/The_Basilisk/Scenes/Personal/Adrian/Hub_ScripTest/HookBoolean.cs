using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookBoolean : MonoBehaviour
{

    private GameObject pelota;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        pelota.SetActive(true);
    }

}
