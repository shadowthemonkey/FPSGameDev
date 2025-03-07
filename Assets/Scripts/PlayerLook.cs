using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // this script is used for the first person camera control

    [SerializeField] private Transform playerCamera;
    [SerializeField] private float lookSensitivity = 0.5f; // serialized, can possibily be altered in a settings menu later on
    [SerializeField] private float lookXLimit = 90f;

    private PlayerInput playerInput;
    private float rotationX = 0f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked; // locks cursor for FPS control
        Cursor.visible = false; //cursor is locked in the centre and invisible
    }

    private void Update()
    {
        HandleLook(); // calls look method
    }

    private void HandleLook()
    {
        float mouseX = playerInput.LookInput.x * lookSensitivity;
        float mouseY = playerInput.LookInput.y * lookSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}