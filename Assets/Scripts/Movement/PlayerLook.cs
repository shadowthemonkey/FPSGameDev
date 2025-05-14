using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // this script is used for the first person camera control

    [SerializeField] private Transform playerCamera;
    [SerializeField] private float lookSensitivity = 0.5f; // serialized, can possibily be altered in a settings menu later on
    [SerializeField] private float lookXLimit = 90f;

    private PlayerInputs playerInput;
    private float rotationX = 0f;

    // recoil
    public float recoilSmoothTime = 0.05f;
    public float recoilDecayDelay = 0.1f;

    private Vector2 currentRecoil = Vector2.zero;
    private Vector2 targetRecoil = Vector2.zero;
    private Vector2 recoilVelocity = Vector2.zero;

    private bool decaying = false;

    private float previousRecoilX = 0f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInputs>();
        Cursor.lockState = CursorLockMode.Locked; // locks cursor for FPS control
        Cursor.visible = false; // cursor is locked in the centre and invisible
    }

    private void Update()
    {
        HandleLook(); // calls look method

        if (decaying)
        {
            // damp to origin recoil
            currentRecoil = Vector2.SmoothDamp(currentRecoil, Vector2.zero, ref recoilVelocity, recoilSmoothTime);
        }
        else
        {
            // If not decaying, match target recoil
            currentRecoil = targetRecoil;
        }
    }

    // function handles first person camera according to mouse input
    private void HandleLook()
    {
        float mouseX = playerInput.lookInput.x * lookSensitivity;
        float mouseY = playerInput.lookInput.y * lookSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        // apply vertical mouse + vertical recoil
        playerCamera.localRotation = Quaternion.Euler(rotationX + currentRecoil.y, 0f, 0f);

        // remove last frame's recoil
        transform.Rotate(Vector3.up * -previousRecoilX);

        // apply mouse + new recoil
        float totalYaw = mouseX + currentRecoil.x;
        transform.Rotate(Vector3.up * totalYaw);

        // store applied recoil for next frame
        previousRecoilX = currentRecoil.x;
    }

    public void ApplyRecoil(Vector2 recoilOffset)
    {
        targetRecoil = recoilOffset;

        decaying = false; // spraying, so no decay
        CancelInvoke(nameof(StartRecoilDecay)); // so the decay invoke should cancel
        Invoke(nameof(StartRecoilDecay), recoilDecayDelay); //invoke again for next check
    }
    private void StartRecoilDecay()
    {
        decaying = true; // if true then lerp back to origin
    }

    public Vector2 GetCurrentRecoil()
    {
        return currentRecoil;
    }
}