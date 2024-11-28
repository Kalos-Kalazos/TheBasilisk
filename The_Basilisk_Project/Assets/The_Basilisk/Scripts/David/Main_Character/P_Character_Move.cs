using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Character_Move : MonoBehaviour
{
    InputsPlayer PlayerInputs; // Referencia a la clase de entradas personalizadas.

    [SerializeField] public float PlayerSpeed;
    [SerializeField] public float MoveSpeed = 10f;
    [SerializeField] public float SprintSpeed = 20f;
    [SerializeField] private CharacterController MyCC;
    [SerializeField] public Animator Camera_Animation;
    [SerializeField] private bool iswalking;

    private P_Raycast raycastScript;
    private Vector3 inputVector;
    private Vector3 movementVector;
    private float myGravity = -10f;

    public Transform cameraTransform; // Referencia al transform de la c�mara

    private void Awake()
    {
        // Instanciamos y asignamos las acciones de entrada
        PlayerInputs = new InputsPlayer();

        // Configuramos las entradas de movimiento
        PlayerInputs.Player.Move.performed += ctx => MoveInput(ctx.ReadValue<Vector2>());
        PlayerInputs.Player.Move.canceled += ctx => MoveInput(Vector2.zero);

        // Configuramos la acci�n de sprint
        PlayerInputs.Player.Sprint.started += ctx => Sprint(true);  // Empieza a esprintar
        PlayerInputs.Player.Sprint.canceled += ctx => Sprint(false);  // Deja de esprintar
    }

    private void OnEnable()
    {
        // Habilitamos las entradas
        PlayerInputs.Enable();
    }

    private void OnDisable()
    {
        // Deshabilitamos las entradas
        PlayerInputs.Disable();
    }

    void Start()
    {
        // Conseguimos el CharacterController
        MyCC = GetComponent<CharacterController>();

        // Conseguimos el transform de la c�mara, suponiendo que est� en el mismo objeto o est� asignado
        cameraTransform = Camera.main.transform;  // Usamos Camera.main para obtener la c�mara principal
    }

    void Update()
    {
        // Mover al jugador y actualizar la animaci�n
        MovePlayer();
        CheckForHead();

        // Actualiza la animaci�n de caminar
        Camera_Animation.SetBool("isWalking", iswalking);
    }

    void MoveInput(Vector2 input)
    {
        // Si la entrada es cero, no hacemos nada
        if (input == Vector2.zero)
        {
            inputVector = Vector3.zero;
            movementVector = Vector3.zero;
            return;
        }

        // Obtener la direcci�n de la c�mara (sin la componente Y, ya que es para la altura)
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Queremos asegurarnos de que el movimiento no tiene componente Y (vertical) 
        cameraForward.y = 0f; // Evitar que el personaje se mueva verticalmente
        cameraRight.y = 0f;

        // Normalizamos las direcciones para evitar velocidad extra al moverse diagonalmente
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Crear el vector de movimiento en funci�n de las teclas de entrada (W, A, S, D) y la c�mara
        inputVector = cameraForward * input.y + cameraRight * input.x;

        // Normalizamos el vector de movimiento para mantener velocidad constante
        inputVector.Normalize();

        // Aplica la velocidad de movimiento y la gravedad
        movementVector = (inputVector * PlayerSpeed) + (Vector3.up * myGravity);
    }

    void MovePlayer()
    {
        // Mueve al jugador usando CharacterController
        MyCC.Move(movementVector * Time.deltaTime);
    }

    void CheckForHead()
    {
        // Si la velocidad es mayor que un umbral, el jugador est� caminando
        if (MyCC.velocity.magnitude > 0.1f)
        {
            iswalking = true;
        }
        else
        {
            iswalking = false;
        }
    }

    void Sprint(bool isSprinting)
    {
        // Si el jugador est� sprintando, cambia la velocidad
        if (isSprinting)
        {
            PlayerSpeed = SprintSpeed;
        }
        else
        {
            PlayerSpeed = MoveSpeed;
        }
    }
}