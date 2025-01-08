using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_DoorInteraction : MonoBehaviour
{
    public InputsPlayer playerInputs;

    public bool performed;


    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
            performed = true;
        else if (context.canceled)
            performed = false;
    }
}
