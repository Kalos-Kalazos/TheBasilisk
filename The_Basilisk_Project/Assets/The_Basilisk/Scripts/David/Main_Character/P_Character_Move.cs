using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Character_Move : MonoBehaviour
{
    public InputsPlayer playerInputs;
    public CharacterController myCC;
    public Vector3 inputVector;
    public Vector3 movementVector;

    public float gravity = -9.81f;
    public float verticalSpeed = 0f;
    public bool isWalking;

    [SerializeField] public float playerSpeed = 10f;
    [SerializeField] public Transform cameraTransform;

    [SerializeField] public float groundCheckDistance = 0.1f;
    [SerializeField] public float climbSpeed = 3f;
    [SerializeField] public bool isClimbing = false;

    private Vector2 moveInput;

    public bool isGrounded;

    private void Awake()
    {
        playerInputs = new InputsPlayer();
        playerInputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;
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
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (!isClimbing)
        {
            isGrounded = myCC.isGrounded;
            MovePlayer();
            ApplyGravity();
            CheckForWalking();
        }
        else
        {
            ClimbPlayer();
        }
    }

    private void MovePlayer()
    {
        if (moveInput == Vector2.zero)
        {
            movementVector = Vector3.zero;
        }
        else
        {
            Vector3 forwardDirection = cameraTransform.forward;
            forwardDirection.y = 0f;
            forwardDirection.Normalize();

            Vector3 rightDirection = cameraTransform.right;
            rightDirection.y = 0f;
            rightDirection.Normalize();

            inputVector = forwardDirection * moveInput.y + rightDirection * moveInput.x;
            inputVector.Normalize();

            movementVector = inputVector * playerSpeed;
        }

        movementVector.y = verticalSpeed;
        myCC.Move(movementVector * Time.deltaTime);
    }

    private void CheckForWalking()
    {
        isWalking = myCC.velocity.magnitude > 0.1f;
    }

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            verticalSpeed = -1f;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }

        if (verticalSpeed < -50f)
        {
            verticalSpeed = -50f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = true;
            verticalSpeed = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = false;
        }
    }

    private void ClimbPlayer()
    {
        float climbDirection = moveInput.y;
        movementVector = Vector3.up * climbDirection * climbSpeed;
        movementVector.y = 0;
        myCC.Move(movementVector * Time.deltaTime);
    }
}