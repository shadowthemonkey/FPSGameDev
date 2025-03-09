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

    [SerializeField] private float dashSpeed = 30f; // speed of the dash
    [SerializeField] private float dashDuration = 0.2f; // duration of the dash

    // input and state taken from respective scripts
    private PlayerInputs playerInput;
    private PlayerState playerState;

    private Vector3 velocity;
    private float currentSpeed; // altered depending on inputs, changed in SetSpeed()

    private float dashEndTime; // time when the dash ends
    private Vector3 dashDirection; // direction of the dash

    private void Awake()
    {
        playerInput = GetComponent<PlayerInputs>();
        playerState = GetComponent<PlayerState>();
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        SetSpeed();
        HandleMovement();
        HandleDash();
        HandleJump();
    }

    private void HandleMovement()
    {
        if (playerState.isDashing)
        {
            // sets dashing and reduces timer, can't use movement buttons whilst dashing
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            //Debug.Log($"Time.time: {Time.time} dashEndTime: {dashEndTime}");
            if (Time.time >= dashEndTime)
            {
                playerState.SetDashing(false);
                playerInput.ResetDash(); // call the method to reset IsDashPressed to false
            }
        }
        else
        {
            if (playerState.isOnLadder)
            {
                HandleLadderMovement();
            }
            else
            {
                HandleGroundAndAirMovement();
            }
        }
    }

    private void HandleJump()
    {
        if (playerInput.isJumpPressed && (playerState.isGrounded || playerState.isOnLadder))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (playerState.isOnLadder)
            {
                playerState.SetOnLadder(false);
            }
            playerInput.ResetJump(); // call the method to reset IsJumpPressed to false
        }
    }

    private void HandleGroundAndAirMovement()
    {
        // calculate movement direction
        Vector3 move = transform.right * playerInput.moveInput.x + transform.forward * playerInput.moveInput.y;

        // apply movement
        if (playerState.isGrounded)
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
        if (!playerState.isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        // display speed in console for testing
        float speed = new Vector3(move.x * currentSpeed, 0, move.z * currentSpeed).magnitude;
        //Debug.Log($"Current Speed: {speed:F2} m/s");
    }

    private void HandleDash()
    {
        if (playerInput.isDashPressed && !playerState.isDashing)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        playerState.SetDashing(true);
        dashEndTime = Time.time + dashDuration;

        // calculate dash direction based on movement input
        dashDirection = transform.right * playerInput.moveInput.x + transform.forward * playerInput.moveInput.y;

        // normalize the direction to ensure consistent dash speed
        if (dashDirection.magnitude > 0)
        {
            dashDirection.Normalize();
        }
        else
        {
            // if no movement input, dash forward
            dashDirection = transform.forward;
        }

        //Debug.Log("Dashing in direction: " + dashDirection);
    }

    private void HandleLadderMovement()
    {
        velocity.y = playerInput.moveInput.y * ladderClimbSpeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void SetSpeed()
    {
        if (playerInput.isWalkPressed)
        {
            currentSpeed = walkSpeed;
        }
        else
        {
            currentSpeed = playerInput.isCrouchPressed ? crouchSpeed : moveSpeed;
        }
    }
}