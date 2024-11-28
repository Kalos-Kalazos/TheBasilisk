using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_Move : MonoBehaviour
{
    private InputsPlayer playerInputs;
    private P_Raycast raycastScript;
    private CharacterController myCC;
    private Vector3 inputVector;
    private Vector3 movementVector;
    private float myGravity = -10f;
    private bool isWalking;

    [SerializeField] private float playerSpeed;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintSpeed = 20f;
    [SerializeField] private Animator cameraAnimation;
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        playerInputs = new InputsPlayer();
        playerInputs.Player.Move.performed += ctx => MoveInput(ctx.ReadValue<Vector2>());
        playerInputs.Player.Move.canceled += ctx => MoveInput(Vector2.zero);
        playerInputs.Player.Sprint.started += ctx => Sprint(true);
        playerInputs.Player.Sprint.canceled += ctx => Sprint(false);
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    private void Start()
    {
        myCC = GetComponent<CharacterController>();
        raycastScript = FindObjectOfType<P_Raycast>();
        cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        CheckForWalking();
        cameraAnimation.SetBool("isWalking", isWalking);
    }

    private void MoveInput(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            inputVector = Vector3.zero;
            movementVector = Vector3.zero;
            return;
        }

        Vector3 raycastDirection = raycastScript.GetRaycastDirection();
        raycastDirection.y = 0f;
        raycastDirection.Normalize();

        inputVector = raycastDirection * input.y + cameraTransform.right * input.x;
        inputVector.Normalize();

        movementVector = (inputVector * playerSpeed) + (Vector3.up * myGravity);
    }

    private void MovePlayer()
    {
        myCC.Move(movementVector * Time.deltaTime);
    }

    private void CheckForWalking()
    {
        isWalking = myCC.velocity.magnitude > 0.1f;
    }

    private void Sprint(bool isSprinting)
    {
        playerSpeed = isSprinting ? sprintSpeed : moveSpeed;
    }
}