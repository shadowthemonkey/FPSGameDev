using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // this script is used for the first person camera control

    [SerializeField] private Transform playerCamera;
    [SerializeField] private float lookSensitivity = 0.5f; // serialized, can possibily be altered in a settings menu later on
    [SerializeField] private float lookXLimit = 90f;

    private PlayerInputs playerInput;
    private float rotationX = 0f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInputs>();
        Cursor.lockState = CursorLockMode.Locked; // locks cursor for FPS control
        Cursor.visible = false; // cursor is locked in the centre and invisible
    }

    private void Update()
    {
        HandleLook(); // calls look method
    }

    // function handles first person camera according to mouse input
    private void HandleLook()
    {
        float mouseX = playerInput.lookInput.x * lookSensitivity;
        float mouseY = playerInput.lookInput.y * lookSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}