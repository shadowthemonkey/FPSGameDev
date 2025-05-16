using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using static Weapon;

public class PlayerInputs : NetworkBehaviour
{
    // script takes in player's input and uses functions connected using the new input system
    // all player inputs will connect to a function here

    // public variables so they can be reached by the other scripts
    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsCrouchPressed { get; private set; }
    public bool IsWalkPressed { get; private set; }
    public bool IsDashPressed { get; private set; }

    // dash cooldown variables, if it stays a cooldown instead of a finite amount purchased per round
    private float dashCooldown = 5f; // cooldown duration in seconds
    private float lastDashTime = -Mathf.Infinity; // time when the last dash occurred, -infinity by default as there's no previous dash

    private bool canJump; // track if the player can jump (must release jump button first)

    // states used, one for player, one for weapons
    private PlayerState playerState;
    private WeaponManager weaponManager;
    private UIManager UIManager;

    private bool isShooting = false;
    private float nextFireTime = 0f;

    [Header("Camera and Audio")]
    public GameObject playerCamera;
    public AudioListener audioListener;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        weaponManager = GetComponent<WeaponManager>();
        UIManager = FindFirstObjectByType<UIManager>();
        /*
        GameObject uiManagerObj = GameObject.FindGameObjectWithTag("UIManager");
        if (uiManagerObj != null)
        {
            UIManager = uiManagerObj.GetComponent<UIManager>();
        }
        */
        canJump = true; // can jump initially set to true
    }

    private void Update()
    {

        if (weaponManager.GetCurrentWeapon().fireMode == FireMode.FullAuto && isShooting)
        {
            if (Time.time >= nextFireTime)
            {
                weaponManager?.Shoot();
                nextFireTime = Time.time + 1f / weaponManager.GetCurrentWeapon().fireRate;
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            // disable camera and audio for non-local players
            if (playerCamera != null)
                playerCamera.SetActive(false);
            if (audioListener != null)
                audioListener.enabled = false;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (UIManager.IsBuyMenuOpen) return; // player can't look around whilst in menu

        lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (UIManager.IsBuyMenuOpen) return;

        if (weaponManager.GetCurrentWeapon().fireMode == FireMode.SemiAuto || weaponManager.GetCurrentWeapon().fireMode == FireMode.Sniper)
        {
            if (context.performed)
            {
                weaponManager?.Shoot();
            }
        }
        else if (weaponManager.GetCurrentWeapon().fireMode == FireMode.FullAuto)
        {
            // logic for semi auto and sniper are basically the same
            if (context.performed)
            {
                isShooting = true;
            }
            else if (context.canceled)
            {
                isShooting = false;
            }
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        // can't zoom in if in buy menu
        if (UIManager.IsBuyMenuOpen) return;
        // can't scope in on non-snipers
        if (weaponManager.GetCurrentWeapon().fireMode == FireMode.Sniper)
        {
            if (context.performed)
            {
                weaponManager?.Scope();
            }
        }
    }

    // jump methods, the triple && if statement is to prevent holding jump, as context.started just didn't work for what I needed
    // this means that consistent jumps are more consistency based
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && canJump && playerState.IsGrounded)
        {
            IsJumpPressed = true;
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
        IsJumpPressed = false;
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        IsWalkPressed = context.performed;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        IsCrouchPressed = context.performed;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weaponManager?.Reload();
        }
    }

    public void OnBuy(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log(UIManager == null ? "UIManager is null" : "UIManager is set");
            UIManager?.ToggleBuyMenu();
        }
    }

    public void OnPrimary(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weaponManager?.SetPrimary();
        }
    }

    public void OnSecondary(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weaponManager?.SetSecondary();
        }
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weaponManager?.SetMelee();
        }
    }

    // dash related methods
    public void OnDash(InputAction.CallbackContext context)
    {
        // you need to press the button, it needs to be after the cooldown
        // this prevents holding down the button to dash indefinitely
        if (context.performed && Time.time >= lastDashTime + dashCooldown)
        {
            IsDashPressed = true;
            lastDashTime = Time.time; // update the last dash time
        }
    }

    // public method called in PlayerMovement.cs
    // this is called after the dashDuration ends so that you can't stay dashing
    public void ResetDash()
    {
        IsDashPressed = false;
    }
}   