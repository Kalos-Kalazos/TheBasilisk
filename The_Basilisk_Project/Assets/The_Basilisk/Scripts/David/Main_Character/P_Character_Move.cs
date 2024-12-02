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

    // Limitar la velocidad de caída (opcional)
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

        movementVector.y = verticalSpeed; // Aplicamos la velocidad vertical (gravedad o salto)
        myCC.Move(movementVector * Time.deltaTime);
    }

    private void CheckForWalking()
    {
        isWalking = myCC.velocity.magnitude > 0.1f;
    }

    private void ApplyGravity()
    {
        // Si estamos en el suelo, mantenemos la velocidad vertical baja para no hundirnos en el suelo
        if (isGrounded && !isClimbing)
        {
            if (verticalSpeed < 0)
            {
                verticalSpeed = -1f; // Una pequeña velocidad negativa para mantener al personaje en el suelo
            }
        }
        else if (!isGrounded && !isClimbing)
        {
            // Si estamos en el aire, aplicamos la gravedad
            verticalSpeed += gravity * Time.deltaTime;

            // Limitar la velocidad de caída
            if (verticalSpeed < maxFallSpeed)
            {
                verticalSpeed = maxFallSpeed;
            }
        }
    }

    private void Jump()
    {
        // Si estamos en el suelo y no estamos escalando, realizamos el salto
        if (isGrounded && !isClimbing)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculamos la velocidad vertical para el salto
        }

        // Si estamos escalando y presionamos el botón de salto, dejamos de escalar y saltamos
        if (isClimbing && Input.GetButtonDown("Jump"))
        {
            isClimbing = false;
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculamos la velocidad del salto
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = true;
            verticalSpeed = 0f; // Si tocamos una superficie escalable, detenemos el movimiento vertical
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = false;
            verticalSpeed = 0f; // Si dejamos de escalar, detenemos el movimiento vertical
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