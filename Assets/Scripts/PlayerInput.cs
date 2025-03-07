using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // script takes in player's input and uses functions connected using the new input system
    // all player inputs will connect to a function here

    // public variables so they can be reached by the other scripts
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsCrouchPressed { get; private set; }
    public bool IsWalkPressed { get; private set; }
    public bool IsDashPressed { get; private set; }

    // dash cooldown variables, if it stays a cooldown instead of a finite amount purchased per round
    private float _dashCooldown = 5f; // cooldown duration in seconds
    private float _lastDashTime = -Mathf.Infinity; // time when the last dash occurred


    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        IsJumpPressed = context.performed; // work on jump not being continous, onButtonDown so you can't easily repeat jumps
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        IsCrouchPressed = context.performed;
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        IsWalkPressed = context.performed;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // work on this also not being holdable
        if (context.performed && Time.time > _lastDashTime + _dashCooldown)
        {
            IsDashPressed = true;
            _lastDashTime = Time.time; // Update the last dash time
        }
        else
        {
            IsDashPressed = false;
        }
    }
}   