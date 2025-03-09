using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // script contains states for the user, may expand to hold more states later.

    public bool isGrounded { get; private set; }
    public bool isCrouching { get; private set; }
    public bool isDashing { get; private set; }
    public bool isOnLadder { get; private set; }

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;
    }

    public void SetCrouching(bool crouching)
    {
        isCrouching = crouching;
    }

    public void SetDashing(bool dashing)
    {
        isDashing = dashing;
    }

    public void SetOnLadder(bool onLadder)
    {
        isOnLadder = onLadder;
    }
}