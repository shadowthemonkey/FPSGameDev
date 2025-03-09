using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerInputs : MonoBehaviour
{
    // script takes in player's input and uses functions connected using the new input system
    // all player inputs will connect to a function here

    // public variables so they can be reached by the other scripts
    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool isJumpPressed { get; private set; }
    public bool isCrouchPressed { get; private set; }
    public bool isWalkPressed { get; private set; }
    public bool isDashPressed { get; private set; }

    // dash cooldown variables, if it stays a cooldown instead of a finite amount purchased per round
    private float dashCooldown = 5f; // cooldown duration in seconds
    private float lastDashTime = -Mathf.Infinity; // time when the last dash occurred, -infinity by default as there's no previous dash

    private bool canJump = true; // track if the player can jump (must release jump button first)

    private PlayerState playerState;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    // jump methods, the triple && if statement is to prevent holding jump, as context.started just didn't work for what I needed
    // this means that consistent jumps are more consistency based
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && canJump && playerState.isGrounded)
        {
            isJumpPressed = true;
            canJump = false; // disable jumping until the button is released
        }

        if (context.canceled)
        {
            canJump = true; // allow jumping again after the button is released
        }
    }
    // public method called in PlayerMovement.cs
    public void ResetJump()
    {
        isJumpPressed = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouchPressed = context.performed;
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        isWalkPressed = context.performed;
    }


    // dash related methods
    public void OnDash(InputAction.CallbackContext context)
    {
        // you need to press the button, it needs to be after the cooldown
        // this prevents holding down the button to dash indefinitely
        if (context.performed && Time.time >= lastDashTime + dashCooldown)
        {
            isDashPressed = true;
            lastDashTime = Time.time; // update the last dash time
        }
    }

    // public method called in PlayerMovement.cs
    // this is called after the dashDuration ends so that you can't stay dashing
    public void ResetDash()
    {
        isDashPressed = false;
    }
}   