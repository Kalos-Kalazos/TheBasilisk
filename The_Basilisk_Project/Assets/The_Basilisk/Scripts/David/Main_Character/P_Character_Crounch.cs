/*using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : MonoBehaviour
{
    private P_Character_Move playermovement;
    private CharacterController characterController;

    public InputsPlayer playerInputs; 
    public float normalHeight = 4f; 
    public float crouchHeight = 0.5f; 
    public float crouchSpeed = 2f; 
    private bool isCrouching = false; 

    private void Start()
    {
        playermovement = GetComponent<P_Character_Move>();

        characterController = GetComponent<CharacterController>();

      
        if (playerInputs != null)
        {
            playerInputs.Player.Crouch.performed += ctx => ToggleCrouch(); 
        }
        else
        {
            Debug.LogError("playerInputs no está asignado.");
        }
    }

    private void ToggleCrouch()
    {
        if (isCrouching)
        {
            playermovement.playerSpeed = 10f;
            StandUp();
        }
        else
        {
            playermovement.playerSpeed = crouchSpeed; 
            Crouch();
        }
    }

    private void Crouch()
    {
        characterController.height = crouchHeight; 
        characterController.center = new Vector3(0, crouchHeight / 2, 0); 
        isCrouching = true;
    }

    private void StandUp()
    {
        characterController.height = normalHeight; 
        characterController.center = new Vector3(0, normalHeight / 2, 0); 
        isCrouching = false;
    }
}*/