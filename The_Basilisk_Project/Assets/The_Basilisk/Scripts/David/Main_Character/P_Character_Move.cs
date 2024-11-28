using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Character_Move : MonoBehaviour
{
    private InputsPlayer playerInputs;
    private CharacterController myCC;
    private Vector3 inputVector;
    private Vector3 movementVector;

    private float gravity = -9.81f;
    private float verticalSpeed = 0f;
    private bool isWalking;

    [SerializeField] public float playerSpeed = 10f;
    [SerializeField] private Animator cameraAnimation;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float groundCheckDistance = 0.1f;  
    [SerializeField] private LayerMask groundLayer; 

    [SerializeField] private float raycastDistance = 5f;  
    [SerializeField] private Color rayColor = Color.blue;  

    private Vector2 moveInput;

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
        ApplyGravity();
        MovePlayer();
        CheckForWalking();
        cameraAnimation.SetBool("isWalking", isWalking);

        
        PerformRaycast();
    }

    private void MovePlayer()
    {
        if (moveInput == Vector2.zero)
        {
            movementVector = Vector3.zero;
            return;
        }

        Vector3 forwardDirection = cameraTransform.forward;
        forwardDirection.y = 0f;  
        forwardDirection.Normalize();

        Vector3 rightDirection = cameraTransform.right;
        rightDirection.y = 0f; 
        rightDirection.Normalize();

        inputVector = forwardDirection * moveInput.y + rightDirection * moveInput.x;
        inputVector.Normalize();

        movementVector = inputVector * playerSpeed;

        movementVector.y = verticalSpeed;  
        myCC.Move(movementVector * Time.deltaTime);  
    }

    private void CheckForWalking()
    {
        isWalking = myCC.velocity.magnitude > 0.1f;
    }

    private void ApplyGravity()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            verticalSpeed = -1f;  
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime; 
        }
    }

    private void PerformRaycast()
    {
        Ray ray = new Ray(transform.position, cameraTransform.forward);  
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
        }

        Debug.DrawRay(transform.position, cameraTransform.forward * raycastDistance, rayColor);
    }

    //mecánica de escalada
    /*
    private void Climb()
    {
        float verticalMovement = playerInputs.Player.Move.ReadValue<Vector2>().y * climbSpeed;
        Vector3 climbDirection = new Vector3(0f, verticalMovement, 0f);

        myCC.Move(climbDirection * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Climb"))
        {
            isClimbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climb"))
        {
            isClimbing = false;
        }
    }
    */
}