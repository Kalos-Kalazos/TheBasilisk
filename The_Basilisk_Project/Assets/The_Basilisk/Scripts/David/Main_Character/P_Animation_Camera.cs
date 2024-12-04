using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Animation_Camera : MonoBehaviour
{
    public Animator animation;
    private P_Character_Move playerMoving;

    private bool isWalkingLastState = false;

    private void Start()
    {
        if (!animation)
        {
            animation = GetComponent<Animator>();
            if (!animation)
            {
                Debug.LogError("Animator no encontrado. Por favor, asigna un Animator desde el Inspector.");
            }
        }

        playerMoving = FindObjectOfType<P_Character_Move>();
        if (!playerMoving)
        {
            Debug.LogError("No se encontró un objeto de tipo P_Character_Move en la escena.");
        }
    }

    private void Update()
    {
        if (playerMoving != null && animation != null)
        {
            bool isWalking = playerMoving.isWalking;
            if (isWalking != isWalkingLastState) 
            {
                animation.SetBool("isWalking", isWalking);
                isWalkingLastState = isWalking;
            }
        }
    }
}