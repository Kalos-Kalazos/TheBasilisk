using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Character_Rb : MonoBehaviour
{
    private bool isGrounded;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
