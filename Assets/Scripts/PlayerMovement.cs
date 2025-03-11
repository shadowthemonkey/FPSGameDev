using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // this script handles the movement of the player, it contains code for normal walking on the ground
    // but also air strafing, and ladder movement
    // the speed of the character is altered here, even though the action for crouching is worked on elsewhere

    // for simplicity, player jumping is also in this script, so that air strafing has a clearer implementation

    // another movement related feature is the dash ability, so it is located here too

    [SerializeField] private CharacterController characterController;

    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float crouchSpeed = 1.75f;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float ladderClimbSpeed = 3f;

    [SerializeField] private float groundAccelerate = 15f;
    [SerializeField] private float airAccelerate = 5f;
    [SerializeField] private float maxVelocityAir = 20f;
    [SerializeField] private float friction = 10f; //how fast the player slows down after letting go of movement inputs,
    // lower thab groundAccelerate so that counterstrafing is a faster method of reaching 0 speed.

    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -9.81f;

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

        // log player's speed for testing
        LogSpeed();
    }

    private void HandleMovement()
    {
        if (playerState.isDashing)
        {
            // sets dashing and reduces timer, can't use other movement buttons whilst dashing
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            //Debug.Log($"Time.time: {Time.time} dashEndTime: {dashEndTime}");
            if (Time.time >= dashEndTime)
            {
                // dash duration is over
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
                // moving whilst grounded, and moving whilst in air, this handles most movement scenarios
                HandleGroundAndAirMovement();
            }
        }
    }

    private void HandleJump()
    {
        // can only jump if pressing the jump button and is either grounded or on a ladder
        if (playerInput.isJumpPressed && (playerState.isGrounded || playerState.isOnLadder))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (playerState.isOnLadder)
            {
                // jump off ladder
                playerState.SetOnLadder(false);
            }
            playerInput.ResetJump(); // call the method to reset IsJumpPressed to false
            // this means that the player can't hold down the button to automatically jump, they must perform the input again
        }
    }

    // movement for grounded movement (walking, crouching, etc) and air movement (keeping momentum, air strafes, etc)
    private void HandleGroundAndAirMovement()
    {
        // get movement input
        Vector2 input = playerInput.moveInput;

        // calc desired movement direction
        Vector3 moveDirection = (transform.right * input.x + transform.forward * input.y).normalized;
        // moveDirection is passed into MoveGround and MoveAir to have the direction the user needs to accelerate to

        // velocity is the previous vector3 showing what velocity was last update
        if (playerState.isGrounded)
        {
            // ground movement
            velocity = MoveGround(moveDirection, velocity);
        }
        else
        {
            // air movement
            velocity = MoveAir(moveDirection, velocity);

            // apply gravity since in air
            velocity.y += gravity * Time.deltaTime;
        }

        // apply movement changes to the character
        characterController.Move(velocity * Time.deltaTime);
    }

    // this version of movement allows the player to counterstrafe in ground
    // if you were to walk left and wanted to slow down faster, you can stop almost immediately by letting go of A, then tapping D
    
    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        // apply friction only if no input is given, this is basically deceleration
        if (playerInput.moveInput.magnitude == 0)
        {
            // speed is the previous velocity's magnitude
            // every update of no moveInput decreases prevVelocity, which is returned as the velocity for this update
            float speed = prevVelocity.magnitude;
            if (speed != 0) // no divide by zero
            {
                float drop = speed * friction * Time.deltaTime;
                prevVelocity *= Mathf.Max(speed - drop, 0) / speed; // scales the velocity down based on friction
            }
        }
        else
        {
            // moveInput is given, accelerate to current speed

            // calc target velocity based on currentSpeed
            Vector3 targetVelocity = accelDir * currentSpeed;

            // calc difference between the target velocity and the current velocity
            Vector3 velocityDifference = targetVelocity - prevVelocity;

            // then apply acceleration to gradually reach the target velocity
            prevVelocity += groundAccelerate * Time.deltaTime * velocityDifference;

            // enforced speed limit without affecting movement direction
            if (prevVelocity.magnitude > currentSpeed)
            {
                prevVelocity = prevVelocity.normalized * currentSpeed;
            }
        }

        return prevVelocity;
    }

    // I am not currently happy with my implementation, strafing doesn't act like source engine physics
    private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {
        float maxSpeed = maxVelocityAir;

        // project velocity onto acceleration direction
        float projVel = Vector3.Dot(prevVelocity, accelDir);
        float accelVel = airAccelerate * Time.deltaTime;

        // if projected velocity exceeds max speed, limit acceleration
        if (projVel + accelVel > maxSpeed)
        {
            accelVel = Mathf.Max(maxSpeed - projVel, 0);
        }

        // apply acceleration to the new velocity
        Vector3 newVelocity = prevVelocity + accelDir * accelVel;

        // enforce speed limit without affecting movement direction
        if (newVelocity.magnitude > maxSpeed)
        {
            newVelocity = newVelocity.normalized * maxSpeed;
        }

        return newVelocity;
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
        // if crouch walking: set the speed to crouchSpeed
        // if shift/slow walking: set the speed to walkSpeed
        // if crouched and slow walking: speed is set to the slower speed (crouchSpeed)
        if (playerInput.isCrouchPressed)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = playerInput.isWalkPressed ? walkSpeed : moveSpeed;
        }
    }

    private void LogSpeed()
    {
        // calc horizontal speed
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        float speed = horizontalVelocity.magnitude;

        Debug.Log($"Player Speed: {speed:F2} m/s");
    }
}