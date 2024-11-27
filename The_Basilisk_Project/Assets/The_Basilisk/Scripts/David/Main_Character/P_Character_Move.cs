using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class P_Character_Move : MonoBehaviour
{
    [Header("=== Movement Settings ===")]
    [SerializeField] public float playerspeed;
    private CharacterController MyCC;
    public Animator Camera_Animation;
    private bool iswalking;


    private Vector3 inputVector;
    private Vector3 movementVector;
    private float myGravity = -10f;
    void Start()
    {
        MyCC = GetComponent<CharacterController>();
    }


    void Update()
    {
        GetInput();
        MovePlayer();
        CheckForHead();

        Camera_Animation.SetBool("isWalking", iswalking);

    }


    void GetInput()
    {
        inputVector = new Vector3(x: Input.GetAxisRaw("Horizontal"),y:0f, z:Input.GetAxisRaw("Vertical"));
        inputVector.Normalize();
        inputVector=transform.TransformDirection(inputVector);

        movementVector = (inputVector * playerspeed) + (Vector3.up * myGravity);

    }

    void MovePlayer()
    {
        MyCC.Move(movementVector*Time.deltaTime);
    }

    void CheckForHead()
    {
        if (MyCC.velocity.magnitude > 0.1)
        {
            iswalking = true;
        }else
        {
            iswalking = false;
        }
    }
}
