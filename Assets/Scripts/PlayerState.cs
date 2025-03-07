using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // script contains states for the user, may expand later.

    public bool IsGrounded { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsOnLadder { get; private set; }

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        IsGrounded = characterController.isGrounded;
    }

    public void SetCrouching(bool crouching)
    {
        IsCrouching = crouching;
    }

    public void SetOnLadder(bool onLadder)
    {
        IsOnLadder = onLadder;
    }
}