using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    // script alters the height of the player based on if they are crouched or not, this script does NOT affect crouch movement speed

    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchTransitionSpeed = 8f;

    private PlayerInput playerInput;
    private PlayerState playerState;
    private CharacterController characterController;
    private float targetHeight;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleCrouch();
        SmoothCrouchTransition();
    }

    private void HandleCrouch()
    {
        if (playerInput.IsCrouchPressed)
        {
            playerState.SetCrouching(true);
            targetHeight = crouchHeight;
        }
        else
        {
            playerState.SetCrouching(false);
            targetHeight = standingHeight;
        }
    }

    private void SmoothCrouchTransition()
    {
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
    }
}