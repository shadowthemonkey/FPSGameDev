using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // this script handles the movement of the player, it contains code for normal walking on the ground
    // but also air strafing, and ladder movement
    // the speed of the character is altered here, even though the action for crouching is worked on elsewhere

    // for simplicity, player jumping is also in this script, so that air strafing has a clearer implementation

    // another movement related feature is the dash ability, so it is located here too

    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 4.2f;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float airControl = 0.5f;
    [SerializeField] private float ladderClimbSpeed = 3f;

    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -9.81f;

    private PlayerInput playerInput;
    private PlayerState playerState;
    private Vector3 velocity;
    private float currentSpeed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        SetSpeed();
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        if (playerState.IsOnLadder)
        {
            HandleLadderMovement();
        }
        else
        {
            HandleGroundAndAirMovement();
        }
    }

    private void HandleJump()
    {
        if (playerInput.IsJumpPressed && (playerState.IsGrounded || playerState.IsOnLadder))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (playerState.IsOnLadder)
            {
                playerState.SetOnLadder(false);
            }
        }
    }

    private void HandleGroundAndAirMovement()
    {
        // calculate movement direction
        Vector3 move = transform.right * playerInput.MoveInput.x + transform.forward * playerInput.MoveInput.y;

        // apply movement
        if (playerState.IsGrounded)
        {
            characterController.Move(move * currentSpeed * Time.deltaTime);
        }
        else
        {
            // air strafing
            Vector3 airMove = move * currentSpeed * airControl * Time.deltaTime;
            characterController.Move(airMove);
        }

        // apply gravity
        if (!playerState.IsGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        // display speed in console for testing
        float speed = new Vector3(move.x * currentSpeed, 0, move.z * currentSpeed).magnitude;
        //Debug.Log($"Current Speed: {speed:F2} m/s");
    }

    private void HandleLadderMovement()
    {
        velocity.y = playerInput.MoveInput.y * ladderClimbSpeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void SetSpeed()
    {
        if (playerInput.IsWalkPressed)
        {
            currentSpeed = walkSpeed;
        }
        else
        {
            currentSpeed = playerInput.IsCrouchPressed ? crouchSpeed : moveSpeed;
        }
    }
}