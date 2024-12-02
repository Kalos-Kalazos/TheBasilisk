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
    [SerializeField] public float jumpHeight = 2f;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool isJumping = false;

    [SerializeField] private float maxFallSpeed = -20f;

    private void Awake()
    {
        playerInputs = new InputsPlayer();
        playerInputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        playerInputs.Player.Jump.performed += ctx => Jump();
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
        isGrounded = myCC.isGrounded;

        if (isClimbing)
        {
            ClimbPlayer();
        }
        else
        {
            MovePlayer();
            ApplyGravity();
            CheckForWalking();
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
        if (isGrounded && !isClimbing)
        {
            if (verticalSpeed < 0)
            {
                verticalSpeed = -1f;
            }
        }
        else if (!isGrounded && !isClimbing)
        {
            verticalSpeed += gravity * Time.deltaTime;

            if (verticalSpeed < maxFallSpeed)
            {
                verticalSpeed = maxFallSpeed;
            }
        }
    }

    private void Jump()
    {
        if (isGrounded && !isClimbing)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isClimbing && Input.GetButtonDown("Jump"))
        {
            isClimbing = false;
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
            verticalSpeed = 0f;
        }
    }

    private void ClimbPlayer()
    {
        float climbDirection = moveInput.y;

        if (climbDirection != 0)
        {
            movementVector = Vector3.up * climbDirection * climbSpeed;
            movementVector.x = 0f;
            movementVector.z = 0f;
            myCC.Move(movementVector * Time.deltaTime);
        }
        else
        {
            movementVector = Vector3.zero;
            myCC.Move(movementVector * Time.deltaTime);
        }
    }
}