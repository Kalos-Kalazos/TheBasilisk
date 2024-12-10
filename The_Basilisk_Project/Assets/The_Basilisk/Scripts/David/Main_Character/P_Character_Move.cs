using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Character_Move : MonoBehaviour
{
    public InputsPlayer playerInputs;
    public CharacterController myCC;
    public Vector2 inputVector; 
    public Vector3 movementVector;
    private P_Character_HookGrappling pGrapple;
    private P_Character_HookSwing pSwing;

    public float gravity = -9.81f;
    public float yVelocity = 0f;
    public bool isWalking;
    public bool isCrouched;

    [SerializeField] public float playerSpeed = 10f;
    [SerializeField] public float crouchSpeed = 2f;
    [SerializeField] public float swingSpeed;
    [SerializeField] public Transform cameraTransform;
    [SerializeField] public float jumpHeight = 2f;
    [SerializeField] public float characterHeightStandUp = 2.0f;
    [SerializeField] public float characterHeightCrouched = 1.0f;
    [SerializeField] public float groundCheckDistance = 0.1f;

    private bool isGrounded;
    private bool canJump = true;

    public Vector2 moveInput { get { return inputVector; } }

    private void Awake()
    {
        playerInputs = new InputsPlayer();
        playerInputs.Player.Move.performed += ctx => inputVector = ctx.ReadValue<Vector2>();
        playerInputs.Player.Move.canceled += ctx => inputVector = Vector2.zero;
        playerInputs.Player.Jump.performed += ctx => Jump();
        playerInputs.Player.Crouch.performed += ctx => ToggleCrouch();
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
        pGrapple = GetComponent<P_Character_HookGrappling>();
        pSwing = GetComponent<P_Character_HookSwing>();

        cameraTransform = Camera.main?.transform;
        if (!myCC) Debug.LogError("CharacterController no encontrado en el objeto.");
        if (!cameraTransform) Debug.LogError("Cámara principal no encontrada.");
    }

    private void Update()
    {
        CheckGrounded();

        
        if (pGrapple != null && pGrapple.activeGrapple ||
            pSwing != null && pSwing.swinging)
        {
           
            return;
        }

        MovePlayer();
        ApplyGravity();
        CheckForWalking();
    }

    private void CheckGrounded()
    {
        isGrounded = myCC.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + myCC.skinWidth);
        if (isGrounded && yVelocity < 0)
        {
            yVelocity = -1f; 
        }
    }

    private void MovePlayer()
    {
        if (!myCC.enabled) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0;
        right.Normalize();

        Vector3 movementDirection = forward * inputVector.y + right * inputVector.x;
        movementDirection.Normalize();

        movementVector = movementDirection * (isCrouched ? crouchSpeed : playerSpeed);
        movementVector.y = yVelocity;

        myCC.Move(movementVector * Time.deltaTime);
    }

    private void CheckForWalking()
    {
        isWalking = myCC.velocity.magnitude > 0.1f;
    }

    private void ApplyGravity()
    {
        if (!myCC.enabled) return;

        if (!isGrounded)
        {
            yVelocity += gravity * Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (pGrapple != null && pGrapple.activeGrapple ||
            pSwing != null && pSwing.swinging) return;

        if (isGrounded && !isCrouched && canJump)
        {
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ToggleCrouch()
    {
        if (!myCC.enabled) return;

        if (isGrounded)
        {
            isCrouched = !isCrouched;
            canJump = !isCrouched;

            myCC.height = isCrouched ? characterHeightCrouched : characterHeightStandUp;
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isGrounded = true;
            yVelocity = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isGrounded = false;
        }
    }*/
}
